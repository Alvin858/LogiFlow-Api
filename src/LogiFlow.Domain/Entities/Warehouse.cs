namespace LogiFlow.Domain.Entities
{
    public class Warehouse
    {
        public int Id { get; set; }

        // Warehouse information
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;

        // Warehouse capacity
        public decimal CapacityKg { get; set; }

        public decimal UsedCapacityKg { get; set; }

        // Warehouse status
        public string Status { get; set; } = "Available";

        // Audit fields
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAtUtc { get; set; }

        // Navigation property
        public ICollection<WarehouseShipment> WarehouseShipments { get; set; }
            = new List<WarehouseShipment>();
    }
}