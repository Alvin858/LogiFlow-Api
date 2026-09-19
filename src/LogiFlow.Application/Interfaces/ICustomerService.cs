using LogiFlow.Application.DTOs.Customers;

namespace LogiFlow.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerResponse> GetProfileAsync(int userId, CancellationToken ct);
    Task<CustomerResponse> UpdateProfileAsync(int userId, UpdateCustomerProfileRequest request, CancellationToken ct);
    Task<IReadOnlyCollection<AddressResponse>> GetAddressesAsync(int userId, CancellationToken ct);
    Task<AddressResponse> AddAddressAsync(int userId, CreateAddressRequest request, CancellationToken ct);
    Task<AddressResponse> UpdateAddressAsync(int userId, int addressId, UpdateAddressRequest request, CancellationToken ct);
    Task DeleteAddressAsync(int userId, int addressId, CancellationToken ct);
    Task<CustomerResponse> GetByIdAsync(int customerId, CancellationToken ct);
}
