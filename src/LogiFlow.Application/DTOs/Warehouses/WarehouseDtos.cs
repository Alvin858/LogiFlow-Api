namespace LogiFlow.Application.DTOs.Warehouses;

public class WarehouseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public decimal CapacityKg { get; set; }

    public decimal UsedCapacityKg { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}


public class CreateWarehouseDto
{
    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public decimal CapacityKg { get; set; }
}


public class UpdateWarehouseDto
{
    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public decimal CapacityKg { get; set; }

    public string Status { get; set; } = "Available";
}


public class WarehouseShipmentDto
{
    public int Id { get; set; }

    public int WarehouseId { get; set; }

    public int ShipmentId { get; set; }

    public string TrackingNumber { get; set; } = string.Empty;

    public string? StorageLocation { get; set; }

    public DateTime? ReceivedAtUtc { get; set; }

    public DateTime? DispatchedAtUtc { get; set; }

    public string Status { get; set; } = string.Empty;
}


public class ReceiveShipmentDto
{
    public int ShipmentId { get; set; }

    public string? StorageLocation { get; set; }
}


public class DispatchShipmentDto
{
    public int ShipmentId { get; set; }
}


public class WarehouseInventoryDto
{
    public int WarehouseId { get; set; }

    public string WarehouseName { get; set; } = string.Empty;

    public decimal CapacityKg { get; set; }

    public decimal UsedCapacityKg { get; set; }

    public decimal AvailableCapacityKg { get; set; }

    public List<WarehouseShipmentDto> Shipments { get; set; } = new();
}