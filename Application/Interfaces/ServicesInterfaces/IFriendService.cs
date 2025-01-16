using Application.DTOs.FriendshipDTOs;

namespace Application.Interfaces.ServicesInterfaces
{
    public interface IFriendService
    {
        Task SendFriendRequestAsync(string senderUserId, string recipientUserId);

        Task RespondToFriendRequestAsync(int requestId, string responderUserId, string action);

        Task<List<FriendRequestDTO>> GetFriendRequestsAsync(string userId);

        Task<List<FriendDTO>> GetFriendsAsync(string userId);

        Task RemoveFriendAsync(string userId, string friendUserId);

        Task BlockUserAsync(string userId, string blockedUserId);

        Task UnblockUserAsync(string userId, string blockedUserId);
    }
}
