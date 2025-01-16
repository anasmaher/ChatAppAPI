using Application.DTOs.FriendshipDTOs;
using Application.Interfaces.ReposInterfaces;
using Application.Interfaces.ServicesInterfaces;

namespace Application.Services
{
    public class FriendService : IFriendService
    {
        private readonly IUnitOfWork unitOfWork;

        public FriendService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Task BlockUserAsync(string userId, string blockedUserId)
        {
            throw new NotImplementedException();
        }

        public Task<List<FriendRequestDTO>> GetFriendRequestsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<FriendDTO>> GetFriendsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFriendAsync(string userId, string friendUserId)
        {
            throw new NotImplementedException();
        }

        public Task RespondToFriendRequestAsync(int requestId, string responderUserId, string action)
        {
            throw new NotImplementedException();
        }

        public Task SendFriendRequestAsync(string senderUserId, string recipientUserId)
        {
            throw new NotImplementedException();
        }

        public Task UnblockUserAsync(string userId, string blockedUserId)
        {
            throw new NotImplementedException();
        }
    }
}
