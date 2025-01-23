using Domain.Entities;

namespace Application.Interfaces.ReposInterfaces
{
    public interface IConversationRepo : IRepo<Conversation>
    {
        Task<Conversation> GetPrivateConversationAsync(string userId1, string userId2);

        Task<List<Conversation>> GetAllAsync(string userId);
    }
}
