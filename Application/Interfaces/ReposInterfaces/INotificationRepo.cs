using Domain.Entities;

namespace Application.Interfaces.ReposInterfaces
{
    public interface INotificationRepo : IRepo<Notification>
    {
        Task<List<Notification>> GetUnreadNotificationsAsync(string userId);

        Task<List<Notification>> GetNotificationsAsync(string userId, int pageNumber, int pageSize);

        Task<int> GetUnreadCountAsync(string userId);
    }
}
