using LogiFlow.Application.DTOs.Drivers;

namespace LogiFlow.Application.Interfaces;

public interface IDriverService
{
    Task<IReadOnlyCollection<DriverResponse>> GetAllAsync(CancellationToken ct);
    Task<DriverResponse> GetByIdAsync(int id, CancellationToken ct);
    Task<DriverResponse> CreateAsync(CreateDriverRequest request, CancellationToken ct);
    Task<DriverResponse> UpdateAsync(int id, UpdateDriverRequest request, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
    Task<DriverResponse> UpdateAvailabilityAsync(int id, UpdateDriverAvailabilityRequest request, CancellationToken ct);
}
