namespace LogiFlow.Application.DTOs.Tracking;

public class TrackingDto
{
    public int Id { get; set; }

    public int ShipmentId { get; set; }

    public string TrackingNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Location { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? Remarks { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}


public class CreateTrackingDto
{
    public int ShipmentId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Location { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? Remarks { get; set; }
}


public class UpdateTrackingDto
{
    public string Status { get; set; } = string.Empty;

    public string? Location { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? Remarks { get; set; }
}