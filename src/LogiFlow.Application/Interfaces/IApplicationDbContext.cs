using LogiFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Role> Roles { get; }
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Customer> Customers { get; }
    DbSet<CustomerAddress> CustomerAddresses { get; }
    DbSet<Vehicle> Vehicles { get; }
    DbSet<Driver> Drivers { get; }

    // Member 2 - Shipment Management
    DbSet<Shipment> Shipments { get; }
    DbSet<ShipmentItem> ShipmentItems { get; }
    DbSet<ShipmentTracking> ShipmentTracking { get; }

    // Member 2 - Warehouse Management
    DbSet<Warehouse> Warehouses { get; }
    DbSet<WarehouseShipment> WarehouseShipments { get; }

    // Member 3 - Route, delivery and scheduling
    DbSet<Route> Routes { get; }
    DbSet<RouteStop> RouteStops { get; }
    DbSet<Delivery> Deliveries { get; }
    DbSet<ProofOfDelivery> ProofsOfDelivery { get; }
    DbSet<Schedule> Schedules { get; }

    // Member 4 - Billing, notifications and administration
    DbSet<Invoice> Invoices { get; }
    DbSet<Payment> Payments { get; }
    DbSet<Notification> Notifications { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}