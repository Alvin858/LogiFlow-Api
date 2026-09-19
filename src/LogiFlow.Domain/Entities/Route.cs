using LogiFlow.Domain.Enums;

namespace LogiFlow.Domain.Entities;

public class Route
{
    public int Id { get; set; }

    public string StartLocation { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;

    public decimal DistanceKm { get; set; }

    public int EstimatedDurationMinutes { get; set; }

    public RouteStatus Status { get; set; } = RouteStatus.Planned;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
}