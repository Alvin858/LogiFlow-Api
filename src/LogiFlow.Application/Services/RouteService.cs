using LogiFlow.Application.DTOs.Routes;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Entities;
using LogiFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Application.Services;

public class RouteService(IApplicationDbContext db) : IRouteService
{
    public async Task<IReadOnlyCollection<RouteResponse>> GetAllAsync(
        CancellationToken ct)
    {
        return await db.Routes
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new RouteResponse(
                x.Id,
                x.StartLocation,
                x.Destination,
                x.DistanceKm,
                x.EstimatedDurationMinutes,
                x.Status,
                x.CreatedAtUtc,
                x.UpdatedAtUtc))
            .ToListAsync(ct);
    }

    public async Task<RouteResponse> GetByIdAsync(
        int id,
        CancellationToken ct)
    {
        var route = await db.Routes
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Route not found.");

        return Map(route);
    }

    public async Task<RouteResponse> CreateAsync(
        CreateRouteRequest request,
        CancellationToken ct)
    {
        if (request.Status == RouteStatus.Active &&
            !await HasStopsAsync(0, ct))
        {
            throw new BadRequestException(
                "A route must have at least one stop before it can be active.");
        }

        var route = new Route
        {
            StartLocation = request.StartLocation.Trim(),
            Destination = request.Destination.Trim(),
            DistanceKm = request.DistanceKm,
            EstimatedDurationMinutes = request.EstimatedDurationMinutes,
            Status = request.Status
        };

        if (route.Status == RouteStatus.Active)
        {
            route.Status = RouteStatus.Planned;
        }

        db.Routes.Add(route);

        await db.SaveChangesAsync(ct);

        return Map(route);
    }

    public async Task<RouteResponse> UpdateAsync(
        int id,
        UpdateRouteRequest request,
        CancellationToken ct)
    {
        var route = await db.Routes
            .SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Route not found.");

        if (request.Status == RouteStatus.Active &&
            !await HasStopsAsync(id, ct))
        {
            throw new BadRequestException(
                "A route must have at least one stop before it can be active.");
        }

        route.StartLocation = request.StartLocation.Trim();
        route.Destination = request.Destination.Trim();
        route.DistanceKm = request.DistanceKm;
        route.EstimatedDurationMinutes = request.EstimatedDurationMinutes;
        route.Status = request.Status;
        route.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);

        return Map(route);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken ct)
    {
        var route = await db.Routes
            .SingleOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Route not found.");

        db.Routes.Remove(route);

        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyCollection<RouteStopResponse>> GetStopsAsync(
        int routeId,
        CancellationToken ct)
    {
        await EnsureRouteExistsAsync(routeId, ct);

        return await db.RouteStops
            .AsNoTracking()
            .Where(x => x.RouteId == routeId)
            .OrderBy(x => x.StopOrder)
            .Select(x => new RouteStopResponse(
                x.Id,
                x.RouteId,
                x.StopOrder,
                x.Address,
                x.Latitude,
                x.Longitude))
            .ToListAsync(ct);
    }

    public async Task<RouteStopResponse> AddStopAsync(
        int routeId,
        CreateRouteStopRequest request,
        CancellationToken ct)
    {
        await EnsureRouteExistsAsync(routeId, ct);

        if (await db.RouteStops.AnyAsync(
                x => x.RouteId == routeId &&
                     x.StopOrder == request.StopOrder,
                ct))
        {
            throw new ConflictException(
                "Stop order must be unique within a route.");
        }

        var existingOrders = await db.RouteStops
            .Where(x => x.RouteId == routeId)
            .Select(x => x.StopOrder)
            .ToListAsync(ct);

        var expectedOrder = existingOrders.Count + 1;

        if (request.StopOrder != expectedOrder)
        {
            throw new BadRequestException(
                $"Stop order must be sequential. The next stop order is {expectedOrder}.");
        }

        var stop = new RouteStop
        {
            RouteId = routeId,
            StopOrder = request.StopOrder,
            Address = request.Address.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        db.RouteStops.Add(stop);

        await db.SaveChangesAsync(ct);

        return Map(stop);
    }

    public async Task<RouteStopResponse> UpdateStopAsync(
        int routeId,
        int stopId,
        UpdateRouteStopRequest request,
        CancellationToken ct)
    {
        await EnsureRouteExistsAsync(routeId, ct);

        var stop = await db.RouteStops
            .SingleOrDefaultAsync(
                x => x.Id == stopId && x.RouteId == routeId,
                ct)
            ?? throw new NotFoundException("Route stop not found.");

        if (await db.RouteStops.AnyAsync(
                x => x.RouteId == routeId &&
                     x.Id != stopId &&
                     x.StopOrder == request.StopOrder,
                ct))
        {
            throw new ConflictException(
                "Stop order must be unique within a route.");
        }

        var stopOrders = await db.RouteStops
            .Where(x => x.RouteId == routeId && x.Id != stopId)
            .Select(x => x.StopOrder)
            .OrderBy(x => x)
            .ToListAsync(ct);

        var expectedOrders = Enumerable
            .Range(1, stopOrders.Count + 1)
            .ToHashSet();

        if (!expectedOrders.Contains(request.StopOrder))
        {
            throw new BadRequestException(
                "Stop order must be sequential within the route.");
        }

        stop.StopOrder = request.StopOrder;
        stop.Address = request.Address.Trim();
        stop.Latitude = request.Latitude;
        stop.Longitude = request.Longitude;

        await db.SaveChangesAsync(ct);

        return Map(stop);
    }

    public async Task DeleteStopAsync(
        int routeId,
        int stopId,
        CancellationToken ct)
    {
        await EnsureRouteExistsAsync(routeId, ct);

        var stop = await db.RouteStops
            .SingleOrDefaultAsync(
                x => x.Id == stopId && x.RouteId == routeId,
                ct)
            ?? throw new NotFoundException("Route stop not found.");

        db.RouteStops.Remove(stop);

        await db.SaveChangesAsync(ct);

        await ResequenceStopsAsync(routeId, ct);
    }

    private async Task<bool> HasStopsAsync(
        int routeId,
        CancellationToken ct)
    {
        if (routeId == 0)
            return false;

        return await db.RouteStops
            .AnyAsync(x => x.RouteId == routeId, ct);
    }

    private async Task EnsureRouteExistsAsync(
        int routeId,
        CancellationToken ct)
    {
        if (!await db.Routes.AnyAsync(x => x.Id == routeId, ct))
            throw new NotFoundException("Route not found.");
    }

    private async Task ResequenceStopsAsync(
        int routeId,
        CancellationToken ct)
    {
        var stops = await db.RouteStops
            .Where(x => x.RouteId == routeId)
            .OrderBy(x => x.StopOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(ct);

        for (var i = 0; i < stops.Count; i++)
        {
            stops[i].StopOrder = i + 1;
        }

        await db.SaveChangesAsync(ct);
    }

    private static RouteResponse Map(Route route) =>
        new(
            route.Id,
            route.StartLocation,
            route.Destination,
            route.DistanceKm,
            route.EstimatedDurationMinutes,
            route.Status,
            route.CreatedAtUtc,
            route.UpdatedAtUtc);

    private static RouteStopResponse Map(RouteStop stop) =>
        new(
            stop.Id,
            stop.RouteId,
            stop.StopOrder,
            stop.Address,
            stop.Latitude,
            stop.Longitude);
}