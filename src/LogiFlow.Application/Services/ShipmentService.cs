using LogiFlow.Application.DTOs.Notifications;
using LogiFlow.Application.DTOs.Shipments;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Application.Services;

public class ShipmentService : IShipmentService
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notifications;

    public ShipmentService(
        IApplicationDbContext context,
        INotificationService notifications)
    {
        _context = context;
        _notifications = notifications;
    }

    // =========================================================
    // GET ALL SHIPMENTS
    // =========================================================

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

    // =========================================================
    // GET SHIPMENT BY ID
    // =========================================================

    public async Task<ShipmentDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var shipment = await _context.Shipments
            .AsNoTracking()
            .Include(x => x.ShipmentItems)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        return shipment == null
            ? null
            : MapToDto(shipment);
    }

    // =========================================================
    // CREATE SHIPMENT
    // =========================================================

    public async Task<ShipmentDto> CreateAsync(
        CreateShipmentDto dto,
        CancellationToken cancellationToken = default)
    {
        // Generate unique tracking number
        var trackingNumber =
            await GenerateTrackingNumberAsync(cancellationToken);

        // Create shipment
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

        // =====================================================
        // ADD SHIPMENT ITEMS
        // =====================================================

        foreach (var item in dto.Items)
        {
            shipment.ShipmentItems.Add(
                new ShipmentItem
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

        // =====================================================
        // SAVE SHIPMENT FIRST
        // =====================================================

        _context.Shipments.Add(shipment);

        await _context.SaveChangesAsync(cancellationToken);

        // =====================================================
        // CREATE INITIAL TRACKING RECORD
        // =====================================================

        var tracking = new ShipmentTracking
        {
            ShipmentId = shipment.Id,

            Status = "Created",

            Location = null,

            Latitude = null,

            Longitude = null,

            Remarks = "Shipment created successfully.",

            UpdatedAtUtc = DateTime.UtcNow
        };

        _context.ShipmentTracking.Add(tracking);

        await _context.SaveChangesAsync(cancellationToken);

        // =====================================================
        // NOTIFY CUSTOMER
        // =====================================================

        await NotifyCustomerAsync(
            shipment.CustomerId,
            "Shipment created",
            $"Shipment {shipment.TrackingNumber} has been created.",
            cancellationToken);

        return MapToDto(shipment);
    }

    // =========================================================
    // UPDATE SHIPMENT
    // =========================================================

    public async Task<ShipmentDto?> UpdateAsync(
        int id,
        UpdateShipmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var shipment = await _context.Shipments
            .Include(x => x.ShipmentItems)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (shipment == null)
        {
            return null;
        }

        if (shipment.Status == "Cancelled")
        {
            throw new InvalidOperationException(
                "Cancelled shipments cannot be updated.");
        }

        // =====================================================
        // UPDATE SHIPMENT DETAILS
        // =====================================================

        shipment.PickupAddressId =
            dto.PickupAddressId;

        shipment.DeliveryAddressId =
            dto.DeliveryAddressId;

        shipment.PackageDescription =
            dto.PackageDescription;

        shipment.WeightKg =
            dto.WeightKg;

        shipment.LengthCm =
            dto.LengthCm;

        shipment.WidthCm =
            dto.WidthCm;

        shipment.HeightCm =
            dto.HeightCm;

        shipment.Priority =
            dto.Priority;

        shipment.ExpectedDeliveryDate =
            dto.ExpectedDeliveryDate;

        shipment.UpdatedAtUtc =
            DateTime.UtcNow;

        // =====================================================
        // UPDATE SHIPMENT ITEMS
        // =====================================================

        shipment.ShipmentItems.Clear();

        foreach (var item in dto.Items)
        {
            shipment.ShipmentItems.Add(
                new ShipmentItem
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

        await _context.SaveChangesAsync(
            cancellationToken);

        return MapToDto(shipment);
    }

    // =========================================================
    // DELETE SHIPMENT
    // =========================================================

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var shipment = await _context.Shipments
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (shipment == null)
        {
            return false;
        }

        _context.Shipments.Remove(shipment);

        await _context.SaveChangesAsync(
            cancellationToken);

        return true;
    }

    // =========================================================
    // CANCEL SHIPMENT
    // =========================================================

    public async Task<bool> CancelAsync(
        int id,
        CancelShipmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var shipment = await _context.Shipments
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

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

        // =====================================================
        // UPDATE SHIPMENT
        // =====================================================

        shipment.Status = "Cancelled";

        shipment.CancelledAtUtc =
            DateTime.UtcNow;

        shipment.UpdatedAtUtc =
            DateTime.UtcNow;

        // =====================================================
        // CREATE TRACKING EVENT
        // =====================================================

        var tracking = new ShipmentTracking
        {
            ShipmentId = shipment.Id,

            Status = "Cancelled",

            Location = null,

            Latitude = null,

            Longitude = null,

            Remarks = string.IsNullOrWhiteSpace(dto.Reason)
                ? "Shipment cancelled."
                : dto.Reason,

            UpdatedAtUtc = DateTime.UtcNow
        };

        _context.ShipmentTracking.Add(tracking);

        await _context.SaveChangesAsync(
            cancellationToken);

        // =====================================================
        // NOTIFY CUSTOMER
        // =====================================================

        await NotifyCustomerAsync(
            shipment.CustomerId,
            "Shipment cancelled",
            $"Shipment {shipment.TrackingNumber} has been cancelled.",
            cancellationToken);

        return true;
    }

    // =========================================================
    // UPDATE SHIPMENT STATUS
    // =========================================================

    public async Task<bool> UpdateStatusAsync(
        int id,
        UpdateShipmentStatusDto dto,
        CancellationToken cancellationToken = default)
    {
        var shipment = await _context.Shipments
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (shipment == null)
        {
            return false;
        }

        if (shipment.Status == "Cancelled")
        {
            throw new InvalidOperationException(
                "Cancelled shipments cannot change status.");
        }

        // =====================================================
        // UPDATE SHIPMENT STATUS
        // =====================================================

        shipment.Status = dto.Status;

        shipment.UpdatedAtUtc =
            DateTime.UtcNow;

        if (dto.Status.Equals(
                "Cancelled",
                StringComparison.OrdinalIgnoreCase))
        {
            shipment.CancelledAtUtc =
                DateTime.UtcNow;
        }

        // =====================================================
        // CREATE TRACKING HISTORY
        // =====================================================

        var tracking = new ShipmentTracking
        {
            ShipmentId = shipment.Id,

            Status = dto.Status,

            Location = null,

            Latitude = null,

            Longitude = null,

            Remarks =
                $"Shipment status changed to {dto.Status}.",

            UpdatedAtUtc = DateTime.UtcNow
        };

        _context.ShipmentTracking.Add(tracking);

        await _context.SaveChangesAsync(
            cancellationToken);

        // =====================================================
        // NOTIFY CUSTOMER
        // =====================================================

        await NotifyCustomerAsync(
            shipment.CustomerId,
            "Shipment status updated",
            $"Shipment {shipment.TrackingNumber} is now {shipment.Status}.",
            cancellationToken);

        return true;
    }

    // =========================================================
    // GET CUSTOMER SHIPMENTS
    // =========================================================

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

        return shipments
            .Select(MapToDto)
            .ToList();
    }

    // =========================================================
    // CUSTOMER NOTIFICATION
    // =========================================================

    private async Task NotifyCustomerAsync(
        int customerId,
        string title,
        string message,
        CancellationToken ct)
    {
        var userId =
            await _context.Customers
                .Where(x => x.Id == customerId)
                .Select(x => (int?)x.UserId)
                .SingleOrDefaultAsync(ct);

        if (userId.HasValue)
        {
            await _notifications.CreateAsync(
                new CreateNotificationRequest(
                    userId.Value,
                    title,
                    message,
                    "Shipment"),
                ct);
        }
    }

    // =========================================================
    // GENERATE TRACKING NUMBER
    // =========================================================

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

    // =========================================================
    // MAP ENTITY TO DTO
    // =========================================================

    private static ShipmentDto MapToDto(
        Shipment shipment)
    {
        return new ShipmentDto
        {
            Id = shipment.Id,

            CustomerId =
                shipment.CustomerId,

            PickupAddressId =
                shipment.PickupAddressId,

            DeliveryAddressId =
                shipment.DeliveryAddressId,

            TrackingNumber =
                shipment.TrackingNumber,

            PackageDescription =
                shipment.PackageDescription,

            WeightKg =
                shipment.WeightKg,

            LengthCm =
                shipment.LengthCm,

            WidthCm =
                shipment.WidthCm,

            HeightCm =
                shipment.HeightCm,

            Priority =
                shipment.Priority,

            ExpectedDeliveryDate =
                shipment.ExpectedDeliveryDate,

            Status =
                shipment.Status,

            CreatedAtUtc =
                shipment.CreatedAtUtc,

            UpdatedAtUtc =
                shipment.UpdatedAtUtc,

            CancelledAtUtc =
                shipment.CancelledAtUtc,

            Items = shipment.ShipmentItems
                .Select(item =>
                    new ShipmentItemDto
                    {
                        Id = item.Id,

                        ItemName =
                            item.ItemName,

                        Description =
                            item.Description,

                        Quantity =
                            item.Quantity,

                        WeightKg =
                            item.WeightKg,

                        LengthCm =
                            item.LengthCm,

                        WidthCm =
                            item.WidthCm,

                        HeightCm =
                            item.HeightCm
                    })
                .ToList()
        };
    }
}