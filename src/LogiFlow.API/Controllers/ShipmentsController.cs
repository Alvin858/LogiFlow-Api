using LogiFlow.Application.DTOs.Shipments;
using LogiFlow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.API.Controllers;

[ApiController]
[Route("api/shipments")]
[Authorize]
public class ShipmentsController : ControllerBase
{
    private readonly IShipmentService _shipmentService;

    public ShipmentsController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    // GET: api/shipments
    [HttpGet]
    public async Task<ActionResult<List<ShipmentDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var shipments = await _shipmentService.GetAllAsync(
            cancellationToken);

        return Ok(shipments);
    }

    // GET: api/shipments/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ShipmentDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentService.GetByIdAsync(
            id,
            cancellationToken);

        if (shipment == null)
        {
            return NotFound(new
            {
                message = "Shipment not found."
            });
        }

        return Ok(shipment);
    }

    // POST: api/shipments
    [HttpPost]
    public async Task<ActionResult<ShipmentDto>> Create(
        [FromBody] CreateShipmentDto dto,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentService.CreateAsync(
            dto,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = shipment.Id },
            shipment);
    }

    // PUT: api/shipments/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ShipmentDto>> Update(
        int id,
        [FromBody] UpdateShipmentDto dto,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentService.UpdateAsync(
            id,
            dto,
            cancellationToken);

        if (shipment == null)
        {
            return NotFound(new
            {
                message = "Shipment not found."
            });
        }

        return Ok(shipment);
    }

    // DELETE: api/shipments/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _shipmentService.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Shipment not found."
            });
        }

        return NoContent();
    }

    // PATCH: api/shipments/5/cancel
    [HttpPatch("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(
        int id,
        [FromBody] CancelShipmentDto dto,
        CancellationToken cancellationToken)
    {
        var cancelled = await _shipmentService.CancelAsync(
            id,
            dto,
            cancellationToken);

        if (!cancelled)
        {
            return NotFound(new
            {
                message = "Shipment not found."
            });
        }

        return Ok(new
        {
            message = "Shipment cancelled successfully."
        });
    }

    // PATCH: api/shipments/5/status
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateShipmentStatusDto dto,
        CancellationToken cancellationToken)
    {
        var updated = await _shipmentService.UpdateStatusAsync(
            id,
            dto,
            cancellationToken);

        if (!updated)
        {
            return NotFound(new
            {
                message = "Shipment not found."
            });
        }

        return Ok(new
        {
            message = "Shipment status updated successfully."
        });
    }

    // GET: api/customers/5/shipments
    [HttpGet("/api/customers/{customerId:int}/shipments")]
    public async Task<ActionResult<List<ShipmentDto>>> GetCustomerShipments(
        int customerId,
        CancellationToken cancellationToken)
    {
        var shipments = await _shipmentService
            .GetCustomerShipmentsAsync(
                customerId,
                cancellationToken);

        return Ok(shipments);
    }
}