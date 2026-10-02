using LogiFlow.Application.Interfaces;
using LogiFlow.Application.Services;
using LogiFlow.Infrastructure.Data;
using LogiFlow.Infrastructure.Services;
using LogiFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogiFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // --------------------------------------------------
        // DATABASE
        // --------------------------------------------------

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
        });


        // --------------------------------------------------
        // DATABASE CONTEXT INTERFACE
        // --------------------------------------------------

        services.AddScoped<IApplicationDbContext>(
            sp => sp.GetRequiredService<ApplicationDbContext>());


        // --------------------------------------------------
        // PASSWORD HASHER
        // --------------------------------------------------

        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();


        // --------------------------------------------------
        // SECURITY & AUTHENTICATION
        // --------------------------------------------------

        services.AddScoped<ISecurityService, SecurityService>();
        services.AddScoped<IAuthService, AuthService>();


        // --------------------------------------------------
        // CUSTOMER
        // --------------------------------------------------

        services.AddScoped<ICustomerService, CustomerService>();


        // --------------------------------------------------
        // VEHICLE & DRIVER
        // --------------------------------------------------

        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IDriverService, DriverService>();


        // --------------------------------------------------
        // ROUTES
        // --------------------------------------------------

        services.AddScoped<IRouteService, RouteService>();


        // --------------------------------------------------
        // DELIVERY
        // --------------------------------------------------

        services.AddScoped<IDeliveryService, DeliveryService>();


        // --------------------------------------------------
        // SCHEDULE
        // --------------------------------------------------

        services.AddScoped<IScheduleService, ScheduleService>();


        // --------------------------------------------------
        // BILLING
        // --------------------------------------------------

        services.AddScoped<IInvoiceService, InvoiceService>();


        // --------------------------------------------------
        // NOTIFICATIONS
        // --------------------------------------------------

        services.AddScoped<INotificationService, NotificationService>();


        // --------------------------------------------------
        // ADMIN
        // --------------------------------------------------

        services.AddScoped<IAdminService, AdminService>();


        return services;
    }
}