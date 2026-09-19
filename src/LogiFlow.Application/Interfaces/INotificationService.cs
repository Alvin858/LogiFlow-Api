using LogiFlow.Application.DTOs.Notifications;

namespace LogiFlow.Application.Interfaces;

public interface INotificationService
{
    Task<IReadOnlyCollection<NotificationResponse>> GetAllAsync(int userId, CancellationToken ct);
    Task<IReadOnlyCollection<NotificationResponse>> GetUnreadAsync(int userId, CancellationToken ct);
    Task<NotificationResponse> CreateAsync(CreateNotificationRequest request, CancellationToken ct);
    Task MarkReadAsync(int id, int userId, CancellationToken ct);
    Task MarkAllReadAsync(int userId, CancellationToken ct);
    Task DeleteAsync(int id, int userId, CancellationToken ct);
}
