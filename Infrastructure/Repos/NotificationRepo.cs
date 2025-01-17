using Application.Interfaces.ReposInterfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace Infrastructure.Repos
{
    public class NotificationRepo : Repo<Notification>, INotificationRepo
    {
        private readonly AppDbContext dbContext;

        public NotificationRepo(AppDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Notification>> GetNotificationsAsync(string userId, int pageNumber, int pageSize)
        {
            var nots = await dbContext.Notifications.Where(n =>  n.UserId == userId)
                .Skip(pageNumber * (pageSize - 1))
                .Take(pageSize)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            return nots;
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            int cntr = await dbContext.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);

            return cntr;
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            var nots = await dbContext.Notifications.Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            return nots;
        }
    }
}
