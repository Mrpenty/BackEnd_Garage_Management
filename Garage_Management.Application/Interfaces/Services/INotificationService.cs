using Garage_Management.Application.DTOs.Notifications;
using Garage_Management.Base.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<ApiResponse<NotificationResponse>> CreateAndSendNotificationAsync(CreateNotificationRequest request, CancellationToken ct = default);

        Task<ApiResponse<List<NotificationResponse>>> GetUserNotificationsAsync(CancellationToken ct = default);
    }
}
