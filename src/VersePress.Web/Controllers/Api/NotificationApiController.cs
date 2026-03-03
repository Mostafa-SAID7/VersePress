using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VersePress.Application.Interfaces;

namespace VersePress.Web.Controllers.Api;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationApiController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationApiController> _logger;

    public NotificationApiController(
        INotificationService notificationService,
        ILogger<NotificationApiController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] bool unreadOnly = false)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var notifications = await _notificationService.GetUserNotificationsAsync(userId, unreadOnly);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications");
            return StatusCode(500, new { error = "An error occurred while getting notifications" });
        }
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        try
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read");
            return StatusCode(500, new { error = "An error occurred while marking notification as read" });
        }
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count");
            return StatusCode(500, new { error = "An error occurred while getting unread count" });
        }
    }
}
