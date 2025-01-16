using Application.Interfaces.ReposInterfaces;
using Infrastructure.Data;

namespace Infrastructure.Repos
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext dbContext;
        public IFriendshipRepo FriendshipRepo { get; }

        public UnitOfWork(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
            
        }

        public async Task<int> CommitAsync()
        {
            return await dbContext.SaveChangesAsync();
        }
    }
}
