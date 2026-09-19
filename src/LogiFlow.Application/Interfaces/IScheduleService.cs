using LogiFlow.Application.DTOs.Schedules;

namespace LogiFlow.Application.Interfaces;

public interface IScheduleService
{
    Task<IReadOnlyCollection<ScheduleResponse>> GetAllAsync(
        CancellationToken ct);

    Task<ScheduleResponse> GetByIdAsync(
        int id,
        CancellationToken ct);

    Task<ScheduleResponse> CreateAsync(
        CreateScheduleRequest request,
        CancellationToken ct);

    Task<ScheduleResponse> UpdateAsync(
        int id,
        UpdateScheduleRequest request,
        CancellationToken ct);

    Task DeleteAsync(
        int id,
        CancellationToken ct);

    Task<ScheduleResponse> RescheduleAsync(
        int id,
        RescheduleRequest request,
        CancellationToken ct);

    Task<ScheduleResponse> CancelAsync(
        int id,
        CancellationToken ct);

    Task<IReadOnlyCollection<ScheduleResponse>> GetDriverScheduleAsync(
        int driverId,
        CancellationToken ct);

    Task<IReadOnlyCollection<ScheduleResponse>> GetVehicleScheduleAsync(
        int vehicleId,
        CancellationToken ct);
}