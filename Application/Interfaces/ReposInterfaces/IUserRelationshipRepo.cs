using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Interfaces.ReposInterfaces
{
    public interface IUserRelationshipRepo : IRepo<UserRelationship>
    {
        Task<UserRelationship> GetFriendshipAsync(string userId1, string userId2);

        Task<List<UserRelationship>> GetFriendRequestsAsync(string userId, int pageNubmer, int pageSize);

        Task<List<UserRelationship>> GetFriendsAsync(string userId, int pageNubmer, int pageSize);

        Task<List<UserRelationship>> GetBlockedUsersAsync(string userId, int pageNubmer, int pageSize);
    }
}
