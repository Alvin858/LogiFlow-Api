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
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<ISecurityService, SecurityService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<IRouteService, RouteService>();
        services.AddScoped<IDeliveryService, DeliveryService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAdminService, AdminService>();
        return services;
    }
}
