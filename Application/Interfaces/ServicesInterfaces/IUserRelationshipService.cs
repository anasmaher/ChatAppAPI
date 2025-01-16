using Application.DTOs.RelationshipDTOs;
using Application.DTOs.ResultsDTOs;
using Domain.Entities;

namespace Application.Interfaces.ServicesInterfaces
{
    public interface IUserRelationshipService
    {
        Task<ServiceResult> SendFriendRequestAsync(string senderUserId, string recipientUserId);

        Task<ServiceResult> RespondToFriendRequestAsync(int requestId, string responderUserId, string action);

        Task<ServiceResult> GetFriendRequestsAsync(string userId);

        Task<ServiceResult> GetFriendsAsync(string userId);

        Task<ServiceResult> RemoveFriendAsync(string userId, string friendUserId);

        Task<ServiceResult> BlockUserAsync(string userId, string blockedUserId);

        Task<ServiceResult> UnblockUserAsync(string userId, string blockedUserId);

        Task<ServiceResult> GetBlockedUsers(string userId);
    }
}
