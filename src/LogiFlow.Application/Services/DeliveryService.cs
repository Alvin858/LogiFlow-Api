
using LogiFlow.Application.DTOs.DeliveryDtos;
using LogiFlow.Application.Interfaces;
using LogiFlow.Application.DTOs.Notifications;
using LogiFlow.Domain.Entities;
using LogiFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Application.Services;

public class DeliveryService(IApplicationDbContext db, INotificationService notifications) : IDeliveryService
{
    public async Task<IReadOnlyCollection<DeliveryResponse>> GetAllAsync(
        CancellationToken ct)
    {
        return await db.Deliveries
            .AsNoTracking()
            .OrderByDescending(x => x.AssignedAtUtc)
            .Select(x => new DeliveryResponse(
                x.Id,
                x.ShipmentId,
                x.DriverId,
                x.VehicleId,
                x.RouteId,
                x.Status,
                x.AssignedAtUtc,
                x.CompletedAtUtc,
                x.FailureReason,
                x.ProofsOfDelivery.Any()))
            .ToListAsync(ct);
    }

    public async Task<DeliveryResponse> GetByIdAsync(
        int id,
        CancellationToken ct)
    {
        var delivery = await db.Deliveries
            .AsNoTracking()
            .Include(x => x.ProofsOfDelivery)
            .SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Delivery not found.");

        return Map(delivery);
    }

    public async Task<DeliveryResponse> CreateAsync(
        CreateDeliveryRequest request,
        CancellationToken ct)
    {
        await EnsureRouteExistsAsync(request.RouteId, ct);

        if (!await db.Drivers.AnyAsync(
                x => x.Id == request.DriverId,
                ct))
        {
            throw new NotFoundException("Driver not found.");
        }

        if (!await db.Vehicles.AnyAsync(
                x => x.Id == request.VehicleId,
                ct))
        {
            throw new NotFoundException("Vehicle not found.");
        }

        if (await HasActiveDeliveryForDriverAsync(
                request.DriverId, null, ct))
        {
            throw new ConflictException(
                "The driver already has another active delivery.");
        }

        if (await HasActiveDeliveryForVehicleAsync(
                request.VehicleId, null, ct))
        {
            throw new ConflictException(
                "The vehicle already has another active delivery.");
        }

        var delivery = new Delivery
        {
            ShipmentId = request.ShipmentId,
            DriverId = request.DriverId,
            VehicleId = request.VehicleId,
            RouteId = request.RouteId,
            Status = DeliveryStatus.Assigned,
            AssignedAtUtc = DateTime.UtcNow
        };

        db.Deliveries.Add(delivery);

        await db.SaveChangesAsync(ct);

        await NotifyDeliveryUsersAsync(delivery, "Delivery assigned", $"Delivery {delivery.Id} has been assigned to you.", ct, true);
        return Map(delivery);
    }

    public async Task<DeliveryResponse> UpdateAsync(
        int id,
        UpdateDeliveryRequest request,
        CancellationToken ct)
    {
        var delivery = await db.Deliveries
            .SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Delivery not found.");

        if (delivery.Status != DeliveryStatus.Assigned)
        {
            throw new BadRequestException(
                "Delivery assignment can only be changed while the delivery is Assigned.");
        }

        await EnsureRouteExistsAsync(request.RouteId, ct);

        if (!await db.Drivers.AnyAsync(
                x => x.Id == request.DriverId,
                ct))
        {
            throw new NotFoundException("Driver not found.");
        }

        if (!await db.Vehicles.AnyAsync(
                x => x.Id == request.VehicleId,
                ct))
        {
            throw new NotFoundException("Vehicle not found.");
        }

        if (await HasActiveDeliveryForDriverAsync(
                request.DriverId, id, ct))
        {
            throw new ConflictException(
                "The driver already has another active delivery.");
        }

        if (await HasActiveDeliveryForVehicleAsync(
                request.VehicleId, id, ct))
        {
            throw new ConflictException(
                "The vehicle already has another active delivery.");
        }

        delivery.ShipmentId = request.ShipmentId;
        delivery.DriverId = request.DriverId;
        delivery.VehicleId = request.VehicleId;
        delivery.RouteId = request.RouteId;

        await db.SaveChangesAsync(ct);

        return Map(delivery);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken ct)
    {
        var delivery = await db.Deliveries
            .SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Delivery not found.");

        if (delivery.Status is
            DeliveryStatus.PickedUp or
            DeliveryStatus.OutForDelivery or
            DeliveryStatus.Completed)
        {
            throw new BadRequestException(
                "An active or completed delivery cannot be deleted.");
        }

        db.Deliveries.Remove(delivery);

        await db.SaveChangesAsync(ct);
    }

