using Application.DTOs.ConversationDTOs;
using Application.DTOs.ResultsDTOs;

namespace Application.Interfaces.ServicesInterfaces
{
    public interface IConversationService
    {
        Task<ServiceResult> SendMessageAsync(string senderId, SendMessageDTO model);

        Task<ServiceResult> GetMessagesForConversationAsync(Guid conversationId, string userId);

        Task<ServiceResult> GetOrCreateConversationAsync(string userId1, string userId2);

        Task<ServiceResult> DeleteMessageAsync(Guid convoId, int messageId, string userId);

        Task<ServiceResult> MarkMessageAsReadAsync(int messageId, string userId);

        Task<ServiceResult> EditMessageAsync(EditMessageDTO model);

        Task<ServiceResult> GetAllConversationsAsync(string userId);
    }
}
