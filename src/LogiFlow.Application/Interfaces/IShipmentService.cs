using LogiFlow.Application.DTOs.Shipments;

namespace LogiFlow.Application.Interfaces;

public interface IShipmentService
{
    Task<List<ShipmentDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ShipmentDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ShipmentDto> CreateAsync(
        CreateShipmentDto dto,
        CancellationToken cancellationToken = default);

    Task<ShipmentDto?> UpdateAsync(
        int id,
        UpdateShipmentDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<bool> CancelAsync(
        int id,
        CancelShipmentDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateStatusAsync(
        int id,
        UpdateShipmentStatusDto dto,
        CancellationToken cancellationToken = default);

    Task<List<ShipmentDto>> GetCustomerShipmentsAsync(
        int customerId,
        CancellationToken cancellationToken = default);
}