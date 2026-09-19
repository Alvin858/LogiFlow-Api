using LogiFlow.Domain.Enums;

namespace LogiFlow.Application.DTOs.Schedules;

public record CreateScheduleRequest(
    int ShipmentId,
    int DriverId,
    int VehicleId,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc);

public record UpdateScheduleRequest(
    int ShipmentId,
    int DriverId,
    int VehicleId,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc);

public record RescheduleRequest(
    DateTime StartTimeUtc,
    DateTime EndTimeUtc);

public record ScheduleResponse(
    int Id,
    int ShipmentId,
    int DriverId,
    int VehicleId,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc,
    ScheduleStatus Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);