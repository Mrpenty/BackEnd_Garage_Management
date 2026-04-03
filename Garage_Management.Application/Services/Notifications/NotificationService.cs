using Garage_Management.Application.DTOs.Notifications;
using Garage_Management.Application.DTOs.Vehicles;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Application.Interfaces.Services.Accounts;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities;
using Garage_Management.Base.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly IUserRepository _userRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISmsService _smsService;
        public NotificationService(INotificationRepository notificationRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, ISmsService smsService )
        {
            _notificationRepo = notificationRepository;
            _userRepo = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _smsService = smsService;
        }
        public async Task<ApiResponse<NotificationResponse>> CreateAndSendNotificationAsync(CreateNotificationRequest request, CancellationToken ct = default)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return ApiResponse<NotificationResponse>.ErrorResponse("Vui lòng đăng nhập");

            }
            var currentUserId = int.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
           
            // Validate
            if (string.IsNullOrWhiteSpace(request.Message) || string.IsNullOrWhiteSpace(request.Channel))
                return ApiResponse<NotificationResponse>.ErrorResponse("Thiếu thông tin bắt buộc");

            //Xử lý recipient → tạo nhiều notification nếu cần
           var notificationsToCreate = new List<Notification>();

            if (request.RecipientType == "SpecificUser" && request.SpecificUserId.HasValue)
            {
                notificationsToCreate.Add(CreateNotificationEntity(request, request.SpecificUserId.Value));
            }
            else if (request.RecipientType == "AllCustomers")
            {
                var customers = await _userRepo.GetCustomersAsync(ct);
                notificationsToCreate.AddRange(customers.Select(u => CreateNotificationEntity(request, u.Id)));
            }
            else if (request.RecipientType == "AllStaff")
            {
                var staff = await _userRepo.GetEmployeesAsync(ct);
                notificationsToCreate.AddRange(staff.Select(u => CreateNotificationEntity(request, u.Id)));
            }
            else
            {
                return ApiResponse<NotificationResponse>.ErrorResponse("Loại người nhận không hợp lệ");
            }

            if (!notificationsToCreate.Any())
                return ApiResponse<NotificationResponse>.ErrorResponse("Không tìm thấy người nhận");

            _notificationRepo.AddRange(notificationsToCreate);
            await _notificationRepo.SaveAsync(ct);

            // Cập nhật trạng thái sau khi "gửi" (hiện tại tạm đánh dấu Sent)
            foreach (var notif in notificationsToCreate)
            {
                notif.Status = NotificationStatus.Sent;
            }
            await _notificationRepo.SaveAsync(ct);

            var firstNotif = notificationsToCreate.First();

            var response = new NotificationResponse
            {
                NotificationId = firstNotif.NotificationId,
                NotificationTypeName = firstNotif.Type.ToString(),
                NotificationStatusName = firstNotif.Status.ToString(),
                Message = request.Message,
                Channel = request.Channel,
                Status = NotificationStatus.Sent
            };

            return ApiResponse<NotificationResponse>.SuccessResponse(response, "Thông báo đã tạo thành công");
        }
        public async Task<ApiResponse<List<NotificationResponse>>> GetUserNotificationsAsync(CancellationToken ct = default)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return ApiResponse<List<NotificationResponse>>.ErrorResponse("Vui lòng đăng nhập");
            }
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var currentUserId))
            {
                return ApiResponse<List<NotificationResponse>>.ErrorResponse("Không thể xác định thông tin người dùng");
            }

            var notifications = await _notificationRepo.GetUserNotificationsAsync(currentUserId, ct);
            var response = notifications.Select(n => new NotificationResponse
            {
                NotificationId = n.NotificationId,
                NotificationTypeName = n.Type.ToString(),
                Message = n.Message,
                Channel = n.Channel,
                Status = n.Status,
                NotificationStatusName = n.Status.ToString()
            }).ToList();

            return ApiResponse<List<NotificationResponse>>.SuccessResponse(response, "Thông báo đã được lấy thành công");
        }
        private Notification CreateNotificationEntity(CreateNotificationRequest req, int userId)
        {
            return new Notification
            {
                UserId = userId,
               // Type = req.Type,
                Message = req.Message,
                Channel = req.Channel,                //Status = "Pending",
                //CreatedBy = "System/Admin", // hoặc currentUserId
                CreatedAt = DateTime.UtcNow
            };
        }

    }
}
