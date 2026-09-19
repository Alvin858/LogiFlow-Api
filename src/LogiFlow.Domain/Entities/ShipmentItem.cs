namespace LogiFlow.Domain.Entities
{
    public class ShipmentItem
    {
        public int Id { get; set; }

        public int ShipmentId { get; set; }

        // Item details
        public string ItemName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Quantity { get; set; }

        // Item package information
        public decimal WeightKg { get; set; }

        public decimal LengthCm { get; set; }

        public decimal WidthCm { get; set; }

        public decimal HeightCm { get; set; }

        // Navigation property
        public Shipment Shipment { get; set; } = null!;
    }
}