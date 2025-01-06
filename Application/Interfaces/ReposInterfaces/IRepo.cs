using System.Linq.Expressions;

namespace Application.Interfaces.ReposInterfaces
{
    public interface IRepo<T> where T : class
    {
        Task<T> GetSingleAsync(Expression<Func<T, bool>> filter);

        Task<T> AddAsync(T Entity);

        Task RemoveAsync(Expression<Func<T, bool>> filter);

        Task<T> Update(T Entity);
    }
}
