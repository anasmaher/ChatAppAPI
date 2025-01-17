using Application.DTOs.ResultsDTOs;

namespace Application.Interfaces.ServicesInterfaces
{
    public interface INotificationService
    {
        Task<ServiceResult> GetUnreadNotificationsAsync(string userId);

        Task<ServiceResult> GetNotificationsAsync(string userId, int pageNumber, int pageSize);

        Task<ServiceResult> GetUnreadCountAsync(string userId);

        Task<ServiceResult> MarkAsReadAsync(int notificationId, string userId);

        Task<ServiceResult> MarkAllAsReadAsync(string userId);

        Task SendFriendRequestNotificationAsync(string recipientUserId, string senderUserId);

        Task SendFriendRequestAcceptedNotificationAsync(string recipientUserId, string senderUserId);
    }
}
