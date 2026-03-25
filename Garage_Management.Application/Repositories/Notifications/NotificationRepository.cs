using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Garage_Management.Application.Repositories.Notifications
{
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<PagedResult<Notification>> GetNotificationsAsync(int? userId, bool? isRead, int page, int pageSize, CancellationToken ct = default)
        {
            var query = this.dbSet.AsQueryable();
            if (userId.HasValue)
            {
                query = query.Where(n => n.UserId == userId.Value);
            }
            if (isRead.HasValue)
            {
                query = query.Where(n => n.IsRead == isRead.Value);
            }
            var totalItems = await query.CountAsync(ct).ConfigureAwait(false);
            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct)
                .ConfigureAwait(false);
           return new PagedResult<Notification>
            {
                Page = page,
                PageSize = pageSize,
                Total = totalItems,
                PageData = notifications
            };
        }
    }
}
