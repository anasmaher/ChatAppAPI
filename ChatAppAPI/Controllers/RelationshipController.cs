using Application.Interfaces.ServicesInterfaces;
using ChatAppAPI.ViewModels.UserVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MimeKit;
using Org.BouncyCastle.Cms;
using System.Security.Claims;

namespace ChatAppAPI.Controllers
{
    /// <summary>
    /// Controller responsible for handling user relationship operations such as friend requests, blocking, and unblocking users.
    /// </summary>
    [Route("api/Relationship")]
    [ApiController]
    [Authorize]
    public class RelationshipController : ControllerBase
    {
        private readonly IUserRelationshipService relationshipService;
        private readonly IMemoryCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipController"/> class.
        /// </summary>
        /// <param name="relationshipService">Service for handling user relationships.</param>
        /// <param name="cache">For caching</param>
        public RelationshipController(IUserRelationshipService relationshipService, IMemoryCache cache)
        {
            this.relationshipService = relationshipService;
            this.cache = cache;
        }

        /// <summary>
        /// Sends a friend request to the specified recipient.
        /// </summary>
        /// <param name="reciptientId">The user identifier of the recipient.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the operation.
        /// Returns <see cref="OkObjectResult"/> with the resulting data if successful,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpPost("send-request/{reciptientId}")]
        public async Task<IActionResult> SendRequest(string reciptientId)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await relationshipService.SendFriendRequestAsync(senderId, reciptientId);

            if (!res.success)
                return BadRequest(res.data);

            return Ok(res.data);
        }

        /// <summary>
        /// Responds to a friend request with the specified action.
        /// </summary>
        /// <param name="requestId">The identifier of the friend request.</param>
        /// <param name="action">The action to take on the request (e.g., "accept" or "decline").</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the operation.
        /// Returns <see cref="OkObjectResult"/> if the action was performed successfully,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpPost("respond-request/{requestId}")]
        public async Task<IActionResult> RespondToRequest(int requestId, [FromBody] string action)
        {
            var responderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await relationshipService.RespondToFriendRequestAsync(requestId, responderId, action);

            if (!res.success)
                return BadRequest(res.data);

            var cacheKey = $"Friends_{responderId}_Page1_Size10";
            cache.Remove(cacheKey);

            return Ok(res.data);
        }

        /// <summary>
        /// Blocks the specified user.
        /// </summary>
        /// <param name="blockedUserId">The user identifier of the user to be blocked.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the block operation.
        /// Returns <see cref="OkObjectResult"/> if the user was successfully blocked,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpPost("block-user/{blockedUserId}")]
        public async Task<IActionResult> BlockUser(string blockedUserId)
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await relationshipService.BlockUserAsync(CurrentUserId, blockedUserId);

            if (!res.success)
                return BadRequest(res.data);

            var cacheKey = $"Friends_{CurrentUserId}_Page1_Size10";
            cache.Remove(cacheKey);

            cacheKey = $"Friends_{blockedUserId}_Page1_Size10";
            cache.Remove(cacheKey);

            cacheKey = $"BlockedUsers_{CurrentUserId}_Page1_Size10";
            cache.Remove(cacheKey);

            return Ok(res.data);
        }

        /// <summary>
        /// Unblocks the specified user.
        /// </summary>
        /// <param name="unblockedUserId">The user identifier of the user to be unblocked.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the unblock operation.
        /// Returns <see cref="OkObjectResult"/> if the user was successfully unblocked,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpPost("unblock-user/{unblockedUserId}")]
        public async Task<IActionResult> UnblockUser(string unblockedUserId)
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await relationshipService.UnblockUserAsync(CurrentUserId, unblockedUserId);

            if (!res.success)
                return BadRequest(res.data);

            var cacheKey = $"BlockedUsers_{CurrentUserId}_Page1_Size10";
            cache.Remove(cacheKey);

            return Ok(res.data);
        }

        /// <summary>
        /// Removes the specified user from the current user's friends list.
        /// </summary>
        /// <param name="friendId">The user identifier of the friend to remove.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> representing the result of the remove operation.
        /// Returns <see cref="OkObjectResult"/> if the friend was successfully removed,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpPost("remove-friend/{friendId}")]
        public async Task<IActionResult> RemoveFriend(string friendId)
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await relationshipService.RemoveFriendAsync(CurrentUserId, friendId);

            if (!res.success)
                return BadRequest(res.data);

            var cacheKey = $"Friends_{CurrentUserId}_Page1_Size10";
            cache.Remove(cacheKey);

            cacheKey = $"Friends_{friendId}_Page1_Size10";
            cache.Remove(cacheKey);

            return Ok(res.data);
        }

        /// <summary>
        /// Retrieves the list of friend requests for the current user.
        /// </summary>
        /// <param name="pageNumber">The page number for paginated results. Default is 1.</param>
        /// <param name="pageSize">The number of items per page. Default is 10.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the list of friend requests.
        /// Returns <see cref="OkObjectResult"/> with the data if successful,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpGet("get-requests")]
        public async Task<IActionResult> GetFriendRequests(int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await relationshipService.GetFriendRequestsAsync(userId, pageNumber, pageSize);
            if (!res.success)
                return BadRequest(res.data);

            return Ok(res.data);
        }

        /// <summary>
        /// Retrieves the list of friends for the current user.
        /// </summary>
        /// <param name="pageNumber">The page number for paginated results. Default is 1.</param>
        /// <param name="pageSize">The number of items per page. Default is 10.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the list of friends.
        /// Returns <see cref="OkObjectResult"/> with the data if successful,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpGet("get-friends")]
        public async Task<IActionResult> GetFriends(int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cacheKey = $"Friends_{userId}_Page{pageNumber}_Size{pageSize}";

            if (cache.TryGetValue(cacheKey, out var cachedData))
                return Ok(cachedData);

            var res = await relationshipService.GetFriendsAsync(userId, pageNumber, pageSize);
            if (!res.success)
                return BadRequest(res.data);
            

            cache.Set(cacheKey, res.data, TimeSpan.FromMinutes(5));
            return Ok(res.data);
        }

        /// <summary>
        /// Retrieves the list of users blocked by the current user.
        /// </summary>
        /// <param name="pageNumber">The page number for paginated results. Default is 1.</param>
        /// <param name="pageSize">The number of items per page. Default is 10.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the list of blocked users.
        /// Returns <see cref="OkObjectResult"/> with the data if successful,
        /// or <see cref="BadRequestObjectResult"/> with error details if the operation fails.
        /// </returns>
        [HttpGet("get-blocked-users")]
        public async Task<IActionResult> GetBlockedUsers(int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cacheKey = $"BlockedUsers_{userId}_Page{pageNumber}_Size{pageSize}";

            if (cache.TryGetValue(cacheKey, out var cachedData))
                return Ok(cachedData);
            
            var res = await relationshipService.GetBlockedUsers(userId, pageNumber, pageSize);
            if (!res.success)
                return BadRequest(res.data);

            cache.Set(cacheKey, res.data, TimeSpan.FromMinutes(5));
            return Ok(res.data);
        }
    }
}