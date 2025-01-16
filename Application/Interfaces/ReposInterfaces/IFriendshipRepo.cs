using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Interfaces.ReposInterfaces
{
    public interface IFriendshipRepo : IRepo<Friendship>
    {
        Task<Friendship> GetFriendshipAsync(string userId1, string userId2);

        Task<List<Friendship>> GetFriendRequestsAsync(string userId);

        Task<List<Friendship>> GetFriendsAsync(string userId);
    }
}
