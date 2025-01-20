namespace Application.Interfaces.ReposInterfaces
{
    public interface IUnitOfWork
    {
        IUserRelationshipRepo UserRelationshipRepo { get; }
        INotificationRepo NotificationRepo { get; }
        IMessageRepo MessageRepo { get; }
        IConversationMemberRepo ConversationMemberRepo { get; }
        IConversationRepo ConversationRepo { get; }

        Task CommitAsync();
    }
}
