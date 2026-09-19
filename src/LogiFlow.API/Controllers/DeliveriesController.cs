
using LogiFlow.Application.DTOs.DeliveryDtos;
using LogiFlow.Application.DTOs.Schedules;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LogiFlow.Application.DTOs.Schedules;
using LogiFlow.Application.Interfaces;

namespace LogiFlow.API.Controllers;

[ApiController]
[Route("api/deliveries")]
[Authorize(
    Roles = RoleNames.Admin + "," +
            RoleNames.LogisticsStaff + "," +
            RoleNames.Driver)]
public class DeliveriesController(IDeliveryService service)
    : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.LogisticsStaff)]
    public async Task<ActionResult<IReadOnlyCollection<DeliveryResponse>>>
        GetAll(CancellationToken ct) =>
        Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DeliveryResponse>>
        GetById(int id, CancellationToken ct) =>
        Ok(await service.GetByIdAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.LogisticsStaff)]
    public async Task<ActionResult<DeliveryResponse>>
        Create(CreateDeliveryRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.LogisticsStaff)]
    public async Task<ActionResult<DeliveryResponse>>
        Update(
            int id,
            UpdateDeliveryRequest request,
            CancellationToken ct) =>
        Ok(await service.UpdateAsync(id, request, ct));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.LogisticsStaff)]
    public async Task<IActionResult>
        Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPatch("{id:int}/pickup")]
    public async Task<ActionResult<DeliveryResponse>>
        Pickup(int id, CancellationToken ct) =>
        Ok(await service.PickupAsync(id, ct));

    [HttpPatch("{id:int}/out-for-delivery")]
    public async Task<ActionResult<DeliveryResponse>>
        OutForDelivery(int id, CancellationToken ct) =>
        Ok(await service.OutForDeliveryAsync(id, ct));

    [HttpPatch("{id:int}/complete")]
    public async Task<ActionResult<DeliveryResponse>>
        Complete(int id, CancellationToken ct) =>
        Ok(await service.CompleteAsync(id, ct));

    [HttpPatch("{id:int}/failed")]
    public async Task<ActionResult<DeliveryResponse>>
        Failed(
            int id,
            FailDeliveryRequest request,
            CancellationToken ct) =>
        Ok(await service.FailAsync(id, request, ct));

    [HttpPost("{id:int}/proof")]
    public async Task<ActionResult<ProofOfDeliveryResponse>>
        AddProof(
            int id,
            CreateProofOfDeliveryRequest request,
            CancellationToken ct)
    {
        var created = await service.AddProofAsync(
            id,
            request,
            ct);

        return Ok(created);
    }
    [HttpGet("{id:int}/schedule")]
    [Authorize(
    Roles = RoleNames.Admin + "," +
            RoleNames.LogisticsStaff + "," +
            RoleNames.Driver)]
    public async Task<ActionResult<IReadOnlyCollection<ScheduleResponse>>>
    GetSchedule(
        int id,
        [FromServices] IScheduleService scheduleService,
        CancellationToken ct) =>
    Ok(await scheduleService.GetDriverScheduleAsync(id, ct));
}