using Application.Interfaces.ReposInterfaces;
using Infrastructure.Data;

namespace Infrastructure.Repos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext dbContext;

        public IConversationMemberRepo ConversationMemberRepo { get; }
        public IMessageRepo MessageRepo { get; }
        public IUserRelationshipRepo UserRelationshipRepo { get; }
        public INotificationRepo NotificationRepo { get; }
        public IConversationRepo ConversationRepo { get; }

        public UnitOfWork(AppDbContext dbContext,
            IUserRelationshipRepo userRelationshipRepo,
            INotificationRepo notificationRepo,
            IMessageRepo messageRepo,
            IConversationMemberRepo conversationMemberRepo,
            IConversationRepo conversationRepo
        )
        {
            this.dbContext = dbContext;
            UserRelationshipRepo = userRelationshipRepo;
            NotificationRepo = notificationRepo;
            MessageRepo = messageRepo;
            ConversationMemberRepo = conversationMemberRepo;
            ConversationRepo = conversationRepo;
        }

        public async Task CommitAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
