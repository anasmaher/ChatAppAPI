namespace Application.Interfaces.ReposInterfaces
{
    public interface IUnitOfWork
    {
        IUserRelationshipRepo UserRelationshipRepo { get; }
        INotificationRepo NotificationRepo { get; }

        Task CommitAsync();
    }
}
