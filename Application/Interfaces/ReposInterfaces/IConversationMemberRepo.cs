using Domain.Entities;

namespace Application.Interfaces.ReposInterfaces
{
    public interface IConversationMemberRepo : IRepo<ConversationMember>
    {
        Task<List<Conversation>> GetConversationsForUserAsync(string userId);
    }
}
