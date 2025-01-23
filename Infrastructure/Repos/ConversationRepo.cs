using Application.Interfaces.ReposInterfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos
{
    public class ConversationRepo : Repo<Conversation>, IConversationRepo
    {
        private readonly AppDbContext dbContext;

        public ConversationRepo(AppDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Conversation>> GetAllAsync(string userId)
        {
            return await dbContext.Conversations
                .Include(c => c.Members)
                .Where(c => c.Members.Any(cm => cm.UserId == userId))
                .ToListAsync();
        }

        public async Task<Conversation> GetPrivateConversationAsync(string userId1, string userId2)
        {
            if (string.IsNullOrEmpty(userId1) || string.IsNullOrEmpty(userId2))
                return null;

            var conversation = await dbContext.Conversations
                .Include(c => c.Members)
                .Where(c => c.Members.Any(m => m.UserId == userId1) && c.Members.Any(m => m.UserId == userId2))
                .FirstOrDefaultAsync();

            return conversation;
        }
    }
}
