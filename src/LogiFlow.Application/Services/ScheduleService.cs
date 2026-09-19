using LogiFlow.Application.DTOs.Schedules;
using LogiFlow.Application.Interfaces;
using LogiFlow.Application.DTOs.Notifications;
using LogiFlow.Domain.Entities;
using LogiFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Application.Services;

public class ScheduleService(IApplicationDbContext db, INotificationService notifications) : IScheduleService
{
    public async Task<IReadOnlyCollection<ScheduleResponse>> GetAllAsync(
        CancellationToken ct)
    {
        return await db.Schedules
            .AsNoTracking()
            .OrderBy(x => x.StartTimeUtc)
            .Select(MapExpression())
            .ToListAsync(ct);
    }

    public async Task<ScheduleResponse> GetByIdAsync(
        int id,
        CancellationToken ct)
    {
        var schedule = await db.Schedules
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Schedule not found.");

        return Map(schedule);
    }

    public async Task<ScheduleResponse> CreateAsync(
        CreateScheduleRequest request,
        CancellationToken ct)
    {
        ValidateUtc(request.StartTimeUtc, request.EndTimeUtc);

        await EnsureResourcesExistAsync(
            request.DriverId,
            request.VehicleId,
            ct);

        await EnsureNoConflictAsync(
            request.DriverId,
            request.VehicleId,
            request.StartTimeUtc,
            request.EndTimeUtc,
            null,
            ct);

        var schedule = new Schedule
        {
            ShipmentId = request.ShipmentId,
            DriverId = request.DriverId,
            VehicleId = request.VehicleId,
            StartTimeUtc = request.StartTimeUtc,
            EndTimeUtc = request.EndTimeUtc,
            Status = ScheduleStatus.Scheduled
        };

        db.Schedules.Add(schedule);

        await db.SaveChangesAsync(ct);

        await NotifyScheduleAsync(schedule, "Delivery scheduled", $"Shipment {schedule.ShipmentId} has been scheduled.", ct);
        return Map(schedule);
    }

    public async Task<ScheduleResponse> UpdateAsync(
        int id,
        UpdateScheduleRequest request,
        CancellationToken ct)
    {
        ValidateUtc(request.StartTimeUtc, request.EndTimeUtc);

        var schedule = await db.Schedules
            .SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Schedule not found.");

        if (schedule.Status is
            ScheduleStatus.Cancelled or
            ScheduleStatus.Completed)
        {
            throw new BadRequestException(
                "A cancelled or completed schedule cannot be updated.");
        }

        await EnsureResourcesExistAsync(
            request.DriverId,
            request.VehicleId,
            ct);

        await EnsureNoConflictAsync(
            request.DriverId,
            request.VehicleId,
            request.StartTimeUtc,
            request.EndTimeUtc,
            id,
            ct);

        schedule.ShipmentId = request.ShipmentId;
        schedule.DriverId = request.DriverId;
        schedule.VehicleId = request.VehicleId;
        schedule.StartTimeUtc = request.StartTimeUtc;
        schedule.EndTimeUtc = request.EndTimeUtc;
        schedule.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return Map(schedule);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken ct)
    {
        var schedule = await db.Schedules
            .SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Schedule not found.");

        if (schedule.Status == ScheduleStatus.Completed)
        {
            throw new BadRequestException(
                "A completed schedule cannot be deleted.");
        }

        db.Schedules.Remove(schedule);

        await db.SaveChangesAsync(ct);
    }

    public async Task<ScheduleResponse> RescheduleAsync(
        int id,
        RescheduleRequest request,
        CancellationToken ct)
    {
        ValidateUtc(request.StartTimeUtc, request.EndTimeUtc);

        var schedule = await db.Schedules
            .SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Schedule not found.");

        if (schedule.Status is
            ScheduleStatus.Cancelled or
            ScheduleStatus.Completed)
        {
            throw new BadRequestException(
                "A cancelled or completed schedule cannot be rescheduled.");
        }

        await EnsureNoConflictAsync(
            schedule.DriverId,
            schedule.VehicleId,
            request.StartTimeUtc,
            request.EndTimeUtc,
            id,
            ct);

        schedule.StartTimeUtc = request.StartTimeUtc;
        schedule.EndTimeUtc = request.EndTimeUtc;
        schedule.Status = ScheduleStatus.Rescheduled;
        schedule.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        await NotifyScheduleAsync(schedule, "Schedule rescheduled", $"Shipment {schedule.ShipmentId} schedule was changed.", ct);

        return Map(schedule);
    }

