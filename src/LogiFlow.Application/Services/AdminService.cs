using LogiFlow.Application.DTOs.Admin;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Application.Services;

public class AdminService(IApplicationDbContext db) : IAdminService
{
    public async Task<AdminDashboardResponse> GetDashboardAsync(CancellationToken ct)
    {
        var totalShipments = await db.Shipments.CountAsync(ct);
        var activeShipments = await db.Shipments.CountAsync(x => x.Status != "Delivered" && x.Status != "Cancelled", ct);
        var totalDeliveries = await db.Deliveries.CountAsync(ct);
        var completedDeliveries = await db.Deliveries.CountAsync(x => x.Status == DeliveryStatus.Completed, ct);
        var failedDeliveries = await db.Deliveries.CountAsync(x => x.Status == DeliveryStatus.Failed, ct);
        var totalRevenue = await db.Payments.Where(x => x.Status == "Completed").Select(x => (decimal?)x.Amount).SumAsync(ct) ?? 0;
        var pendingRevenue = await db.Invoices.Where(x => x.Status != "Paid" && x.Status != "Cancelled").Select(x => (decimal?)x.TotalAmount).SumAsync(ct) ?? 0;
        return new(totalShipments, activeShipments, totalDeliveries, completedDeliveries, failedDeliveries,
            await db.Customers.CountAsync(ct), await db.Drivers.CountAsync(ct), await db.Drivers.CountAsync(x => x.IsAvailable, ct),
            await db.Vehicles.CountAsync(ct), await db.Vehicles.CountAsync(x => x.Status == VehicleStatus.Available, ct),
            await db.Warehouses.CountAsync(ct), totalRevenue, pendingRevenue);
    }

