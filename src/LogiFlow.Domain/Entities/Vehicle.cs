using LogiFlow.Domain.Enums;

namespace LogiFlow.Domain.Entities;

public class Vehicle
{
    public int Id { get; set; }
    public string VehicleType { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public decimal CapacityKg { get; set; }
    public VehicleStatus Status { get; set; } = VehicleStatus.Available;
    public DateTime? InsuranceExpiryDate { get; set; }
    public DateTime? FitnessExpiryDate { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
