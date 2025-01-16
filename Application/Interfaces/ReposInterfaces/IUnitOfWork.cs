namespace Application.Interfaces.ReposInterfaces
{
    public interface IUnitOfWork
    {
        IUserRelationshipRepo UserRelationships { get; }

        Task CommitAsync();
    }
}