    public async Task<DeliveryResponse> PickupAsync(
        int id,
        CancellationToken ct)
    {
        var delivery = await GetDeliveryAsync(id, ct);

        if (delivery.Status != DeliveryStatus.Assigned)
        {
            throw new BadRequestException(
                "Only an Assigned delivery can be picked up.");
        }

        delivery.Status = DeliveryStatus.PickedUp;

        await db.SaveChangesAsync(ct);
        await NotifyCustomerForDeliveryAsync(delivery, "Shipment picked up", $"Shipment {delivery.ShipmentId} has been picked up.", ct);

        return Map(delivery);
    }

    public async Task<DeliveryResponse> OutForDeliveryAsync(
        int id,
        CancellationToken ct)
    {
        var delivery = await GetDeliveryAsync(id, ct);

        if (delivery.Status != DeliveryStatus.PickedUp)
        {
            throw new BadRequestException(
                "Only a PickedUp delivery can move to OutForDelivery.");
        }

        delivery.Status = DeliveryStatus.OutForDelivery;

        await db.SaveChangesAsync(ct);
        await NotifyCustomerForDeliveryAsync(delivery, "Out for delivery", $"Shipment {delivery.ShipmentId} is out for delivery.", ct);

        return Map(delivery);
    }

    public async Task<DeliveryResponse> CompleteAsync(
        int id,
        CancellationToken ct)
    {
        var delivery = await GetDeliveryWithProofsAsync(id, ct);

        if (delivery.Status != DeliveryStatus.OutForDelivery)
        {
            throw new BadRequestException(
                "Only an OutForDelivery delivery can be completed.");
        }

        if (!delivery.ProofsOfDelivery.Any())
        {
            throw new BadRequestException(
                "Proof of delivery is required before completing the delivery.");
        }

        delivery.Status = DeliveryStatus.Completed;
        delivery.CompletedAtUtc = DateTime.UtcNow;
        delivery.FailureReason = null;

        await db.SaveChangesAsync(ct);
        await NotifyCustomerForDeliveryAsync(delivery, "Delivery completed", $"Shipment {delivery.ShipmentId} has been delivered successfully.", ct);

        return Map(delivery);
    }

    public async Task<DeliveryResponse> FailAsync(
        int id,
        FailDeliveryRequest request,
        CancellationToken ct)
    {
        var delivery = await GetDeliveryAsync(id, ct);

        if (delivery.Status is
            DeliveryStatus.Completed or
            DeliveryStatus.Failed)
        {
            throw new BadRequestException(
                "A completed or failed delivery cannot be failed again.");
        }

        delivery.Status = DeliveryStatus.Failed;
        delivery.FailureReason = request.FailureReason.Trim();

        await db.SaveChangesAsync(ct);
        await NotifyCustomerForDeliveryAsync(delivery, "Delivery failed", $"Delivery for shipment {delivery.ShipmentId} failed: {delivery.FailureReason}", ct);

        return Map(delivery);
    }

