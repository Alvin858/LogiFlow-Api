using LogiFlow.Application.DTOs.Shipments;
using LogiFlow.Application.Interfaces;
using LogiFlow.Application.DTOs.Notifications;
using LogiFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Application.Services;

public class ShipmentService : IShipmentService
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notifications;

    public ShipmentService(IApplicationDbContext context, INotificationService notifications)
    {
        _context = context;
        _notifications = notifications;
    }

    public async Task<List<ShipmentDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var shipments = await _context.Shipments
            .AsNoTracking()
            .Include(x => x.ShipmentItems)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return shipments.Select(MapToDto).ToList();
    }

    public async Task<ShipmentDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var shipment = await _context.Shipments
            .AsNoTracking()
            .Include(x => x.ShipmentItems)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return shipment == null ? null : MapToDto(shipment);
    }

    public async Task<ShipmentDto> CreateAsync(
        CreateShipmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var trackingNumber = await GenerateTrackingNumberAsync(
            cancellationToken);

        var shipment = new Shipment
        {
            CustomerId = dto.CustomerId,
            PickupAddressId = dto.PickupAddressId,
            DeliveryAddressId = dto.DeliveryAddressId,
            TrackingNumber = trackingNumber,
            PackageDescription = dto.PackageDescription,
            WeightKg = dto.WeightKg,
            LengthCm = dto.LengthCm,
            WidthCm = dto.WidthCm,
            HeightCm = dto.HeightCm,
            Priority = dto.Priority,
            ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
            Status = "Created",
            CreatedAtUtc = DateTime.UtcNow
        };

        foreach (var item in dto.Items)
        {
            shipment.ShipmentItems.Add(new ShipmentItem
            {
                ItemName = item.ItemName,
                Description = item.Description,
                Quantity = item.Quantity,
                WeightKg = item.WeightKg,
                LengthCm = item.LengthCm,
                WidthCm = item.WidthCm,
                HeightCm = item.HeightCm
            });
        }

        _context.Shipments.Add(shipment);

        await _context.SaveChangesAsync(cancellationToken);

        await NotifyCustomerAsync(shipment.CustomerId, "Shipment created", $"Shipment {shipment.TrackingNumber} has been created.", cancellationToken);
        return MapToDto(shipment);
    }

    public async Task<ShipmentDto?> UpdateAsync(
        int id,
        UpdateShipmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var shipment = await _context.Shipments
            .Include(x => x.ShipmentItems)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (shipment == null)
        {
            return null;
        }

        if (shipment.Status == "Cancelled")
        {
            throw new InvalidOperationException(
                "Cancelled shipments cannot be updated.");
        }

        shipment.PickupAddressId = dto.PickupAddressId;
        shipment.DeliveryAddressId = dto.DeliveryAddressId;
        shipment.PackageDescription = dto.PackageDescription;
        shipment.WeightKg = dto.WeightKg;
        shipment.LengthCm = dto.LengthCm;
        shipment.WidthCm = dto.WidthCm;
        shipment.HeightCm = dto.HeightCm;
        shipment.Priority = dto.Priority;
        shipment.ExpectedDeliveryDate = dto.ExpectedDeliveryDate;
        shipment.UpdatedAtUtc = DateTime.UtcNow;

        shipment.ShipmentItems.Clear();

        foreach (var item in dto.Items)
        {
            shipment.ShipmentItems.Add(new ShipmentItem
            {
                ItemName = item.ItemName,
                Description = item.Description,
                Quantity = item.Quantity,
                WeightKg = item.WeightKg,
                LengthCm = item.LengthCm,
                WidthCm = item.WidthCm,
                HeightCm = item.HeightCm
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(shipment);
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var shipment = await _context.Shipments
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (shipment == null)
        {
            return false;
        }

        _context.Shipments.Remove(shipment);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> CancelAsync(
        int id,
        CancelShipmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var shipment = await _context.Shipments
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (shipment == null)
        {
            return false;
        }

        if (shipment.Status == "Delivered")
        {
            throw new InvalidOperationException(
                "Delivered shipments cannot be cancelled.");
        }

        if (shipment.Status == "Cancelled")
        {
            return true;
        }

        shipment.Status = "Cancelled";
        shipment.CancelledAtUtc = DateTime.UtcNow;
        shipment.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        await NotifyCustomerAsync(shipment.CustomerId, "Shipment cancelled", $"Shipment {shipment.TrackingNumber} has been cancelled.", cancellationToken);
        return true;
    }

    public async Task<bool> UpdateStatusAsync(
        int id,
        UpdateShipmentStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        var shipment = await _context.Shipments
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (shipment == null)
        {
            return false;
        }

        if (shipment.Status == "Cancelled")
        {
            throw new InvalidOperationException(
                "Cancelled shipments cannot change status.");
        }

        shipment.Status = dto.Status;
        shipment.UpdatedAtUtc = DateTime.UtcNow;

        if (dto.Status.Equals(
                "Cancelled",
                StringComparison.OrdinalIgnoreCase))
        {
            shipment.CancelledAtUtc = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        await NotifyCustomerAsync(shipment.CustomerId, "Shipment status updated", $"Shipment {shipment.TrackingNumber} is now {shipment.Status}.", cancellationToken);
        return true;
    }

    public async Task<List<ShipmentDto>> GetCustomerShipmentsAsync(
        int customerId,
        CancellationToken cancellationToken = default)
    {
        var shipments = await _context.Shipments
            .AsNoTracking()
            .Include(x => x.ShipmentItems)
            .Where(x => x.CustomerId == customerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return shipments.Select(MapToDto).ToList();
    }


    private async Task NotifyCustomerAsync(int customerId, string title, string message, CancellationToken ct)
    {
        var userId = await _context.Customers.Where(x => x.Id == customerId).Select(x => (int?)x.UserId).SingleOrDefaultAsync(ct);
        if (userId.HasValue) await _notifications.CreateAsync(new CreateNotificationRequest(userId.Value, title, message, "Shipment"), ct);
    }

    private async Task<string> GenerateTrackingNumberAsync(
        CancellationToken cancellationToken)
    {
        string trackingNumber;

        do
        {
            trackingNumber =
                $"LF{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
        }
        while (await _context.Shipments
            .AnyAsync(
                x => x.TrackingNumber == trackingNumber,
                cancellationToken));

        return trackingNumber;
    }

    private static ShipmentDto MapToDto(Shipment shipment)
    {
        return new ShipmentDto
        {
            Id = shipment.Id,
            CustomerId = shipment.CustomerId,
            PickupAddressId = shipment.PickupAddressId,
            DeliveryAddressId = shipment.DeliveryAddressId,
            TrackingNumber = shipment.TrackingNumber,
            PackageDescription = shipment.PackageDescription,
            WeightKg = shipment.WeightKg,
            LengthCm = shipment.LengthCm,
            WidthCm = shipment.WidthCm,
            HeightCm = shipment.HeightCm,
            Priority = shipment.Priority,
            ExpectedDeliveryDate = shipment.ExpectedDeliveryDate,
            Status = shipment.Status,
            CreatedAtUtc = shipment.CreatedAtUtc,
            UpdatedAtUtc = shipment.UpdatedAtUtc,
            CancelledAtUtc = shipment.CancelledAtUtc,
            Items = shipment.ShipmentItems
                .Select(item => new ShipmentItemDto
                {
                    Id = item.Id,
                    ItemName = item.ItemName,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    WeightKg = item.WeightKg,
                    LengthCm = item.LengthCm,
                    WidthCm = item.WidthCm,
                    HeightCm = item.HeightCm
                })
                .ToList()
        };
    }
}