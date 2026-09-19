namespace LogiFlow.Application.DTOs.Notifications;

public record CreateNotificationRequest(
    int UserId,
    string Title,
    string Message,
    string Type);

public record NotificationResponse(
    int Id,
    int UserId,
    string Title,
    string Message,
    string Type,
    bool IsRead,
    DateTime CreatedAtUtc);
