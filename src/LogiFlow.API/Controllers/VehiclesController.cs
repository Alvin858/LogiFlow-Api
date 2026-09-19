using LogiFlow.Application.DTOs.Vehicles;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.API.Controllers;

[ApiController]
[Route("api/vehicles")]
[Authorize(Roles = RoleNames.Admin + "," + RoleNames.LogisticsStaff)]
public class VehiclesController(IVehicleService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyCollection<VehicleResponse>>> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));
    [HttpGet("{id:int}")] public async Task<ActionResult<VehicleResponse>> GetById(int id, CancellationToken ct) => Ok(await service.GetByIdAsync(id, ct));
    [HttpPost]
    public async Task<ActionResult<VehicleResponse>> Create(CreateVehicleRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
    [HttpPut("{id:int}")] public async Task<ActionResult<VehicleResponse>> Update(int id, UpdateVehicleRequest request, CancellationToken ct) => Ok(await service.UpdateAsync(id, request, ct));
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id, CancellationToken ct) { await service.DeleteAsync(id, ct); return NoContent(); }
    [HttpPatch("{id:int}/status")] public async Task<ActionResult<VehicleResponse>> Status(int id, UpdateVehicleStatusRequest request, CancellationToken ct) => Ok(await service.UpdateStatusAsync(id, request, ct));
}
