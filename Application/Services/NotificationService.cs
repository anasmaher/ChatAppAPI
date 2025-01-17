using Application.DTOs.ResultsDTOs;
using Application.DTOs.SignalrDTOs;
using Application.Interfaces.ReposInterfaces;
using Application.Interfaces.ServicesInterfaces;
using AutoMapper;
using ChatAppAPI.Hubs;
using Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IHubContext<ChatHub> hubContext;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<ChatHub> hubContext)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.hubContext = hubContext;
        }

        public async Task<ServiceResult> GetUnreadNotificationsAsync(string userId)
        {
            var nots = await unitOfWork.NotificationRepo.GetUnreadNotificationsAsync(userId);

            var notsDTO = mapper.Map<List<NotificationDTO>>(nots);

            return new ServiceResult(true, data: notsDTO);
        }

        public async Task<ServiceResult> GetNotificationsAsync(string userId, int pageNumber, int pageSize)
        {
            var nots = await unitOfWork.NotificationRepo.GetNotificationsAsync(userId, pageNumber, pageSize);

            var notsDTO = mapper.Map<List<NotificationDTO>>(nots);

            return new ServiceResult(true, data: notsDTO);
        }

        public async Task<ServiceResult> GetUnreadCountAsync(string userId)
        {
            int count = await unitOfWork.NotificationRepo.GetUnreadCountAsync(userId);

            return new ServiceResult(true, data: count);
        }

        public async Task<ServiceResult> MarkAsReadAsync(int notificationId, string userId)
        {
            var notification = await unitOfWork.NotificationRepo.GetSingleAsync(n => n.Id == notificationId);

            if (notification is not null && notification.UserId == userId)
            {
                notification.IsRead = true;
                unitOfWork.NotificationRepo.Update(notification);

                await unitOfWork.CommitAsync();
            }

            return new ServiceResult(true, data: "Notification was marked read");
        }

        public async Task<ServiceResult> MarkAllAsReadAsync(string userId)
        {
            var nots = await unitOfWork.NotificationRepo.GetUnreadNotificationsAsync(userId);

            foreach (var notification in nots)
            {
                notification.IsRead = true;
            }

            await unitOfWork.CommitAsync();

            return new ServiceResult(true, data: "All notifications were marked read");
        }

        public async Task SendFriendRequestNotificationAsync(string recipientUserId, string senderUserId)
        {
            await hubContext.Clients.User(recipientUserId).SendAsync("ReceiveFriendRequest", new
            {
                SenderUserId = senderUserId,
                SentDate = DateTime.UtcNow
            });
        }

        public async Task SendFriendRequestAcceptedNotificationAsync(string recipientUserId, string senderUserId)
        {
            await hubContext.Clients.User(recipientUserId).SendAsync("FriendRequestAccepted", new
            {
                SenderUserId = senderUserId,
                AcceptedDate = DateTime.UtcNow
            });
        }
    }
}
