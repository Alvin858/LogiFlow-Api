using LogiFlow.API.Extensions;
using LogiFlow.Application.DTOs.Notifications;
using LogiFlow.Application.Interfaces;
using LogiFlow.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogiFlow.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController(INotificationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<NotificationResponse>>> GetAll(CancellationToken ct) => Ok(await service.GetAllAsync(User.GetUserId(), ct));
    [HttpGet("unread")]
    public async Task<ActionResult<IReadOnlyCollection<NotificationResponse>>> GetUnread(CancellationToken ct) => Ok(await service.GetUnreadAsync(User.GetUserId(), ct));
    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<NotificationResponse>> Create(CreateNotificationRequest request, CancellationToken ct) => Ok(await service.CreateAsync(request, ct));
    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> Read(int id, CancellationToken ct) { await service.MarkReadAsync(id, User.GetUserId(), ct); return NoContent(); }
    [HttpPatch("read-all")]
    public async Task<IActionResult> ReadAll(CancellationToken ct) { await service.MarkAllReadAsync(User.GetUserId(), ct); return NoContent(); }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct) { await service.DeleteAsync(id, User.GetUserId(), ct); return NoContent(); }
}
