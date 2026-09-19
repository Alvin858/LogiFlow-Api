
using LogiFlow.Application.DTOs.DeliveryDtos;

namespace LogiFlow.Application.Interfaces;

public interface IDeliveryService
{
    Task<IReadOnlyCollection<DeliveryResponse>> GetAllAsync(
        CancellationToken ct);

    Task<DeliveryResponse> GetByIdAsync(
        int id,
        CancellationToken ct);

    Task<DeliveryResponse> CreateAsync(
        CreateDeliveryRequest request,
        CancellationToken ct);

    Task<DeliveryResponse> UpdateAsync(
        int id,
        UpdateDeliveryRequest request,
        CancellationToken ct);

    Task DeleteAsync(
        int id,
        CancellationToken ct);

    Task<DeliveryResponse> PickupAsync(
        int id,
        CancellationToken ct);

    Task<DeliveryResponse> OutForDeliveryAsync(
        int id,
        CancellationToken ct);

    Task<DeliveryResponse> CompleteAsync(
        int id,
        CancellationToken ct);

    Task<DeliveryResponse> FailAsync(
        int id,
        FailDeliveryRequest request,
        CancellationToken ct);

    Task<ProofOfDeliveryResponse> AddProofAsync(
        int deliveryId,
        CreateProofOfDeliveryRequest request,
        CancellationToken ct);
}