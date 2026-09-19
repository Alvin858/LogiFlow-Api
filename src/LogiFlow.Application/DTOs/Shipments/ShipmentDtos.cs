namespace LogiFlow.Application.DTOs.Shipments;

public class ShipmentDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int PickupAddressId { get; set; }
    public int DeliveryAddressId { get; set; }

    public string TrackingNumber { get; set; } = string.Empty;
    public string PackageDescription { get; set; } = string.Empty;

    public decimal WeightKg { get; set; }
    public decimal LengthCm { get; set; }
    public decimal WidthCm { get; set; }
    public decimal HeightCm { get; set; }

    public string Priority { get; set; } = "Normal";
    public DateTime ExpectedDeliveryDate { get; set; }
    public string Status { get; set; } = "Created";

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }

    public List<ShipmentItemDto> Items { get; set; } = new();
}

public class ShipmentItemDto
{
    public int Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; }

    public decimal WeightKg { get; set; }
    public decimal LengthCm { get; set; }
    public decimal WidthCm { get; set; }
    public decimal HeightCm { get; set; }
}

public class CreateShipmentDto
{
    public int CustomerId { get; set; }
    public int PickupAddressId { get; set; }
    public int DeliveryAddressId { get; set; }

    public string PackageDescription { get; set; } = string.Empty;

    public decimal WeightKg { get; set; }
    public decimal LengthCm { get; set; }
    public decimal WidthCm { get; set; }
    public decimal HeightCm { get; set; }

    public string Priority { get; set; } = "Normal";
    public DateTime ExpectedDeliveryDate { get; set; }

    public List<CreateShipmentItemDto> Items { get; set; } = new();
}

public class CreateShipmentItemDto
{
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; }

    public decimal WeightKg { get; set; }
    public decimal LengthCm { get; set; }
    public decimal WidthCm { get; set; }
    public decimal HeightCm { get; set; }
}

public class UpdateShipmentDto
{
    public int PickupAddressId { get; set; }
    public int DeliveryAddressId { get; set; }

    public string PackageDescription { get; set; } = string.Empty;

    public decimal WeightKg { get; set; }
    public decimal LengthCm { get; set; }
    public decimal WidthCm { get; set; }
    public decimal HeightCm { get; set; }

    public string Priority { get; set; } = "Normal";
    public DateTime ExpectedDeliveryDate { get; set; }

    public List<CreateShipmentItemDto> Items { get; set; } = new();
}

public class UpdateShipmentStatusDto
{
    public string Status { get; set; } = string.Empty;
}

public class CancelShipmentDto
{
    public string? Reason { get; set; }
}