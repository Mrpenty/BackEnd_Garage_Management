using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities;
using Garage_Management.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Repositories
{
    public interface INotificationRepository : IBaseRepository<Notification>
    {
        /// <summary>
        /// Lấy danh sách thông báo của người dùng
        /// </summary>
        Task<PagedResult<Notification>> GetNotificationsAsync(int? userId, bool? isRead, int page, int pageSize, CancellationToken ct = default);

        /// <summary>
        /// Lấy danh sách thông báo của người dùng
        /// </summary>
        Task<List<Notification>> GetUserNotificationsAsync(int userId, CancellationToken ct = default);

    }
}

