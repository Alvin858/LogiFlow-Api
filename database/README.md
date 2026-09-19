# Database setup

The project uses SQL Server with Entity Framework Core migrations.

## Package Manager Console
```powershell
Update-Database -Project LogiFlow.Infrastructure -StartupProject LogiFlow.API
```

The supplied migration chain includes the existing team work plus:

- `AddBillingNotifications` — creates `Invoices`, `Payments` and `Notifications`.

If you are starting from a new database, `Update-Database` applies all pending migrations in order.

## Important
Check `src/LogiFlow.API/appsettings.json` and replace the development connection string with your own SQL Server connection before running the application.
