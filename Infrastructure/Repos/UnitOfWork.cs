using Application.Interfaces.ReposInterfaces;
using Infrastructure.Data;

namespace Infrastructure.Repos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext dbContext;
        public IUserRelationshipRepo UserRelationships { get; private set; }

        public UnitOfWork(AppDbContext dbContext, IUserRelationshipRepo userRelationshipRepo)
        {
            this.dbContext = dbContext;
            this.UserRelationships = userRelationshipRepo;
        }

        public async Task CommitAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
