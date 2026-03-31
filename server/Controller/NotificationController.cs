using Bussiness.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Server.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : 0;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetNotifications([FromQuery] int limit = 50)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var notifications = await _notificationService.GetUserNotificationsAsync(userId, limit);
            var unreadCount = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { notifications, unreadCount });
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { count });
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(long id)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            await _notificationService.MarkAsReadAsync(id, userId);
            return Ok(new { success = true });
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            await _notificationService.DeleteForUserAsync(id, userId);
            return Ok(new { success = true });
        }
    }
}
