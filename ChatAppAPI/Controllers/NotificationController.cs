using Application.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatAppAPI.Controllers
{
    [Route("api/Notification")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService notificationService;

        public NotificationController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await notificationService.GetUnreadCountAsync(userId);

            if (res.success)
                return Ok(res.data);

            return BadRequest(res.Errors);
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await notificationService.GetUnreadNotificationsAsync(userId);

            if (res.success)
                return Ok(res.data);

            return BadRequest(res.Errors);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllNotifications(int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await notificationService.GetNotificationsAsync(userId, pageNumber, pageSize);

            if (res.success)
                return Ok(res.data);

            return BadRequest(res.Errors);
        }

        [HttpPost("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await notificationService.MarkAsReadAsync(id, userId);

            if (res.success)
                return Ok(res.data);

            return BadRequest(res.Errors);
        }

        [HttpPost("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

             var res = await notificationService.MarkAllAsReadAsync(userId);

            if (res.success)
                return Ok(res.data);

            return BadRequest(res.Errors);
        }
    }
}
