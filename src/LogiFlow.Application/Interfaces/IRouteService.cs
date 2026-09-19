using LogiFlow.Application.DTOs.Routes;

namespace LogiFlow.Application.Interfaces;

public interface IRouteService
{
    Task<IReadOnlyCollection<RouteResponse>> GetAllAsync(
        CancellationToken ct);

    Task<RouteResponse> GetByIdAsync(
        int id,
        CancellationToken ct);

    Task<RouteResponse> CreateAsync(
        CreateRouteRequest request,
        CancellationToken ct);

    Task<RouteResponse> UpdateAsync(
        int id,
        UpdateRouteRequest request,
        CancellationToken ct);

    Task DeleteAsync(
        int id,
        CancellationToken ct);

    Task<IReadOnlyCollection<RouteStopResponse>> GetStopsAsync(
        int routeId,
        CancellationToken ct);

    Task<RouteStopResponse> AddStopAsync(
        int routeId,
        CreateRouteStopRequest request,
        CancellationToken ct);

    Task<RouteStopResponse> UpdateStopAsync(
        int routeId,
        int stopId,
        UpdateRouteStopRequest request,
        CancellationToken ct);

    Task DeleteStopAsync(
        int routeId,
        int stopId,
        CancellationToken ct);
}