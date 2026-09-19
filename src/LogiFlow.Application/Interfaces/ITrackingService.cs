using LogiFlow.Application.DTOs.Tracking;

namespace LogiFlow.Application.Interfaces;

public interface ITrackingService
{
    Task<TrackingDto?> GetByTrackingNumberAsync(
        string trackingNumber,
        CancellationToken cancellationToken = default);

    Task<List<TrackingDto>> GetShipmentTrackingAsync(
        int shipmentId,
        CancellationToken cancellationToken = default);

    Task<TrackingDto> CreateAsync(
        CreateTrackingDto dto,
        CancellationToken cancellationToken = default);

    Task<TrackingDto?> UpdateAsync(
        int id,
        UpdateTrackingDto dto,
        CancellationToken cancellationToken = default);

    Task<List<TrackingDto>> GetHistoryAsync(
        int id,
        CancellationToken cancellationToken = default);
}