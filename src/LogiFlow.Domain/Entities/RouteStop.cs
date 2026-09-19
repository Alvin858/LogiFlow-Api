namespace LogiFlow.Domain.Entities;

public class RouteStop
{
    public int Id { get; set; }

    public int RouteId { get; set; }

    public int StopOrder { get; set; }

    public string Address { get; set; } = string.Empty;

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public Route Route { get; set; } = null!;
}