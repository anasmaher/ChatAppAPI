using Application.Interfaces.ReposInterfaces;
using Infrastructure.Data;

namespace Infrastructure.Repos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext dbContext;
        public IUserRelationshipRepo UserRelationshipRepo { get; private set; }
        public INotificationRepo NotificationRepo { get; private set; }

        public UnitOfWork(AppDbContext dbContext, IUserRelationshipRepo userRelationshipRepo, INotificationRepo NotificationRepo)
        {
            this.dbContext = dbContext;
            this.UserRelationshipRepo = userRelationshipRepo;
            this.NotificationRepo = NotificationRepo;
        }

        public async Task CommitAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
