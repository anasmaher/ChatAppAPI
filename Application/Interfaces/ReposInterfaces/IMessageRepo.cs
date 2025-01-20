using Domain.Entities;

namespace Application.Interfaces.ReposInterfaces
{
    public interface IMessageRepo : IRepo<Message>
    {
        Task<List<Message>> GetMessagesForConversationAsync(Guid conversationId);
    }
}
