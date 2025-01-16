using Application.Interfaces.ReposInterfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Utilities;

namespace Infrastructure.Repos
{
    public class UserRelationshipRepo : Repo<UserRelationship>, IUserRelationshipRepo
    {
        private readonly AppDbContext dbContext;

        public UserRelationshipRepo(AppDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<UserRelationship>> GetFriendRequestsAsync(string userId)
        {
            var friendRequests = await dbContext.Friendships
                .Include(f => f.ActionUser)
                .Where(f => f.Status == RelationshipStatusEnum.Pending && f.User2Id == userId)
                .ToListAsync();

            return friendRequests;
        }

        public async Task<List<UserRelationship>> GetFriendsAsync(string userId)
        {
            var friends = await dbContext.Friendships
                .Where(f => f.Status == RelationshipStatusEnum.Accepted && (f.User1Id == userId || f.User2Id == userId))
                .ToListAsync();

            return friends;
        }

        public async Task<UserRelationship> GetFriendshipAsync(string userId1, string userId2)
        {
            var (orderedUserId1, OrderedUserId2) = UserHelpers.GetOrderedUserIds(userId1, userId2);

            var friendship = await dbContext.Friendships
                .FirstOrDefaultAsync(f => f.User1Id == orderedUserId1 && f.User2Id == OrderedUserId2);

            return friendship;
        }

        public async Task<List<UserRelationship>> GetBlockedUsersAsync(string userId)
        {
            var blockedUsers = await dbContext.Friendships
                .Where(f => f.Status == RelationshipStatusEnum.Blocked && (f.User1Id == userId || f.User2Id == userId))
                .ToListAsync();

            return blockedUsers;
        }
    }
}
