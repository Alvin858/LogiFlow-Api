using LogiFlow.Application.DTOs.Admin;

namespace LogiFlow.Application.Interfaces;

public interface IAdminService
{
    Task<AdminDashboardResponse> GetDashboardAsync(CancellationToken ct);
    Task<IReadOnlyCollection<ShipmentReportRow>> GetShipmentReportAsync(ReportFilter filter, CancellationToken ct);
    Task<IReadOnlyCollection<DeliveryReportRow>> GetDeliveryReportAsync(ReportFilter filter, CancellationToken ct);
    Task<IReadOnlyCollection<RevenueReportRow>> GetRevenueReportAsync(ReportFilter filter, CancellationToken ct);
    Task<IReadOnlyCollection<VehicleReportRow>> GetVehicleReportAsync(ReportFilter filter, CancellationToken ct);
    Task<IReadOnlyCollection<DriverReportRow>> GetDriverReportAsync(ReportFilter filter, CancellationToken ct);
    Task<IReadOnlyCollection<WarehouseReportRow>> GetWarehouseReportAsync(CancellationToken ct);
    Task<IReadOnlyCollection<AdminUserRow>> GetUsersAsync(CancellationToken ct);
    Task<IReadOnlyCollection<AdminCustomerRow>> GetCustomersAsync(CancellationToken ct);
    Task<IReadOnlyCollection<AdminDriverRow>> GetDriversAsync(CancellationToken ct);
    Task<IReadOnlyCollection<AdminVehicleRow>> GetVehiclesAsync(CancellationToken ct);
    Task<IReadOnlyCollection<AdminWarehouseRow>> GetWarehousesAsync(CancellationToken ct);
    Task<IReadOnlyCollection<AdminShipmentRow>> GetShipmentsAsync(CancellationToken ct);
    Task<IReadOnlyCollection<AdminDeliveryRow>> GetDeliveriesAsync(CancellationToken ct);
    Task<IReadOnlyCollection<AdminInvoiceRow>> GetInvoicesAsync(CancellationToken ct);
}
