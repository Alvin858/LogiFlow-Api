using LogiFlow.Application.DTOs.Notifications;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LogiFlow.Application.Services;

public class NotificationService(IApplicationDbContext db) : INotificationService
{
    public async Task<IReadOnlyCollection<NotificationResponse>> GetAllAsync(int userId, CancellationToken ct) =>
        await db.Notifications.AsNoTracking().Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAtUtc)
            .Select(MapExpression()).ToListAsync(ct);

    public async Task<IReadOnlyCollection<NotificationResponse>> GetUnreadAsync(int userId, CancellationToken ct) =>
        await db.Notifications.AsNoTracking().Where(x => x.UserId == userId && !x.IsRead).OrderByDescending(x => x.CreatedAtUtc)
            .Select(MapExpression()).ToListAsync(ct);

    public async Task<NotificationResponse> CreateAsync(CreateNotificationRequest request, CancellationToken ct)
    {
        if (!await db.Users.AnyAsync(x => x.Id == request.UserId, ct)) throw new NotFoundException("User not found.");
        var notification = new Notification { UserId = request.UserId, Title = request.Title.Trim(), Message = request.Message.Trim(), Type = request.Type.Trim(), CreatedAtUtc = DateTime.UtcNow };
        db.Notifications.Add(notification);
        await db.SaveChangesAsync(ct);
        return Map(notification);
    }

    public async Task MarkReadAsync(int id, int userId, CancellationToken ct)
    {
        var notification = await db.Notifications.SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct) ?? throw new NotFoundException("Notification not found.");
        notification.IsRead = true;
        await db.SaveChangesAsync(ct);
    }

    public async Task MarkAllReadAsync(int userId, CancellationToken ct)
    {
        var notifications = await db.Notifications.Where(x => x.UserId == userId && !x.IsRead).ToListAsync(ct);
        foreach (var notification in notifications) notification.IsRead = true;
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, int userId, CancellationToken ct)
    {
        var notification = await db.Notifications.SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct) ?? throw new NotFoundException("Notification not found.");
        db.Notifications.Remove(notification);
        await db.SaveChangesAsync(ct);
    }

    private static NotificationResponse Map(Notification x) => new(x.Id, x.UserId, x.Title, x.Message, x.Type, x.IsRead, x.CreatedAtUtc);
    private static System.Linq.Expressions.Expression<Func<Notification, NotificationResponse>> MapExpression() => x => new NotificationResponse(x.Id, x.UserId, x.Title, x.Message, x.Type, x.IsRead, x.CreatedAtUtc);
}
