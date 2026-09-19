using LogiFlow.Application.DTOs.Routes;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.API.Controllers;

[ApiController]
[Route("api/routes")]
[Authorize(Roles = RoleNames.Admin + "," + RoleNames.LogisticsStaff)]
public class RoutesController(IRouteService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<RouteResponse>>>
        GetAll(CancellationToken ct) =>
        Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RouteResponse>>
        GetById(int id, CancellationToken ct) =>
        Ok(await service.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<RouteResponse>>
        Create(CreateRouteRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RouteResponse>>
        Update(
            int id,
            UpdateRouteRequest request,
            CancellationToken ct) =>
        Ok(await service.UpdateAsync(id, request, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
        Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpGet("{id:int}/stops")]
    public async Task<ActionResult<IReadOnlyCollection<RouteStopResponse>>>
        GetStops(int id, CancellationToken ct) =>
        Ok(await service.GetStopsAsync(id, ct));

    [HttpPost("{id:int}/stops")]
    public async Task<ActionResult<RouteStopResponse>>
        AddStop(
            int id,
            CreateRouteStopRequest request,
            CancellationToken ct)
    {
        var created = await service.AddStopAsync(id, request, ct);

        return CreatedAtAction(
            nameof(GetStops),
            new { id },
            created);
    }

    [HttpPut("{id:int}/stops/{stopId:int}")]
    public async Task<ActionResult<RouteStopResponse>>
        UpdateStop(
            int id,
            int stopId,
            UpdateRouteStopRequest request,
            CancellationToken ct) =>
        Ok(await service.UpdateStopAsync(
            id,
            stopId,
            request,
            ct));

    [HttpDelete("{id:int}/stops/{stopId:int}")]
    public async Task<IActionResult>
        DeleteStop(
            int id,
            int stopId,
            CancellationToken ct)
    {
        await service.DeleteStopAsync(id, stopId, ct);
        return NoContent();
    }
}