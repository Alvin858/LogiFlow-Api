using LogiFlow.Application.DTOs.Warehouses;

namespace LogiFlow.Application.Interfaces;

public interface IWarehouseService
{
    Task<List<WarehouseDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<WarehouseDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<WarehouseDto> CreateAsync(
        CreateWarehouseDto dto,
        CancellationToken cancellationToken = default);

    Task<WarehouseDto?> UpdateAsync(
        int id,
        UpdateWarehouseDto dto,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<List<WarehouseShipmentDto>> GetShipmentsAsync(
        int warehouseId,
        CancellationToken cancellationToken = default);

    Task<WarehouseShipmentDto> ReceiveAsync(
        int warehouseId,
        ReceiveShipmentDto dto,
        CancellationToken cancellationToken = default);

    Task<WarehouseShipmentDto> DispatchAsync(
        int warehouseId,
        DispatchShipmentDto dto,
        CancellationToken cancellationToken = default);

    Task<WarehouseInventoryDto> GetInventoryAsync(
        CancellationToken cancellationToken = default);
}