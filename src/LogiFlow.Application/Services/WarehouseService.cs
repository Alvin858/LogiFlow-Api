using LogiFlow.Application.DTOs.Warehouses;
using LogiFlow.Application.Interfaces;
using LogiFlow.Application.DTOs.Notifications;
using LogiFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Application.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notifications;

    public WarehouseService(IApplicationDbContext context, INotificationService notifications)
    {
        _context = context;
        _notifications = notifications;
    }

    public async Task<List<WarehouseDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var warehouses = await _context.Warehouses
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return warehouses.Select(MapToDto).ToList();
    }

    public async Task<WarehouseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await _context.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        return warehouse == null ? null : MapToDto(warehouse);
    }

    public async Task<WarehouseDto> CreateAsync(
        CreateWarehouseDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.CapacityKg <= 0)
        {
            throw new InvalidOperationException(
                "Warehouse capacity must be greater than zero.");
        }

        var warehouse = new Warehouse
        {
            Name = dto.Name,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            PostalCode = dto.PostalCode,
            CapacityKg = dto.CapacityKg,
            UsedCapacityKg = 0,
            Status = "Available",
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.Warehouses.Add(warehouse);

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(warehouse);
    }

    public async Task<WarehouseDto?> UpdateAsync(
        int id,
        UpdateWarehouseDto dto,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (warehouse == null)
        {
            return null;
        }

        if (dto.CapacityKg <= 0)
        {
            throw new InvalidOperationException(
                "Warehouse capacity must be greater than zero.");
        }

        if (dto.CapacityKg < warehouse.UsedCapacityKg)
        {
            throw new InvalidOperationException(
                "Warehouse capacity cannot be less than used capacity.");
        }

        warehouse.Name = dto.Name;
        warehouse.Address = dto.Address;
        warehouse.City = dto.City;
        warehouse.State = dto.State;
        warehouse.PostalCode = dto.PostalCode;
        warehouse.CapacityKg = dto.CapacityKg;
        warehouse.Status = dto.Status;
        warehouse.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(warehouse);
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await _context.Warehouses
            .Include(x => x.WarehouseShipments)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (warehouse == null)
        {
            return false;
        }

        if (warehouse.WarehouseShipments.Any(
                x => x.Status == "Received"))
        {
            throw new InvalidOperationException(
                "Warehouse containing received shipments cannot be deleted.");
        }

        _context.Warehouses.Remove(warehouse);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<List<WarehouseShipmentDto>> GetShipmentsAsync(
        int warehouseId,
        CancellationToken cancellationToken = default)
    {
        var warehouseExists = await _context.Warehouses
            .AnyAsync(
                x => x.Id == warehouseId,
                cancellationToken);

        if (!warehouseExists)
        {
            throw new InvalidOperationException(
                "Warehouse not found.");
        }

        var shipments = await _context.WarehouseShipments
            .AsNoTracking()
            .Include(x => x.Shipment)
            .Where(x => x.WarehouseId == warehouseId)
            .OrderByDescending(x => x.ReceivedAtUtc)
            .ToListAsync(cancellationToken);

        return shipments.Select(MapWarehouseShipmentToDto).ToList();
    }

    public async Task<WarehouseShipmentDto> ReceiveAsync(
        int warehouseId,
        ReceiveShipmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(
                x => x.Id == warehouseId,
                cancellationToken);

        if (warehouse == null)
        {
            throw new InvalidOperationException(
                "Warehouse not found.");
        }

        var shipment = await _context.Shipments
            .FirstOrDefaultAsync(
                x => x.Id == dto.ShipmentId,
                cancellationToken);

        if (shipment == null)
        {
            throw new InvalidOperationException(
                "Shipment not found.");
        }

        var alreadyReceived = await _context.WarehouseShipments
            .AnyAsync(
                x =>
                    x.WarehouseId == warehouseId &&
                    x.ShipmentId == dto.ShipmentId &&
                    x.Status == "Received",
                cancellationToken);

        if (alreadyReceived)
        {
            throw new InvalidOperationException(
                "Shipment is already received in this warehouse.");
        }

        if (warehouse.UsedCapacityKg + shipment.WeightKg >
            warehouse.CapacityKg)
        {
            throw new InvalidOperationException(
                "Warehouse does not have enough available capacity.");
        }

        var warehouseShipment = new WarehouseShipment
        {
            WarehouseId = warehouseId,
            ShipmentId = dto.ShipmentId,
            StorageLocation = dto.StorageLocation,
            ReceivedAtUtc = DateTime.UtcNow,
            Status = "Received"
        };

        _context.WarehouseShipments.Add(warehouseShipment);

        warehouse.UsedCapacityKg += shipment.WeightKg;
        warehouse.UpdatedAtUtc = DateTime.UtcNow;

        shipment.Status = "In Warehouse";
        shipment.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        await NotifyCustomerAsync(shipment.CustomerId, "Warehouse arrival", $"Shipment {shipment.TrackingNumber} arrived at {warehouse.Name}.", cancellationToken);
        warehouseShipment.Shipment = shipment;

        return MapWarehouseShipmentToDto(warehouseShipment);
    }

    public async Task<WarehouseShipmentDto> DispatchAsync(
        int warehouseId,
        DispatchShipmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(
                x => x.Id == warehouseId,
                cancellationToken);

        if (warehouse == null)
        {
            throw new InvalidOperationException(
                "Warehouse not found.");
        }

        var warehouseShipment = await _context.WarehouseShipments
            .Include(x => x.Shipment)
            .FirstOrDefaultAsync(
                x =>
                    x.WarehouseId == warehouseId &&
                    x.ShipmentId == dto.ShipmentId &&
                    x.Status == "Received",
                cancellationToken);

        if (warehouseShipment == null)
        {
            throw new InvalidOperationException(
                "Received shipment was not found in this warehouse.");
        }

        warehouseShipment.Status = "Dispatched";
        warehouseShipment.DispatchedAtUtc = DateTime.UtcNow;

        warehouse.UsedCapacityKg -=
            warehouseShipment.Shipment.WeightKg;

        if (warehouse.UsedCapacityKg < 0)
        {
            warehouse.UsedCapacityKg = 0;
        }

        warehouse.UpdatedAtUtc = DateTime.UtcNow;

        warehouseShipment.Shipment.Status = "Dispatched";
        warehouseShipment.Shipment.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return MapWarehouseShipmentToDto(warehouseShipment);
    }

    public async Task<WarehouseInventoryDto> GetInventoryAsync(
        CancellationToken cancellationToken = default)
    {
        var warehouses = await _context.Warehouses
            .AsNoTracking()
            .Include(x => x.WarehouseShipments)
                .ThenInclude(x => x.Shipment)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var inventory = new WarehouseInventoryDto
        {
            WarehouseId = 0,
            WarehouseName = "All Warehouses",
            CapacityKg = warehouses.Sum(x => x.CapacityKg),
            UsedCapacityKg = warehouses.Sum(x => x.UsedCapacityKg),
            AvailableCapacityKg =
                warehouses.Sum(x => x.CapacityKg) -
                warehouses.Sum(x => x.UsedCapacityKg)
        };

        inventory.Shipments = warehouses
            .SelectMany(x => x.WarehouseShipments)
            .Where(x => x.Status == "Received")
            .Select(MapWarehouseShipmentToDto)
            .ToList();

        return inventory;
    }

    private async Task NotifyCustomerAsync(int customerId, string title, string message, CancellationToken ct)
    {
        var userId = await _context.Customers.Where(x => x.Id == customerId).Select(x => (int?)x.UserId).SingleOrDefaultAsync(ct);
        if (userId.HasValue) await _notifications.CreateAsync(new CreateNotificationRequest(userId.Value, title, message, "Warehouse"), ct);
    }

    private static WarehouseDto MapToDto(
        Warehouse warehouse)
    {
        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Address = warehouse.Address,
            City = warehouse.City,
            State = warehouse.State,
            PostalCode = warehouse.PostalCode,
            CapacityKg = warehouse.CapacityKg,
            UsedCapacityKg = warehouse.UsedCapacityKg,
            Status = warehouse.Status,
            CreatedAtUtc = warehouse.CreatedAtUtc,
            UpdatedAtUtc = warehouse.UpdatedAtUtc
        };
    }

    private static WarehouseShipmentDto MapWarehouseShipmentToDto(
        WarehouseShipment warehouseShipment)
    {
        return new WarehouseShipmentDto
        {
            Id = warehouseShipment.Id,
            WarehouseId = warehouseShipment.WarehouseId,
            ShipmentId = warehouseShipment.ShipmentId,
            TrackingNumber =
                warehouseShipment.Shipment?.TrackingNumber
                ?? string.Empty,
            StorageLocation =
                warehouseShipment.StorageLocation,
            ReceivedAtUtc =
                warehouseShipment.ReceivedAtUtc,
            DispatchedAtUtc =
                warehouseShipment.DispatchedAtUtc,
            Status =
                warehouseShipment.Status
        };
    }
}