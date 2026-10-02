using LogiFlow.Domain.Constants;
using LogiFlow.Domain.Entities;
using LogiFlow.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Infrastructure.Data;

public static class DatabaseSeeder
{
    // ============================================================
    // DEMO ACCOUNTS
    // ============================================================

    private const string StaffEmail = "staff@logiflow.local";
    private const string StaffPassword = "Staff@12345";

    private const string DriverEmail = "driver@logiflow.local";
    private const string DriverPassword = "Driver@12345";

    private const string CustomerEmail = "customer@logiflow.local";
    private const string CustomerPassword = "Customer@12345";


    // ============================================================
    // MAIN SEED METHOD
    // ============================================================

    public static async Task SeedAsync(
        ApplicationDbContext db,
        IPasswordHasher<User> hasher,
        string email,
        string password,
        CancellationToken ct = default)
    {
        // Apply migrations
        await db.Database.MigrateAsync(ct);


        // ========================================================
        // GET ALL ROLES
        // ========================================================

        var adminRole = await db.Roles
            .SingleAsync(x => x.Name == RoleNames.Admin, ct);

        var staffRole = await db.Roles
            .SingleAsync(x => x.Name == RoleNames.LogisticsStaff, ct);

        var driverRole = await db.Roles
            .SingleAsync(x => x.Name == RoleNames.Driver, ct);

        var customerRole = await db.Roles
            .SingleAsync(x => x.Name == RoleNames.Customer, ct);


        // ========================================================
        // CREATE / REPAIR DEMO USERS
        // ========================================================

        var admin = await EnsureUserAsync(
            db,
            hasher,
            email,
            password,
            "System",
            "Administrator",
            adminRole.Id,
            "+91 9000000001",
            ct);


        var staff = await EnsureUserAsync(
            db,
            hasher,
            StaffEmail,
            StaffPassword,
            "Logistics",
            "Staff",
            staffRole.Id,
            "+91 9000000002",
            ct);


        var driverUser = await EnsureUserAsync(
            db,
            hasher,
            DriverEmail,
            DriverPassword,
            "Demo",
            "Driver",
            driverRole.Id,
            "+91 9000000003",
            ct);


        var customerUser = await EnsureUserAsync(
            db,
            hasher,
            CustomerEmail,
            CustomerPassword,
            "Demo",
            "Customer",
            customerRole.Id,
            "+91 9000000004",
            ct);


        // ========================================================
        // CUSTOMER PROFILE
        // ========================================================

        var customer = await db.Customers
            .SingleOrDefaultAsync(
                x => x.UserId == customerUser.Id,
                ct);


        if (customer == null)
        {
            customer = new Customer
            {
                UserId = customerUser.Id,
                CompanyName = "Demo Customer Logistics",
                ContactPerson = "Demo Customer"
            };

            db.Customers.Add(customer);

            await db.SaveChangesAsync(ct);
        }


        // ========================================================
        // CUSTOMER ADDRESSES
        // ========================================================

        var addresses = await db.CustomerAddresses
            .Where(x => x.CustomerId == customer.Id)
            .OrderBy(x => x.Id)
            .ToListAsync(ct);


        if (addresses.Count == 0)
        {
            var pickupAddress = new CustomerAddress
            {
                CustomerId = customer.Id,
                AddressLine1 = "10 Demo Main Road",
                City = "Nagercoil",
                State = "Tamil Nadu",
                PostalCode = "629001",
                Country = "India",
                IsDefault = true
            };


            var deliveryAddress = new CustomerAddress
            {
                CustomerId = customer.Id,
                AddressLine1 = "25 Demo Delivery Street",
                City = "Nagercoil",
                State = "Tamil Nadu",
                PostalCode = "629002",
                Country = "India",
                IsDefault = false
            };


            db.CustomerAddresses.AddRange(
                pickupAddress,
                deliveryAddress
            );

            await db.SaveChangesAsync(ct);


            addresses = await db.CustomerAddresses
                .Where(x => x.CustomerId == customer.Id)
                .OrderBy(x => x.Id)
                .ToListAsync(ct);
        }
        else if (addresses.Count == 1)
        {
            var deliveryAddress = new CustomerAddress
            {
                CustomerId = customer.Id,
                AddressLine1 = "25 Demo Delivery Street",
                City = "Nagercoil",
                State = "Tamil Nadu",
                PostalCode = "629002",
                Country = "India",
                IsDefault = false
            };


            db.CustomerAddresses.Add(deliveryAddress);

            await db.SaveChangesAsync(ct);


            addresses = await db.CustomerAddresses
                .Where(x => x.CustomerId == customer.Id)
                .OrderBy(x => x.Id)
                .ToListAsync(ct);
        }


        // ========================================================
        // DRIVER PROFILE
        // ========================================================

        var driver = await db.Drivers
            .SingleOrDefaultAsync(
                x => x.UserId == driverUser.Id,
                ct);


        if (driver == null)
        {
            driver = new Driver
            {
                UserId = driverUser.Id,
                LicenseNumber = "DEMO-LIC-001",
                LicenseExpiryDate = DateTime.UtcNow.AddYears(2),
                ExperienceYears = 5,
                IsAvailable = true
            };


            db.Drivers.Add(driver);

            await db.SaveChangesAsync(ct);
        }


        // ========================================================
        // VEHICLE
        // ========================================================

        var vehicle = await db.Vehicles
            .SingleOrDefaultAsync(
                x => x.RegistrationNumber == "TN-DMO-001",
                ct);


        if (vehicle == null)
        {
            vehicle = new Vehicle
            {
                VehicleType = "Delivery Van",
                RegistrationNumber = "TN-DMO-001",
                CapacityKg = 1500,
                Status = VehicleStatus.Available,
                InsuranceExpiryDate = DateTime.UtcNow.AddYears(1),
                FitnessExpiryDate = DateTime.UtcNow.AddYears(1)
            };


            db.Vehicles.Add(vehicle);

            await db.SaveChangesAsync(ct);
        }


        // ========================================================
        // WAREHOUSE
        // ========================================================

        var warehouse = await db.Warehouses
            .SingleOrDefaultAsync(
                x => x.Name == "Demo Central Warehouse",
                ct);


        if (warehouse == null)
        {
            warehouse = new Warehouse
            {
                Name = "Demo Central Warehouse",
                Address = "50 Logistics Park",
                City = "Nagercoil",
                State = "Tamil Nadu",
                PostalCode = "629003",
                CapacityKg = 10000,
                UsedCapacityKg = 0,
                Status = "Available"
            };


            db.Warehouses.Add(warehouse);

            await db.SaveChangesAsync(ct);
        }


        // ========================================================
        // ROUTE
        // ========================================================

        var route = await db.Routes
            .SingleOrDefaultAsync(
                x =>
                    x.StartLocation == "Demo Central Warehouse" &&
                    x.Destination == "Demo Delivery Street",
                ct);


        if (route == null)
        {
            route = new Route
            {
                StartLocation = "Demo Central Warehouse",
                Destination = "Demo Delivery Street",
                DistanceKm = 12.5m,
                EstimatedDurationMinutes = 35,
                Status = RouteStatus.Planned
            };


            db.Routes.Add(route);

            await db.SaveChangesAsync(ct);


            db.RouteStops.AddRange(

                new RouteStop
                {
                    RouteId = route.Id,
                    StopOrder = 1,
                    Address = "Demo Central Warehouse",
                    Latitude = 8.1780m,
                    Longitude = 77.4340m
                },

                new RouteStop
                {
                    RouteId = route.Id,
                    StopOrder = 2,
                    Address = "25 Demo Delivery Street, Nagercoil",
                    Latitude = 8.1810m,
                    Longitude = 77.4420m
                }
            );


            await db.SaveChangesAsync(ct);
        }


        // ========================================================
        // DEMO SHIPMENT
        // ========================================================

        var shipment = await db.Shipments
            .SingleOrDefaultAsync(
                x => x.TrackingNumber == "LF-DEMO-0001",
                ct);


        if (shipment == null)
        {
            shipment = new Shipment
            {
                CustomerId = customer.Id,

                PickupAddressId = addresses[0].Id,

                DeliveryAddressId = addresses[1].Id,

                TrackingNumber = "LF-DEMO-0001",

                PackageDescription =
                    "Demo electronics package",

                WeightKg = 15,

                LengthCm = 40,

                WidthCm = 30,

                HeightCm = 25,

                Priority = "High",

                ExpectedDeliveryDate =
                    DateTime.UtcNow.AddDays(1),

                Status = "Created"
            };


            shipment.ShipmentItems.Add(
                new ShipmentItem
                {
                    ItemName = "Demo Electronics",

                    Description =
                        "Demo shipment item",

                    Quantity = 1,

                    WeightKg = 15,

                    LengthCm = 40,

                    WidthCm = 30,

                    HeightCm = 25
                }
            );


            db.Shipments.Add(shipment);

            await db.SaveChangesAsync(ct);
        }


        // ========================================================
        // WAREHOUSE SHIPMENT
        // ========================================================

        var warehouseShipment =
            await db.WarehouseShipments
                .SingleOrDefaultAsync(
                    x =>
                        x.ShipmentId == shipment.Id &&
                        x.WarehouseId == warehouse.Id,
                    ct);


        if (warehouseShipment == null)
        {
            warehouseShipment = new WarehouseShipment
            {
                WarehouseId = warehouse.Id,

                ShipmentId = shipment.Id,

                StorageLocation = "A-01",

                ReceivedAtUtc = DateTime.UtcNow,

                Status = "Received"
            };


            warehouse.UsedCapacityKg +=
                shipment.WeightKg;


            db.WarehouseShipments.Add(
                warehouseShipment);


            await db.SaveChangesAsync(ct);
        }


        // ========================================================
        // DELIVERY
        // ========================================================

        var delivery = await db.Deliveries
            .SingleOrDefaultAsync(
                x => x.ShipmentId == shipment.Id,
                ct);


        if (delivery == null)
        {
            delivery = new Delivery
            {
                ShipmentId = shipment.Id,

                DriverId = driver.Id,

                VehicleId = vehicle.Id,

                RouteId = route.Id,

                Status = DeliveryStatus.Assigned,

                AssignedAtUtc = DateTime.UtcNow
            };


            db.Deliveries.Add(delivery);


            shipment.Status = "Assigned";


            // ====================================================
            // SCHEDULE
            // ====================================================

            db.Schedules.Add(
                new Schedule
                {
                    ShipmentId = shipment.Id,

                    DriverId = driver.Id,

                    VehicleId = vehicle.Id,

                    StartTimeUtc =
                        DateTime.UtcNow.AddHours(1),

                    EndTimeUtc =
                        DateTime.UtcNow.AddHours(3),

                    Status = ScheduleStatus.Scheduled
                }
            );


            // ====================================================
            // TRACKING
            // ====================================================

            db.ShipmentTracking.Add(
                new ShipmentTracking
                {
                    ShipmentId = shipment.Id,

                    Status = "Assigned",

                    Location =
                        "Demo Central Warehouse",

                    Remarks =
                        "Ready for driver pickup"
                }
            );


            await db.SaveChangesAsync(ct);
        }
    }


