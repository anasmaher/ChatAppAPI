using Application.Interfaces.ServicesInterfaces;
using ChatAppAPI.ViewModels.UserVMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatAppAPI.Controllers
{
    [Route("api/Relationship")]
    [ApiController]
    [Authorize]
    public class RelationshipController : ControllerBase
    {
        private readonly IUserRelationshipService relationshipService;

        public RelationshipController(IUserRelationshipService relationshipService)
        {
            this.relationshipService = relationshipService;
        }

        [HttpPost("send-request/{reciptientId}")]
        public async Task<IActionResult> SendRequest(string reciptientId)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await relationshipService.SendFriendRequestAsync(senderId, reciptientId);

            if (!res.success)
                return BadRequest(res.data);

            return Ok(res.data);
        }

        [HttpPost("respond-request/{requestId}")]
        public async Task<IActionResult> RespondToRequest(int requestId, [FromBody] string action)
        {
            var responderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await relationshipService.RespondToFriendRequestAsync(requestId, responderId, action);

            if (!res.success)
                return BadRequest(res.data);

            return Ok(res.data);
        }

        [HttpPost("block-user/{blockedUserId}")]
        public async Task<IActionResult> BlockUser(string blockedUserId)
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await relationshipService.BlockUserAsync(CurrentUserId, blockedUserId);

            if (!res.success)
                return BadRequest(res.data);

            return Ok(res.data);
        }

        [HttpPost("unblock-user/{unblockedUserId}")]
        public async Task<IActionResult> UnblockUser(string unblockedUserId)
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await relationshipService.UnblockUserAsync(CurrentUserId, unblockedUserId);

            if (!res.success)
                return BadRequest(res.data);

            return Ok(res.data);
        }

        [HttpPost("remove-friend/{friendId}")]
        public async Task<IActionResult> RespondToRequest(string friendId)
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await relationshipService.RemoveFriendAsync(CurrentUserId, friendId);

            if (!res.success)
                return BadRequest(res.data);

            return Ok(res.data);
        }

        [HttpGet("get-requests")]
        public async Task<IActionResult> GetFriendRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await relationshipService.GetFriendRequestsAsync(userId);

            if (!res.success)
                return BadRequest(res.data);
            
            return Ok(res.data);
        }

        [HttpGet("get-friends")]
        public async Task<IActionResult> GetFriends()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await relationshipService.GetFriendsAsync(userId);

            if (!res.success)
                return BadRequest(res.data);

            return Ok(res.data);
        }

        [HttpGet("get-blocked-users")]
        public async Task<IActionResult> GetBlockedUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var res = await relationshipService.GetBlockedUsers(userId);

            if (!res.success)
                return BadRequest(res.data);

            return Ok(res.data);
        }
    }
}
