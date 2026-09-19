namespace LogiFlow.Application.DTOs.Admin;

public record AdminDashboardResponse(
    int TotalShipments,
    int ActiveShipments,
    int TotalDeliveries,
    int CompletedDeliveries,
    int FailedDeliveries,
    int TotalCustomers,
    int TotalDrivers,
    int AvailableDrivers,
    int TotalVehicles,
    int AvailableVehicles,
    int TotalWarehouses,
    decimal TotalRevenue,
    decimal PendingRevenue);

public record ReportFilter(
    DateTime? FromUtc,
    DateTime? ToUtc,
    string? Status);

public record ShipmentReportRow(
    string Status,
    int Count);

public record DeliveryReportRow(
    string Status,
    int Count);

public record RevenueReportRow(
    DateTime DateUtc,
    decimal Amount);

public record VehicleReportRow(
    int VehicleId,
    string RegistrationNumber,
    int TotalDeliveries,
    int CompletedDeliveries,
    double UtilizationPercent);

public record DriverReportRow(
    int DriverId,
    string DriverName,
    int TotalDeliveries,
    int CompletedDeliveries,
    int FailedDeliveries,
    double SuccessRatePercent);

public record WarehouseReportRow(
    int WarehouseId,
    string Name,
    decimal CapacityKg,
    decimal UsedCapacityKg,
    double UtilizationPercent);

public record AdminUserRow(int Id, string Name, string Email, string Role, bool IsActive, DateTime CreatedAtUtc);
public record AdminCustomerRow(int Id, string CompanyName, string ContactPerson, string Email);
public record AdminDriverRow(int Id, string Name, string LicenseNumber, bool IsAvailable);
public record AdminVehicleRow(int Id, string RegistrationNumber, string VehicleType, decimal CapacityKg, string Status);
public record AdminWarehouseRow(int Id, string Name, string City, decimal CapacityKg, decimal UsedCapacityKg, string Status);
public record AdminShipmentRow(int Id, string TrackingNumber, int CustomerId, string Status, string Priority, DateTime CreatedAtUtc);
public record AdminDeliveryRow(int Id, int ShipmentId, int DriverId, int VehicleId, int RouteId, string Status, DateTime AssignedAtUtc);
public record AdminInvoiceRow(int Id, string InvoiceNumber, int ShipmentId, int CustomerId, decimal TotalAmount, string Status, DateTime IssuedAtUtc);