    public async Task<ScheduleResponse> CancelAsync(
        int id,
        CancellationToken ct)
    {
        var schedule = await db.Schedules
            .SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Schedule not found.");

        if (schedule.Status == ScheduleStatus.Completed)
        {
            throw new BadRequestException(
                "A completed schedule cannot be cancelled.");
        }

        schedule.Status = ScheduleStatus.Cancelled;
        schedule.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return Map(schedule);
    }

    public async Task<IReadOnlyCollection<ScheduleResponse>>
        GetDriverScheduleAsync(
            int driverId,
            CancellationToken ct)
    {
        if (!await db.Drivers.AnyAsync(x => x.Id == driverId, ct))
            throw new NotFoundException("Driver not found.");

        return await db.Schedules
            .AsNoTracking()
            .Where(x => x.DriverId == driverId)
            .OrderBy(x => x.StartTimeUtc)
            .Select(MapExpression())
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<ScheduleResponse>>
        GetVehicleScheduleAsync(
            int vehicleId,
            CancellationToken ct)
    {
        if (!await db.Vehicles.AnyAsync(x => x.Id == vehicleId, ct))
            throw new NotFoundException("Vehicle not found.");

        return await db.Schedules
            .AsNoTracking()
            .Where(x => x.VehicleId == vehicleId)
            .OrderBy(x => x.StartTimeUtc)
            .Select(MapExpression())
            .ToListAsync(ct);
    }

    private async Task NotifyScheduleAsync(Schedule schedule, string title, string message, CancellationToken ct)
    {
        var driverUserId = await db.Drivers.Where(x => x.Id == schedule.DriverId).Select(x => (int?)x.UserId).SingleOrDefaultAsync(ct);
        if (driverUserId.HasValue) await notifications.CreateAsync(new CreateNotificationRequest(driverUserId.Value, title, message, "Scheduling"), ct);
        var customerId = await db.Shipments.Where(x => x.Id == schedule.ShipmentId).Select(x => (int?)x.CustomerId).SingleOrDefaultAsync(ct);
        if (customerId.HasValue)
        {
            var userId = await db.Customers.Where(x => x.Id == customerId.Value).Select(x => (int?)x.UserId).SingleOrDefaultAsync(ct);
            if (userId.HasValue) await notifications.CreateAsync(new CreateNotificationRequest(userId.Value, title, message, "Scheduling"), ct);
        }
    }

    private async Task EnsureResourcesExistAsync(
        int driverId,
        int vehicleId,
        CancellationToken ct)
    {
        if (!await db.Drivers.AnyAsync(x => x.Id == driverId, ct))
            throw new NotFoundException("Driver not found.");

        if (!await db.Vehicles.AnyAsync(x => x.Id == vehicleId, ct))
            throw new NotFoundException("Vehicle not found.");
    }

    private async Task EnsureNoConflictAsync(
        int driverId,
        int vehicleId,
        DateTime start,
        DateTime end,
        int? excludeScheduleId,
        CancellationToken ct)
    {
        var conflict = await db.Schedules.AnyAsync(
            x =>
                x.Status != ScheduleStatus.Cancelled &&
                x.Status != ScheduleStatus.Completed &&
                (!excludeScheduleId.HasValue ||
                 x.Id != excludeScheduleId.Value) &&
                (x.DriverId == driverId ||
                 x.VehicleId == vehicleId) &&
                start < x.EndTimeUtc &&
                end > x.StartTimeUtc,
            ct);

        if (conflict)
        {
            throw new ConflictException(
                "The driver or vehicle already has an overlapping schedule.");
        }
    }

    private static void ValidateUtc(
        DateTime start,
        DateTime end)
    {
        if (start.Kind != DateTimeKind.Utc ||
            end.Kind != DateTimeKind.Utc)
        {
            throw new BadRequestException(
                "StartTimeUtc and EndTimeUtc must be UTC.");
        }

        if (start >= end)
        {
            throw new BadRequestException(
                "StartTimeUtc must be earlier than EndTimeUtc.");
        }
    }

    private static ScheduleResponse Map(
        Schedule schedule) =>
        new(
            schedule.Id,
            schedule.ShipmentId,
            schedule.DriverId,
            schedule.VehicleId,
            schedule.StartTimeUtc,
            schedule.EndTimeUtc,
            schedule.Status,
            schedule.CreatedAtUtc,
            schedule.UpdatedAtUtc);

    private static System.Linq.Expressions.Expression
        <Func<Schedule, ScheduleResponse>> MapExpression() =>
        x => new ScheduleResponse(
            x.Id,
            x.ShipmentId,
            x.DriverId,
            x.VehicleId,
            x.StartTimeUtc,
            x.EndTimeUtc,
            x.Status,
            x.CreatedAtUtc,
            x.UpdatedAtUtc);
}