using Application.Interfaces.ReposInterfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos
{
    public class ConversationMemberRepo : Repo<ConversationMember>, IConversationMemberRepo
    {
        private readonly AppDbContext dbContext;

        public ConversationMemberRepo(AppDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Conversation>> GetConversationsForUserAsync(string userId)
        {
            var conversations = await dbContext.ConversationMembers
                .Where(cm => cm.UserId == userId)
                .Include(cm => cm.Conversation)
                    .ThenInclude(c => c.Members)
                .Select(cm => cm.Conversation)
                .ToListAsync();

            return conversations;
        }
    }
}
