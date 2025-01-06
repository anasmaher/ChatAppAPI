using Application.Interfaces.ReposInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repos
{
    public class Repo<T> : IRepo<T> where T : class
    {
        private readonly AppDbContext dbContext;
        private readonly DbSet<T> dbSet;

        public Repo(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<T>();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<T> AddAsync(T Entity)
        {
            await dbSet.AddAsync(Entity);
            return Entity;
        }

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> filter)
        {
            return await dbSet.FirstOrDefaultAsync(filter);
        }

        public async Task RemoveAsync(Expression<Func<T, bool>> filter)
        {
            var obj = await dbSet.FirstOrDefaultAsync(filter);
            dbSet.Remove(obj);
        }

        public async Task<T> Update(T Entitiy)
        {
            dbSet.Update(Entitiy);
            return Entitiy;
        }
    }
}
