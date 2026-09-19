using LogiFlow.Application.DTOs.Vehicles;

namespace LogiFlow.Application.Interfaces;

public interface IVehicleService
{
    Task<IReadOnlyCollection<VehicleResponse>> GetAllAsync(CancellationToken ct);
    Task<VehicleResponse> GetByIdAsync(int id, CancellationToken ct);
    Task<VehicleResponse> CreateAsync(CreateVehicleRequest request, CancellationToken ct);
    Task<VehicleResponse> UpdateAsync(int id, UpdateVehicleRequest request, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
    Task<VehicleResponse> UpdateStatusAsync(int id, UpdateVehicleStatusRequest request, CancellationToken ct);
}
