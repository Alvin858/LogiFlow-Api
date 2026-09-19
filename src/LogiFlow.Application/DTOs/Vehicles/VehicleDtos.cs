using LogiFlow.Domain.Enums;

namespace LogiFlow.Application.DTOs.Vehicles;

public record CreateVehicleRequest(string VehicleType, string RegistrationNumber, decimal CapacityKg, VehicleStatus Status, DateTime? InsuranceExpiryDate, DateTime? FitnessExpiryDate);
public record UpdateVehicleRequest(string VehicleType, string RegistrationNumber, decimal CapacityKg, VehicleStatus Status, DateTime? InsuranceExpiryDate, DateTime? FitnessExpiryDate);
public record UpdateVehicleStatusRequest(VehicleStatus Status);
public record VehicleResponse(int Id, string VehicleType, string RegistrationNumber, decimal CapacityKg, VehicleStatus Status, DateTime? InsuranceExpiryDate, DateTime? FitnessExpiryDate, DateTime CreatedAtUtc);