    public async Task<IReadOnlyCollection<ShipmentReportRow>> GetShipmentReportAsync(ReportFilter filter, CancellationToken ct)
    {
        var q = db.Shipments.AsNoTracking().AsQueryable();
        if (filter.FromUtc.HasValue) q = q.Where(x => x.CreatedAtUtc >= filter.FromUtc.Value);
        if (filter.ToUtc.HasValue) q = q.Where(x => x.CreatedAtUtc <= filter.ToUtc.Value);
        if (!string.IsNullOrWhiteSpace(filter.Status)) q = q.Where(x => x.Status == filter.Status);
        return await q.GroupBy(x => x.Status).Select(g => new ShipmentReportRow(g.Key, g.Count())).OrderBy(x => x.Status).ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<DeliveryReportRow>> GetDeliveryReportAsync(ReportFilter filter, CancellationToken ct)
    {
        var q = db.Deliveries.AsNoTracking().AsQueryable();
        if (filter.FromUtc.HasValue) q = q.Where(x => x.AssignedAtUtc >= filter.FromUtc.Value);
        if (filter.ToUtc.HasValue) q = q.Where(x => x.AssignedAtUtc <= filter.ToUtc.Value);
        if (!string.IsNullOrWhiteSpace(filter.Status) && Enum.TryParse<DeliveryStatus>(filter.Status, true, out var status)) q = q.Where(x => x.Status == status);
        return await q.GroupBy(x => x.Status).Select(g => new DeliveryReportRow(g.Key.ToString(), g.Count())).OrderBy(x => x.Status).ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<RevenueReportRow>> GetRevenueReportAsync(ReportFilter filter, CancellationToken ct)
    {
        var q = db.Payments.AsNoTracking().Where(x => x.Status == "Completed");
        if (filter.FromUtc.HasValue) q = q.Where(x => x.PaidAtUtc >= filter.FromUtc.Value);
        if (filter.ToUtc.HasValue) q = q.Where(x => x.PaidAtUtc <= filter.ToUtc.Value);
        return await q.GroupBy(x => x.PaidAtUtc!.Value.Date).Select(g => new RevenueReportRow(g.Key, g.Sum(x => x.Amount))).OrderBy(x => x.DateUtc).ToListAsync(ct);
    }

    public async Task<IReadOnlyCollection<VehicleReportRow>> GetVehicleReportAsync(ReportFilter filter, CancellationToken ct)
    {
        var vehicles = await db.Vehicles.AsNoTracking().OrderBy(x => x.RegistrationNumber).ToListAsync(ct);
        var deliveries = db.Deliveries.AsNoTracking().AsQueryable();
        if (filter.FromUtc.HasValue) deliveries = deliveries.Where(x => x.AssignedAtUtc >= filter.FromUtc.Value);
        if (filter.ToUtc.HasValue) deliveries = deliveries.Where(x => x.AssignedAtUtc <= filter.ToUtc.Value);
        var rows = await deliveries.GroupBy(x => x.VehicleId).Select(g => new { VehicleId = g.Key, Total = g.Count(), Completed = g.Count(x => x.Status == DeliveryStatus.Completed) }).ToListAsync(ct);
        return vehicles.Select(v => { var r = rows.FirstOrDefault(x => x.VehicleId == v.Id); var total = r?.Total ?? 0; return new VehicleReportRow(v.Id, v.RegistrationNumber, total, r?.Completed ?? 0, total == 0 ? 0 : r!.Completed * 100.0 / total); }).ToList();
    }

    public async Task<IReadOnlyCollection<DriverReportRow>> GetDriverReportAsync(ReportFilter filter, CancellationToken ct)
    {
        var drivers = await db.Drivers.AsNoTracking().Include(x => x.User).OrderBy(x => x.Id).ToListAsync(ct);
        var deliveries = db.Deliveries.AsNoTracking().AsQueryable();
        if (filter.FromUtc.HasValue) deliveries = deliveries.Where(x => x.AssignedAtUtc >= filter.FromUtc.Value);
        if (filter.ToUtc.HasValue) deliveries = deliveries.Where(x => x.AssignedAtUtc <= filter.ToUtc.Value);
        var rows = await deliveries.GroupBy(x => x.DriverId).Select(g => new { DriverId = g.Key, Total = g.Count(), Completed = g.Count(x => x.Status == DeliveryStatus.Completed), Failed = g.Count(x => x.Status == DeliveryStatus.Failed) }).ToListAsync(ct);
        return drivers.Select(d => { var r = rows.FirstOrDefault(x => x.DriverId == d.Id); var total = r?.Total ?? 0; return new DriverReportRow(d.Id, $"{d.User.FirstName} {d.User.LastName}".Trim(), total, r?.Completed ?? 0, r?.Failed ?? 0, total == 0 ? 0 : (r!.Completed * 100.0 / total)); }).ToList();
    }

    public async Task<IReadOnlyCollection<AdminUserRow>> GetUsersAsync(CancellationToken ct) =>
        await db.Users.AsNoTracking().Include(x => x.Role).OrderBy(x => x.Id).Select(x => new AdminUserRow(x.Id, (x.FirstName + " " + x.LastName).Trim(), x.Email, x.Role.Name, x.IsActive, x.CreatedAtUtc)).ToListAsync(ct);

    public async Task<IReadOnlyCollection<AdminCustomerRow>> GetCustomersAsync(CancellationToken ct) =>
        await db.Customers.AsNoTracking().Include(x => x.User).OrderBy(x => x.Id).Select(x => new AdminCustomerRow(x.Id, x.CompanyName, x.ContactPerson, x.User.Email)).ToListAsync(ct);

    public async Task<IReadOnlyCollection<AdminDriverRow>> GetDriversAsync(CancellationToken ct) =>
        await db.Drivers.AsNoTracking().Include(x => x.User).OrderBy(x => x.Id).Select(x => new AdminDriverRow(x.Id, (x.User.FirstName + " " + x.User.LastName).Trim(), x.LicenseNumber, x.IsAvailable)).ToListAsync(ct);

    public async Task<IReadOnlyCollection<AdminVehicleRow>> GetVehiclesAsync(CancellationToken ct) =>
        await db.Vehicles.AsNoTracking().OrderBy(x => x.Id).Select(x => new AdminVehicleRow(x.Id, x.RegistrationNumber, x.VehicleType, x.CapacityKg, x.Status.ToString())).ToListAsync(ct);

    public async Task<IReadOnlyCollection<AdminWarehouseRow>> GetWarehousesAsync(CancellationToken ct) =>
        await db.Warehouses.AsNoTracking().OrderBy(x => x.Id).Select(x => new AdminWarehouseRow(x.Id, x.Name, x.City, x.CapacityKg, x.UsedCapacityKg, x.Status)).ToListAsync(ct);

    public async Task<IReadOnlyCollection<AdminShipmentRow>> GetShipmentsAsync(CancellationToken ct) =>
        await db.Shipments.AsNoTracking().OrderByDescending(x => x.CreatedAtUtc).Select(x => new AdminShipmentRow(x.Id, x.TrackingNumber, x.CustomerId, x.Status, x.Priority, x.CreatedAtUtc)).ToListAsync(ct);

    public async Task<IReadOnlyCollection<AdminDeliveryRow>> GetDeliveriesAsync(CancellationToken ct) =>
        await db.Deliveries.AsNoTracking().OrderByDescending(x => x.AssignedAtUtc).Select(x => new AdminDeliveryRow(x.Id, x.ShipmentId, x.DriverId, x.VehicleId, x.RouteId, x.Status.ToString(), x.AssignedAtUtc)).ToListAsync(ct);

    public async Task<IReadOnlyCollection<AdminInvoiceRow>> GetInvoicesAsync(CancellationToken ct) =>
        await db.Invoices.AsNoTracking().OrderByDescending(x => x.IssuedAtUtc).Select(x => new AdminInvoiceRow(x.Id, x.InvoiceNumber, x.ShipmentId, x.CustomerId, x.TotalAmount, x.Status, x.IssuedAtUtc)).ToListAsync(ct);

    public async Task<IReadOnlyCollection<WarehouseReportRow>> GetWarehouseReportAsync(CancellationToken ct)
    {
        return await db.Warehouses.AsNoTracking().OrderBy(x => x.Name).Select(x => new WarehouseReportRow(x.Id, x.Name, x.CapacityKg, x.UsedCapacityKg, x.CapacityKg == 0 ? 0 : (double)(x.UsedCapacityKg * 100 / x.CapacityKg))).ToListAsync(ct);
    }
}
