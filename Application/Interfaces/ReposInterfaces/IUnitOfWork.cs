namespace Application.Interfaces.ReposInterfaces
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
    }
}
