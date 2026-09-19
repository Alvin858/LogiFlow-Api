using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    // =====================================================
    // Existing Entities
    // =====================================================

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Driver> Drivers => Set<Driver>();

    // =====================================================
    // Member 2 Entities
    // =====================================================

    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<ShipmentItem> ShipmentItems => Set<ShipmentItem>();
    public DbSet<ShipmentTracking> ShipmentTracking => Set<ShipmentTracking>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<WarehouseShipment> WarehouseShipments => Set<WarehouseShipment>();

    // Member 3
    public DbSet<Route> Routes => Set<Route>();
    public DbSet<RouteStop> RouteStops => Set<RouteStop>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<ProofOfDelivery> ProofsOfDelivery => Set<ProofOfDelivery>();
    public DbSet<Schedule> Schedules => Set<Schedule>();

    // Member 4
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =====================================================
        // Role
        // =====================================================

        modelBuilder.Entity<Role>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();

            e.HasIndex(x => x.Name)
                .IsUnique();
        });

        // =====================================================
        // User
        // =====================================================

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Email)
                .HasMaxLength(256)
                .IsRequired();

            e.HasIndex(x => x.Email)
                .IsUnique();

            e.Property(x => x.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            e.Property(x => x.LastName)
                .HasMaxLength(100)
                .IsRequired();

            e.Property(x => x.PasswordHash)
                .IsRequired();

            e.HasOne(x => x.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // Refresh Token
        // =====================================================

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.TokenHash)
                .HasMaxLength(128)
                .IsRequired();

            e.HasIndex(x => x.TokenHash)
                .IsUnique();

            e.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================================================
        // Customer
        // =====================================================

        modelBuilder.Entity<Customer>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasIndex(x => x.UserId)
                .IsUnique();

            e.Property(x => x.CompanyName)
                .HasMaxLength(200)
                .IsRequired();

            e.Property(x => x.ContactPerson)
                .HasMaxLength(200)
                .IsRequired();

            e.Property(x => x.TaxNumber)
                .HasMaxLength(50);

            e.HasOne(x => x.User)
                .WithOne(x => x.Customer)
                .HasForeignKey<Customer>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================================================
        // Customer Address
        // =====================================================

        modelBuilder.Entity<CustomerAddress>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.AddressLine1)
                .HasMaxLength(250)
                .IsRequired();

            e.Property(x => x.AddressLine2)
                .HasMaxLength(250);

            e.Property(x => x.City)
                .HasMaxLength(100)
                .IsRequired();

            e.Property(x => x.State)
                .HasMaxLength(100)
                .IsRequired();

            e.Property(x => x.PostalCode)
                .HasMaxLength(20)
                .IsRequired();

            e.Property(x => x.Country)
                .HasMaxLength(100)
                .IsRequired();

            e.HasIndex(x => new { x.CustomerId, x.IsDefault });

            e.HasOne(x => x.Customer)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================================================
        // Vehicle
        // =====================================================

        modelBuilder.Entity<Vehicle>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.VehicleType)
                .HasMaxLength(100)
                .IsRequired();

            e.Property(x => x.RegistrationNumber)
                .HasMaxLength(30)
                .IsRequired();

            e.HasIndex(x => x.RegistrationNumber)
                .IsUnique();

            e.Property(x => x.CapacityKg)
                .HasPrecision(18, 2);

            e.Property(x => x.Status)
                .HasConversion<int>();
        });

        // =====================================================
        // Driver
        // =====================================================

        modelBuilder.Entity<Driver>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasIndex(x => x.UserId)
                .IsUnique();

            e.HasIndex(x => x.LicenseNumber)
                .IsUnique();

            e.Property(x => x.LicenseNumber)
                .HasMaxLength(50)
                .IsRequired();

            e.HasOne(x => x.User)
                .WithOne(x => x.Driver)
                .HasForeignKey<Driver>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================================================
        // Shipment
        // =====================================================

        modelBuilder.Entity<Shipment>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasIndex(x => x.TrackingNumber)
                .IsUnique();

            e.Property(x => x.TrackingNumber)
                .HasMaxLength(50)
                .IsRequired();

            e.Property(x => x.PackageDescription)
                .HasMaxLength(500)
                .IsRequired();

            e.Property(x => x.WeightKg)
                .HasPrecision(18, 2);

            e.Property(x => x.LengthCm)
                .HasPrecision(18, 2);

            e.Property(x => x.WidthCm)
                .HasPrecision(18, 2);

            e.Property(x => x.HeightCm)
                .HasPrecision(18, 2);

            e.Property(x => x.Priority)
                .HasMaxLength(30)
                .IsRequired();

            e.Property(x => x.Status)
                .HasMaxLength(50)
                .IsRequired();

            e.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.PickupAddress)
                .WithMany()
                .HasForeignKey(x => x.PickupAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.DeliveryAddress)
                .WithMany()
                .HasForeignKey(x => x.DeliveryAddressId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // Shipment Item
        // =====================================================

        modelBuilder.Entity<ShipmentItem>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.ItemName)
                .HasMaxLength(200)
                .IsRequired();

            e.Property(x => x.Description)
                .HasMaxLength(500);

            e.Property(x => x.WeightKg)
                .HasPrecision(18, 2);

            e.Property(x => x.LengthCm)
                .HasPrecision(18, 2);

            e.Property(x => x.WidthCm)
                .HasPrecision(18, 2);

            e.Property(x => x.HeightCm)
                .HasPrecision(18, 2);

            e.HasOne(x => x.Shipment)
                .WithMany(x => x.ShipmentItems)
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================================================
        // Shipment Tracking
        // =====================================================

        modelBuilder.Entity<ShipmentTracking>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Status)
                .HasMaxLength(50)
                .IsRequired();

            e.Property(x => x.Location)
                .HasMaxLength(250);

            e.Property(x => x.Latitude)
                .HasPrecision(10, 7);

            e.Property(x => x.Longitude)
                .HasPrecision(10, 7);

            e.Property(x => x.Remarks)
                .HasMaxLength(500);

            e.HasOne(x => x.Shipment)
                .WithMany(x => x.TrackingHistory)
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================================================
        // Warehouse
        // =====================================================

        modelBuilder.Entity<Warehouse>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            e.Property(x => x.Address)
                .HasMaxLength(250)
                .IsRequired();

            e.Property(x => x.City)
                .HasMaxLength(100)
                .IsRequired();

            e.Property(x => x.State)
                .HasMaxLength(100)
                .IsRequired();

            e.Property(x => x.PostalCode)
                .HasMaxLength(20)
                .IsRequired();

            e.Property(x => x.CapacityKg)
                .HasPrecision(18, 2);

            e.Property(x => x.UsedCapacityKg)
                .HasPrecision(18, 2);

            e.Property(x => x.Status)
                .HasMaxLength(50)
                .IsRequired();
        });

        // =====================================================
        // Warehouse Shipment
        // =====================================================

        modelBuilder.Entity<WarehouseShipment>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.StorageLocation)
                .HasMaxLength(100);

            e.Property(x => x.Status)
                .HasMaxLength(50)
                .IsRequired();

            e.HasOne(x => x.Warehouse)
                .WithMany(x => x.WarehouseShipments)
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Shipment)
                .WithMany(x => x.WarehouseShipments)
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =====================================================
        // MEMBER 3 - ROUTE
        // =====================================================
        modelBuilder.Entity<Route>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.StartLocation).HasMaxLength(250).IsRequired();
            e.Property(x => x.Destination).HasMaxLength(250).IsRequired();
            e.Property(x => x.DistanceKm).HasPrecision(18, 2);
            e.Property(x => x.EstimatedDurationMinutes).IsRequired();
            e.Property(x => x.Status).HasConversion<int>();
            e.HasIndex(x => x.Status);
            e.HasMany(x => x.RouteStops).WithOne(x => x.Route).HasForeignKey(x => x.RouteId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RouteStop>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Address).HasMaxLength(500).IsRequired();
            e.Property(x => x.Latitude).HasPrecision(9, 6);
            e.Property(x => x.Longitude).HasPrecision(9, 6);
            e.HasIndex(x => new { x.RouteId, x.StopOrder }).IsUnique();
        });

        modelBuilder.Entity<Delivery>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).HasConversion<int>();
            e.Property(x => x.FailureReason).HasMaxLength(1000);
            e.HasIndex(x => x.ShipmentId);
            e.HasIndex(x => x.DriverId);
            e.HasIndex(x => x.VehicleId);
            e.HasIndex(x => x.RouteId);
            e.HasIndex(x => x.Status);
            e.HasMany(x => x.ProofsOfDelivery).WithOne(x => x.Delivery).HasForeignKey(x => x.DeliveryId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProofOfDelivery>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ReceiverName).HasMaxLength(200).IsRequired();
            e.Property(x => x.SignaturePath).HasMaxLength(500).IsRequired();
            e.Property(x => x.PhotoPath).HasMaxLength(500).IsRequired();
            e.Property(x => x.Remarks).HasMaxLength(1000);
            e.HasIndex(x => x.DeliveryId);
        });

        modelBuilder.Entity<Schedule>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).HasConversion<int>();
            e.HasIndex(x => x.ShipmentId);
            e.HasIndex(x => x.DriverId);
            e.HasIndex(x => x.VehicleId);
            e.HasIndex(x => x.StartTimeUtc);
            e.HasIndex(x => x.EndTimeUtc);
            e.HasIndex(x => x.Status);
        });

        // =====================================================
        // MEMBER 4 - INVOICE
        // =====================================================
        modelBuilder.Entity<Invoice>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.InvoiceNumber).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.InvoiceNumber).IsUnique();
            e.HasIndex(x => x.ShipmentId).IsUnique();
            e.HasIndex(x => x.CustomerId);
            e.Property(x => x.Subtotal).HasPrecision(18, 2);
            e.Property(x => x.TaxAmount).HasPrecision(18, 2);
            e.Property(x => x.DiscountAmount).HasPrecision(18, 2);
            e.Property(x => x.TotalAmount).HasPrecision(18, 2);
            e.Property(x => x.Status).HasMaxLength(30).IsRequired();
            e.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Shipment).WithMany().HasForeignKey(x => x.ShipmentId).OnDelete(DeleteBehavior.Restrict);
        });

        // =====================================================
        // MEMBER 4 - PAYMENT
        // =====================================================
        modelBuilder.Entity<Payment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.PaymentReference).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.PaymentReference).IsUnique();
            e.Property(x => x.PaymentMethod).HasMaxLength(50).IsRequired();
            e.Property(x => x.Amount).HasPrecision(18, 2);
            e.Property(x => x.Status).HasMaxLength(30).IsRequired();
            e.HasIndex(x => x.InvoiceId);
            e.HasOne(x => x.Invoice).WithMany(x => x.Payments).HasForeignKey(x => x.InvoiceId).OnDelete(DeleteBehavior.Cascade);
        });

        // =====================================================
        // MEMBER 4 - NOTIFICATION
        // =====================================================
        modelBuilder.Entity<Notification>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Message).HasMaxLength(1000).IsRequired();
            e.Property(x => x.Type).HasMaxLength(50).IsRequired();
            e.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAtUtc });
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // =====================================================
        // Role Seed Data
        // =====================================================

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "Logistics Staff" },
            new Role { Id = 3, Name = "Driver" },
            new Role { Id = 4, Name = "Customer" }
        );
    }
}