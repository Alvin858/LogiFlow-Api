using LogiFlow.Domain.Enums;

namespace LogiFlow.Domain.Entities;

public class Delivery
{
    public int Id { get; set; }

    public int ShipmentId { get; set; }

    public int DriverId { get; set; }

    public int VehicleId { get; set; }

    public int RouteId { get; set; }

    public DeliveryStatus Status { get; set; } = DeliveryStatus.Assigned;

    public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAtUtc { get; set; }

    public string? FailureReason { get; set; }

    public ICollection<ProofOfDelivery> ProofsOfDelivery { get; set; } =
        new List<ProofOfDelivery>();
}