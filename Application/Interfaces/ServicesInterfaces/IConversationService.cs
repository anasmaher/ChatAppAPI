using Application.DTOs.ConversationDTOs;
using Application.DTOs.ResultsDTOs;

namespace Application.Interfaces.ServicesInterfaces
{
    public interface IConversationService
    {
        Task<ServiceResult> SendMessageAsync(string senderId, SendMessageDTO model);

        Task<ServiceResult> GetMessagesForConversationAsync(Guid conversationId);

        Task<ServiceResult> GetOrCreateConversationAsync(string userId1, string userId2);
    }
}
