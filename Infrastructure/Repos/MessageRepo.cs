using Application.Interfaces.ReposInterfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos
{
    public class MessageRepo : Repo<Message>, IMessageRepo
    {
        private readonly AppDbContext dbContext;

        public MessageRepo(AppDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Message>> GetMessagesForConversationAsync(Guid conversationId)
        {
            var messages = await dbContext.Messages
                .Where(m => m.ConversationId == conversationId)
                .Include(m => m.Sender)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return messages;
        }
    }
}
