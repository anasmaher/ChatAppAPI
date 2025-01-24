using Application.Interfaces.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace ChatAppAPI.Controllers
{
    /// <summary>
    /// Controller responsible for handling notification-related operations.
    /// </summary>
    [Route("api/Notification")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService notificationService;
        private readonly IMemoryCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationController"/> class.
        /// </summary>
        /// <param name="notificationService">Service for handling notifications.</param>
        /// <param name="cache">For caching</param>
        public NotificationController(INotificationService notificationService, IMemoryCache cache)
        {
            this.notificationService = notificationService;
            this.cache = cache;
        }

        /// <summary>
        /// Retrieves the count of unread notifications for the authenticated user.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the count of unread notifications.
        /// Returns <see cref="OkObjectResult"/> with the count if successful,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cacheKey = $"UnreadCount_{userId}";

            if (cache.TryGetValue(cacheKey, out int cachedCount))
                return Ok(cachedCount);
            
            var res = await notificationService.GetUnreadCountAsync(userId);

            if (res.success)
            {
                cache.Set(cacheKey, res.data, TimeSpan.FromMinutes(5));
                return Ok(res.data);
            }

            return BadRequest(res.Errors);
        }

        /// <summary>
        /// Retrieves all unread notifications for the authenticated user.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a list of unread notifications.
        /// Returns <see cref="OkObjectResult"/> with the list if successful,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cacheKey = $"UnreadNotifications_{userId}";

            if (cache.TryGetValue(cacheKey, out var cachedNotifications))
                return Ok(cachedNotifications);
            
            var res = await notificationService.GetUnreadNotificationsAsync(userId);

            if (res.success)
            {
                cache.Set(cacheKey, res.data, TimeSpan.FromMinutes(5));
                return Ok(res.data);
            }

            return BadRequest(res.Errors);
        }

        /// <summary>
        /// Retrieves all notifications for the authenticated user, with pagination.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve. Default is 1.</param>
        /// <param name="pageSize">The number of notifications per page. Default is 10.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a paginated list of notifications.
        /// Returns <see cref="OkObjectResult"/> with the list if successful,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllNotifications(int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cacheKey = $"AllNotifications_{userId}_Page{pageNumber}_Size{pageSize}";

            if (cache.TryGetValue(cacheKey, out var cachedNotifications))
                return Ok(cachedNotifications);
            
            var res = await notificationService.GetNotificationsAsync(userId, pageNumber, pageSize);

            if (res.success)
            {
                cache.Set(cacheKey, res.data, TimeSpan.FromMinutes(5));
                return Ok(res.data);
            }

            return BadRequest(res.Errors);
        }

        /// <summary>
        /// Marks a specific notification as read.
        /// </summary>
        /// <param name="id">The unique identifier of the notification to mark as read.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="OkObjectResult"/> if successful,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpPost("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await notificationService.MarkAsReadAsync(id, userId);

            if (res.success)
            {
                var unreadCountCacheKey = $"UnreadCount_{userId}";
                var unreadNotificationsCacheKey = $"UnreadNotifications_{userId}";

                // Remove specific caches
                cache.Remove(unreadCountCacheKey);
                cache.Remove(unreadNotificationsCacheKey);

                return Ok(res.data);
            }

            return BadRequest(res.Errors);
        }

        /// <summary>
        /// Marks all notifications as read for the authenticated user.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="OkObjectResult"/> if successful,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpPost("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await notificationService.MarkAllAsReadAsync(userId);

            if (res.success)
            {
                // Inline cache invalidation
                var unreadCountCacheKey = $"UnreadCount_{userId}";
                var unreadNotificationsCacheKey = $"UnreadNotifications_{userId}";

                // Remove specific caches
                cache.Remove(unreadCountCacheKey);
                cache.Remove(unreadNotificationsCacheKey);

                return Ok(res.data);
            }

            return BadRequest(res.Errors);
        }
    }
}