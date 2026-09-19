namespace LogiFlow.Domain.Entities
{
    public class ShipmentTracking
    {
        public int Id { get; set; }

        public int ShipmentId { get; set; }

        // Tracking status
        public string Status { get; set; } = string.Empty;

        // Current location
        public string? Location { get; set; }

        // GPS coordinates
        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        // Additional information
        public string? Remarks { get; set; }

        // Tracking update time
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Shipment Shipment { get; set; } = null!;
    }
}