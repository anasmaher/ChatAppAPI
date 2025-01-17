namespace Application.Interfaces.ReposInterfaces
{
    public interface IUnitOfWork
    {
        IUserRelationshipRepo UserRelationships { get; }
        INotificationRepo NotificationRepo { get; }

        Task CommitAsync();
    }
}
