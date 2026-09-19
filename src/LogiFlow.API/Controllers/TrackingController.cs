using LogiFlow.Application.DTOs.Tracking;
using LogiFlow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.API.Controllers;

[ApiController]
[Route("api/tracking")]
[Authorize]
public class TrackingController : ControllerBase
{
    private readonly ITrackingService _trackingService;

    public TrackingController(ITrackingService trackingService)
    {
        _trackingService = trackingService;
    }

    // GET: api/tracking/{trackingNumber}
    [HttpGet("{trackingNumber}")]
    public async Task<ActionResult<TrackingDto>> GetByTrackingNumber(
        string trackingNumber,
        CancellationToken cancellationToken)
    {
        var tracking = await _trackingService
            .GetByTrackingNumberAsync(
                trackingNumber,
                cancellationToken);

        if (tracking == null)
        {
            return NotFound(new
            {
                message = "Tracking information not found."
            });
        }

        return Ok(tracking);
    }

    // GET: api/shipments/{shipmentId}/tracking
    [HttpGet("/api/shipments/{shipmentId:int}/tracking")]
    public async Task<ActionResult<List<TrackingDto>>> GetShipmentTracking(
        int shipmentId,
        CancellationToken cancellationToken)
    {
        var tracking = await _trackingService
            .GetShipmentTrackingAsync(
                shipmentId,
                cancellationToken);

        return Ok(tracking);
    }

    // POST: api/tracking
    [HttpPost]
    public async Task<ActionResult<TrackingDto>> Create(
        [FromBody] CreateTrackingDto dto,
        CancellationToken cancellationToken)
    {
        var tracking = await _trackingService
            .CreateAsync(
                dto,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetByTrackingNumber),
            new
            {
                trackingNumber = tracking.TrackingNumber
            },
            tracking);
    }

    // PUT: api/tracking/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TrackingDto>> Update(
        int id,
        [FromBody] UpdateTrackingDto dto,
        CancellationToken cancellationToken)
    {
        var tracking = await _trackingService
            .UpdateAsync(
                id,
                dto,
                cancellationToken);

        if (tracking == null)
        {
            return NotFound(new
            {
                message = "Tracking record not found."
            });
        }

        return Ok(tracking);
    }

    // GET: api/tracking/{id}/history
    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<List<TrackingDto>>> GetHistory(
        int id,
        CancellationToken cancellationToken)
    {
        var history = await _trackingService
            .GetHistoryAsync(
                id,
                cancellationToken);

        return Ok(history);
    }
}