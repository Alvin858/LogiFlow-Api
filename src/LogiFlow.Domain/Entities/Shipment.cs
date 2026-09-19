namespace LogiFlow.Domain.Entities
{
    public class Shipment
    {
        public int Id { get; set; }

        // Customer who owns this shipment
        public int CustomerId { get; set; }

        // Pickup and delivery addresses
        public int PickupAddressId { get; set; }
        public int DeliveryAddressId { get; set; }

        // Shipment identification
        public string TrackingNumber { get; set; } = string.Empty;

        // Package information
        public string PackageDescription { get; set; } = string.Empty;

        public decimal WeightKg { get; set; }

        public decimal LengthCm { get; set; }
        public decimal WidthCm { get; set; }
        public decimal HeightCm { get; set; }

        // Shipment priority
        public string Priority { get; set; } = "Normal";

        // Expected delivery
        public DateTime ExpectedDeliveryDate { get; set; }

        // Current shipment status
        public string Status { get; set; } = "Created";

        // Audit fields
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAtUtc { get; set; }

        public DateTime? CancelledAtUtc { get; set; }


        // Navigation properties

        public Customer? Customer { get; set; }

        public CustomerAddress? PickupAddress { get; set; }

        public CustomerAddress? DeliveryAddress { get; set; }

        public ICollection<ShipmentItem> ShipmentItems { get; set; }
            = new List<ShipmentItem>();

        public ICollection<ShipmentTracking> TrackingHistory { get; set; }
            = new List<ShipmentTracking>();

        public ICollection<WarehouseShipment> WarehouseShipments { get; set; }
            = new List<WarehouseShipment>();
    }
}