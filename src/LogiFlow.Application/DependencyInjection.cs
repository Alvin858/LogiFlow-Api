using FluentValidation;
using LogiFlow.Application.Interfaces;
using LogiFlow.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LogiFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly);

        services.AddScoped<IShipmentService, ShipmentService>();

        services.AddScoped<ITrackingService, TrackingService>();

        services.AddScoped<IWarehouseService, WarehouseService>();

        return services;
    }
}