using LogiFlow.Application.DTOs.Admin;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = RoleNames.Admin)]
public class AdminController(IAdminService service) : ControllerBase
{
    // Dashboard
    [HttpGet("dashboard")]
    public async Task<ActionResult<AdminDashboardResponse>> Dashboard(
        CancellationToken ct) =>
        Ok(await service.GetDashboardAsync(ct));

    // Admin Lists
    [HttpGet("users")]
    public async Task<ActionResult<IReadOnlyCollection<AdminUserRow>>> Users(
        CancellationToken ct) =>
        Ok(await service.GetUsersAsync(ct));

    [HttpGet("customers")]
    public async Task<ActionResult<IReadOnlyCollection<AdminCustomerRow>>> Customers(
        CancellationToken ct) =>
        Ok(await service.GetCustomersAsync(ct));

    [HttpGet("drivers")]
    public async Task<ActionResult<IReadOnlyCollection<AdminDriverRow>>> Drivers(
        CancellationToken ct) =>
        Ok(await service.GetDriversAsync(ct));

    [HttpGet("vehicles")]
    public async Task<ActionResult<IReadOnlyCollection<AdminVehicleRow>>> Vehicles(
        CancellationToken ct) =>
        Ok(await service.GetVehiclesAsync(ct));

    [HttpGet("warehouses")]
    public async Task<ActionResult<IReadOnlyCollection<AdminWarehouseRow>>> Warehouses(
        CancellationToken ct) =>
        Ok(await service.GetWarehousesAsync(ct));

    [HttpGet("shipments")]
    public async Task<ActionResult<IReadOnlyCollection<AdminShipmentRow>>> Shipments(
        CancellationToken ct) =>
        Ok(await service.GetShipmentsAsync(ct));

    [HttpGet("deliveries")]
    public async Task<ActionResult<IReadOnlyCollection<AdminDeliveryRow>>> DeliveryList(
        CancellationToken ct) =>
        Ok(await service.GetDeliveriesAsync(ct));

    [HttpGet("invoices")]
    public async Task<ActionResult<IReadOnlyCollection<AdminInvoiceRow>>> InvoiceList(
        CancellationToken ct) =>
        Ok(await service.GetInvoicesAsync(ct));


    // Reports
    [HttpGet("reports/shipments")]
    public async Task<ActionResult<IReadOnlyCollection<ShipmentReportRow>>> ShipmentReport(
        [FromQuery] ReportFilter filter,
        CancellationToken ct) =>
        Ok(await service.GetShipmentReportAsync(filter, ct));

    [HttpGet("reports/deliveries")]
    public async Task<ActionResult<IReadOnlyCollection<DeliveryReportRow>>> DeliveryReport(
        [FromQuery] ReportFilter filter,
        CancellationToken ct) =>
        Ok(await service.GetDeliveryReportAsync(filter, ct));

    [HttpGet("reports/revenue")]
    public async Task<ActionResult<IReadOnlyCollection<RevenueReportRow>>> RevenueReport(
        [FromQuery] ReportFilter filter,
        CancellationToken ct) =>
        Ok(await service.GetRevenueReportAsync(filter, ct));

    [HttpGet("reports/vehicles")]
    public async Task<ActionResult<IReadOnlyCollection<VehicleReportRow>>> VehicleReport(
        [FromQuery] ReportFilter filter,
        CancellationToken ct) =>
        Ok(await service.GetVehicleReportAsync(filter, ct));

    [HttpGet("reports/drivers")]
    public async Task<ActionResult<IReadOnlyCollection<DriverReportRow>>> DriverReport(
        [FromQuery] ReportFilter filter,
        CancellationToken ct) =>
        Ok(await service.GetDriverReportAsync(filter, ct));

    [HttpGet("reports/warehouses")]
    public async Task<ActionResult<IReadOnlyCollection<WarehouseReportRow>>> WarehouseReport(
        CancellationToken ct) =>
        Ok(await service.GetWarehouseReportAsync(ct));
}