    public async Task<ProofOfDeliveryResponse> AddProofAsync(
        int deliveryId,
        CreateProofOfDeliveryRequest request,
        CancellationToken ct)
    {
        var delivery = await GetDeliveryAsync(deliveryId, ct);

        if (delivery.Status is
            DeliveryStatus.Completed or
            DeliveryStatus.Failed)
        {
            throw new BadRequestException(
                "Proof cannot be added to a completed or failed delivery.");
        }

        var proof = new ProofOfDelivery
        {
            DeliveryId = deliveryId,
            ReceiverName = request.ReceiverName.Trim(),
            SignaturePath = request.SignaturePath.Trim(),
            PhotoPath = request.PhotoPath.Trim(),
            Remarks = request.Remarks?.Trim(),
            CapturedAtUtc = DateTime.UtcNow
        };

        db.ProofsOfDelivery.Add(proof);

        await db.SaveChangesAsync(ct);

        return Map(proof);
    }

    private async Task NotifyCustomerForDeliveryAsync(Delivery delivery, string title, string message, CancellationToken ct)
    {
        var customerId = await db.Shipments.Where(x => x.Id == delivery.ShipmentId).Select(x => (int?)x.CustomerId).SingleOrDefaultAsync(ct);
        if (!customerId.HasValue) return;
        var userId = await db.Customers.Where(x => x.Id == customerId.Value).Select(x => (int?)x.UserId).SingleOrDefaultAsync(ct);
        if (userId.HasValue) await notifications.CreateAsync(new CreateNotificationRequest(userId.Value, title, message, "Delivery"), ct);
    }

    private async Task NotifyDeliveryUsersAsync(Delivery delivery, string title, string message, CancellationToken ct, bool includeDriver)
    {
        if (includeDriver)
        {
            var driverUserId = await db.Drivers.Where(x => x.Id == delivery.DriverId).Select(x => (int?)x.UserId).SingleOrDefaultAsync(ct);
            if (driverUserId.HasValue) await notifications.CreateAsync(new CreateNotificationRequest(driverUserId.Value, title, message, "Delivery"), ct);
        }
    }

    private async Task<Delivery> GetDeliveryAsync(
        int id,
        CancellationToken ct)
    {
        return await db.Deliveries
            .SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Delivery not found.");
    }

    private async Task<Delivery> GetDeliveryWithProofsAsync(
        int id,
        CancellationToken ct)
    {
        return await db.Deliveries
            .Include(x => x.ProofsOfDelivery)
            .SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Delivery not found.");
    }

    private async Task EnsureRouteExistsAsync(
        int routeId,
        CancellationToken ct)
    {
        if (!await db.Routes.AnyAsync(x => x.Id == routeId, ct))
            throw new NotFoundException("Route not found.");
    }

    private async Task<bool> HasActiveDeliveryForDriverAsync(
        int driverId,
        int? excludeDeliveryId,
        CancellationToken ct)
    {
        return await db.Deliveries.AnyAsync(
            x => x.DriverId == driverId &&
                 x.Status != DeliveryStatus.Completed &&
                 x.Status != DeliveryStatus.Failed &&
                 (!excludeDeliveryId.HasValue ||
                  x.Id != excludeDeliveryId.Value),
            ct);
    }

    private async Task<bool> HasActiveDeliveryForVehicleAsync(
        int vehicleId,
        int? excludeDeliveryId,
        CancellationToken ct)
    {
        return await db.Deliveries.AnyAsync(
            x => x.VehicleId == vehicleId &&
                 x.Status != DeliveryStatus.Completed &&
                 x.Status != DeliveryStatus.Failed &&
                 (!excludeDeliveryId.HasValue ||
                  x.Id != excludeDeliveryId.Value),
            ct);
    }

    private static DeliveryResponse Map(Delivery delivery) =>
        new(
            delivery.Id,
            delivery.ShipmentId,
            delivery.DriverId,
            delivery.VehicleId,
            delivery.RouteId,
            delivery.Status,
            delivery.AssignedAtUtc,
            delivery.CompletedAtUtc,
            delivery.FailureReason,
            delivery.ProofsOfDelivery?.Any() ?? false);

    private static ProofOfDeliveryResponse Map(
        ProofOfDelivery proof) =>
        new(
            proof.Id,
            proof.DeliveryId,
            proof.ReceiverName,
            proof.SignaturePath,
            proof.PhotoPath,
            proof.Remarks,
            proof.CapturedAtUtc);
}