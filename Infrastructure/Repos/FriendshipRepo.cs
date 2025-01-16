using Application.Interfaces.ReposInterfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Utilities;

namespace Infrastructure.Repos
{
    public class FriendshipRepo : Repo<Friendship>, IFriendshipRepo
    {
        private readonly AppDbContext dbContext;

        public FriendshipRepo(AppDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Friendship>> GetFriendRequestsAsync(string userId)
        {
            var friendRequests = await dbContext.Friendships
                .Where(f => f.User2Id == userId && f.Status == FriendshipStatusEnum.Pending)
                .ToListAsync();

            return friendRequests;
        }

        public async Task<List<Friendship>> GetFriendsAsync(string userId)
        {
            var friends = await dbContext.Friendships
                .Where(f => f.User1Id == userId || f.User2Id == userId && f.Status == FriendshipStatusEnum.Accepted)
                .ToListAsync();

            return friends;
        }

        public async Task<Friendship> GetFriendshipAsync(string userId1, string userId2)
        {
            var (orderedUserId1, OrderedUserId2) = UserHelpers.GetOrderedUserIds(userId1, userId2);

            var friendship = await dbContext.Friendships
                .FirstOrDefaultAsync(f => f.User1Id == orderedUserId1 && f.User2Id == OrderedUserId2);

            return friendship;
        }
    }
}
