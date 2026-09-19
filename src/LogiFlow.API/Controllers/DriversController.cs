using LogiFlow.Application.DTOs.Drivers;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.API.Controllers;

[ApiController]
[Route("api/drivers")]
[Authorize(Roles = RoleNames.Admin + "," + RoleNames.LogisticsStaff)]
public class DriversController(IDriverService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyCollection<DriverResponse>>> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(ct));
    [HttpGet("{id:int}")] public async Task<ActionResult<DriverResponse>> GetById(int id, CancellationToken ct) => Ok(await service.GetByIdAsync(id, ct));
    [HttpPost]
    public async Task<ActionResult<DriverResponse>> Create(CreateDriverRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
    [HttpPut("{id:int}")] public async Task<ActionResult<DriverResponse>> Update(int id, UpdateDriverRequest request, CancellationToken ct) => Ok(await service.UpdateAsync(id, request, ct));
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id, CancellationToken ct) { await service.DeleteAsync(id, ct); return NoContent(); }
    [HttpPatch("{id:int}/availability")] public async Task<ActionResult<DriverResponse>> Availability(int id, UpdateDriverAvailabilityRequest request, CancellationToken ct) => Ok(await service.UpdateAvailabilityAsync(id, request, ct));
}
