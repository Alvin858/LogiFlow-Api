using LogiFlow.Domain.Enums;

namespace LogiFlow.Application.DTOs.DeliveryDtos;

public record CreateDeliveryRequest(
    int ShipmentId,
    int DriverId,
    int VehicleId,
    int RouteId);

public record UpdateDeliveryRequest(
    int ShipmentId,
    int DriverId,
    int VehicleId,
    int RouteId);

public record DeliveryResponse(
    int Id,
    int ShipmentId,
    int DriverId,
    int VehicleId,
    int RouteId,
    DeliveryStatus Status,
    DateTime AssignedAtUtc,
    DateTime? CompletedAtUtc,
    string? FailureReason,
    bool HasProofOfDelivery);

public record CompleteDeliveryRequest;

public record FailDeliveryRequest(
    string FailureReason);

public record CreateProofOfDeliveryRequest(
    string ReceiverName,
    string SignaturePath,
    string PhotoPath,
    string? Remarks);

public record ProofOfDeliveryResponse(
    int Id,
    int DeliveryId,
    string ReceiverName,
    string SignaturePath,
    string PhotoPath,
    string? Remarks,
    DateTime CapturedAtUtc);