namespace LogiFlow.Domain.Entities
{
    public class WarehouseShipment
    {
        public int Id { get; set; }

        public int WarehouseId { get; set; }

        public int ShipmentId { get; set; }

        // Warehouse storage information
        public string? StorageLocation { get; set; }

        // Receiving and dispatch times
        public DateTime? ReceivedAtUtc { get; set; }

        public DateTime? DispatchedAtUtc { get; set; }

        // Warehouse shipment status
        public string Status { get; set; } = "Received";

        // Navigation properties
        public Warehouse Warehouse { get; set; } = null!;

        public Shipment Shipment { get; set; } = null!;
    }
}