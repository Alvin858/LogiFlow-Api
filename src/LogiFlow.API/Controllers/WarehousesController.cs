using LogiFlow.Application.DTOs.Warehouses;
using LogiFlow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.API.Controllers;

[ApiController]
[Route("api/warehouses")]
[Authorize]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehousesController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    // GET: api/warehouses
    [HttpGet]
    public async Task<ActionResult<List<WarehouseDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var warehouses = await _warehouseService.GetAllAsync(
            cancellationToken);

        return Ok(warehouses);
    }

    // GET: api/warehouses/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<WarehouseDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseService.GetByIdAsync(
            id,
            cancellationToken);

        if (warehouse == null)
        {
            return NotFound(new
            {
                message = "Warehouse not found."
            });
        }

        return Ok(warehouse);
    }

    // POST: api/warehouses
    [HttpPost]
    public async Task<ActionResult<WarehouseDto>> Create(
        [FromBody] CreateWarehouseDto dto,
        CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseService.CreateAsync(
            dto,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = warehouse.Id },
            warehouse);
    }

    // PUT: api/warehouses/{id}
    [HttpPut("{id:int}")]
    public async Task<ActionResult<WarehouseDto>> Update(
        int id,
        [FromBody] UpdateWarehouseDto dto,
        CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseService.UpdateAsync(
            id,
            dto,
            cancellationToken);

        if (warehouse == null)
        {
            return NotFound(new
            {
                message = "Warehouse not found."
            });
        }

        return Ok(warehouse);
    }

    // DELETE: api/warehouses/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _warehouseService.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Warehouse not found."
            });
        }

        return NoContent();
    }

    // GET: api/warehouses/{id}/shipments
    [HttpGet("{id:int}/shipments")]
    public async Task<ActionResult<List<WarehouseShipmentDto>>>
        GetShipments(
            int id,
            CancellationToken cancellationToken)
    {
        var shipments = await _warehouseService.GetShipmentsAsync(
            id,
            cancellationToken);

        return Ok(shipments);
    }

    // POST: api/warehouses/{id}/receive
    [HttpPost("{id:int}/receive")]
    public async Task<ActionResult<WarehouseShipmentDto>> Receive(
        int id,
        [FromBody] ReceiveShipmentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _warehouseService.ReceiveAsync(
            id,
            dto,
            cancellationToken);

        return Ok(result);
    }

    // POST: api/warehouses/{id}/dispatch
    [HttpPost("{id:int}/dispatch")]
    public async Task<ActionResult<WarehouseShipmentDto>> Dispatch(
        int id,
        [FromBody] DispatchShipmentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _warehouseService.DispatchAsync(
            id,
            dto,
            cancellationToken);

        return Ok(result);
    }

    // GET: api/warehouse/inventory
    [HttpGet("/api/warehouse/inventory")]
    public async Task<ActionResult<WarehouseInventoryDto>>
        GetInventory(
            CancellationToken cancellationToken)
    {
        var inventory = await _warehouseService.GetInventoryAsync(
            cancellationToken);

        return Ok(inventory);
    }
}