    // ============================================================
    // CREATE OR REPAIR USER
    // ============================================================

    private static async Task<User> EnsureUserAsync(
        ApplicationDbContext db,
        IPasswordHasher<User> hasher,
        string email,
        string password,
        string firstName,
        string lastName,
        int roleId,
        string phone,
        CancellationToken ct)
    {
        var normalizedEmail =
            email.Trim().ToLowerInvariant();


        var user = await db.Users
            .SingleOrDefaultAsync(
                x => x.Email == normalizedEmail,
                ct);


        // ========================================================
        // USER DOES NOT EXIST
        // ========================================================

        if (user == null)
        {
            user = new User
            {
                FirstName = firstName,

                LastName = lastName,

                Email = normalizedEmail,

                PhoneNumber = phone,

                RoleId = roleId,

                IsActive = true
            };


            user.PasswordHash =
                hasher.HashPassword(
                    user,
                    password);


            db.Users.Add(user);

            await db.SaveChangesAsync(ct);

            return user;
        }


        // ========================================================
        // USER ALREADY EXISTS
        // ========================================================

        user.FirstName = firstName;

        user.LastName = lastName;

        user.PhoneNumber = phone;

        user.RoleId = roleId;

        user.IsActive = true;


        // ========================================================
        // RESET PASSWORD IF NECESSARY
        // ========================================================

        var passwordResult =
            hasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password);


        if (passwordResult !=
            PasswordVerificationResult.Success)
        {
            user.PasswordHash =
                hasher.HashPassword(
                    user,
                    password);
        }


        await db.SaveChangesAsync(ct);


        return user;
    }
}