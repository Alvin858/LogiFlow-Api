using LogiFlow.Domain.Enums;

namespace LogiFlow.Application.DTOs.Routes;

public record CreateRouteRequest(
    string StartLocation,
    string Destination,
    decimal DistanceKm,
    int EstimatedDurationMinutes,
    RouteStatus Status);

public record UpdateRouteRequest(
    string StartLocation,
    string Destination,
    decimal DistanceKm,
    int EstimatedDurationMinutes,
    RouteStatus Status);

public record RouteResponse(
    int Id,
    string StartLocation,
    string Destination,
    decimal DistanceKm,
    int EstimatedDurationMinutes,
    RouteStatus Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public record CreateRouteStopRequest(
    int StopOrder,
    string Address,
    decimal Latitude,
    decimal Longitude);

public record UpdateRouteStopRequest(
    int StopOrder,
    string Address,
    decimal Latitude,
    decimal Longitude);

public record RouteStopResponse(
    int Id,
    int RouteId,
    int StopOrder,
    string Address,
    decimal Latitude,
    decimal Longitude);