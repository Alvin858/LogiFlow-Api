# LogiFlow — Integrated Logistics & Delivery Management System

Complete backend assembled from the supplied team work and extended with **Member 4 (M10-M12)**.

## Technology
- ASP.NET Core Web API / .NET 8
- C#
- Entity Framework Core 8
- Microsoft SQL Server
- JWT Bearer authentication + role-based authorization
- FluentValidation
- Swagger

## Team modules included
- **M1-M3:** Authentication, customer management, vehicles and drivers
- **M4-M6:** Shipments, shipment tracking and warehouse management
- **M7-M9:** Routes, deliveries and scheduling
- **M10-M12:** Billing & payments, notifications, admin dashboard and reports

## Member 4 implementation
### M10 Billing & Payments
- Invoice creation/update and automatic subtotal + tax - discount calculation
- Unique invoice numbers
- Customer-only invoice/payment access
- Payment recording and payment-reference validation
- Payment status updates and invoice Paid state
- Invoice/payment history

### M11 Notifications
- User notification inbox
- Unread notification retrieval
- Mark one/read-all/delete operations
- Admin-created notifications
- Automatic notifications for shipment creation/status changes, warehouse arrival, delivery assignment/pickup/completion, scheduling and invoice generation

### M12 Administration & Analytics
- Admin-only dashboard KPIs
- Existing-data administration endpoints for users, customers, drivers, vehicles, warehouses, shipments, deliveries and invoices
- Shipment, delivery, revenue, vehicle, driver and warehouse reports
- Date/status filtering where applicable
- No duplicate business tables

## Run
1. Open `LogiFlow.sln` in Visual Studio.
2. Restore NuGet packages.
3. Set `LogiFlow.API` as the startup project.
4. Verify the SQL Server connection string in `src/LogiFlow.API/appsettings.json`.
5. Run the migrations described in `database/README.md`.
6. Start the API and open Swagger.

The existing project seeds the development Admin role/account configuration. Replace development secrets/credentials before production use.
