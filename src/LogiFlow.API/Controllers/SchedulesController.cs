using LogiFlow.Application.DTOs.Schedules;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.API.Controllers;

[ApiController]
[Route("api/schedules")]
[Authorize(Roles = RoleNames.Admin + "," + RoleNames.LogisticsStaff)]
public class SchedulesController(IScheduleService service)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ScheduleResponse>>>
        GetAll(CancellationToken ct) =>
        Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ScheduleResponse>>
        GetById(int id, CancellationToken ct) =>
        Ok(await service.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<ScheduleResponse>>
        Create(
            CreateScheduleRequest request,
            CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ScheduleResponse>>
        Update(
            int id,
            UpdateScheduleRequest request,
            CancellationToken ct) =>
        Ok(await service.UpdateAsync(id, request, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
        Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPatch("{id:int}/reschedule")]
    public async Task<ActionResult<ScheduleResponse>>
        Reschedule(
            int id,
            RescheduleRequest request,
            CancellationToken ct) =>
        Ok(await service.RescheduleAsync(
            id,
            request,
            ct));

    [HttpPatch("{id:int}/cancel")]
    public async Task<ActionResult<ScheduleResponse>>
        Cancel(int id, CancellationToken ct) =>
        Ok(await service.CancelAsync(id, ct));
}