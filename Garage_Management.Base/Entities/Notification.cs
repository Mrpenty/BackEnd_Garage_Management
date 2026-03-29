using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Accounts;
using System;

namespace Garage_Management.Base.Entities
{
    /// <summary>
    /// Bảng Notification - Lưu thông báo gửi tới người dùng trong hệ thống
    /// </summary>
    public class Notification : AuditableEntity
    {
   
        public int NotificationId { get; set; }

        /// <summary>
        /// Người dùng nhận thông báo
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Tài khoản người dùng nhận thông báo
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Loại thông báo
        /// </summary>
        public NotificationType Type { get; set; } = NotificationType.General;

        /// <summary>
        /// Nội dung thông báo
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Cờ đánh dấu đã đọc hay chưa
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Thời điểm người dùng đọc thông báo (nếu có)
        /// </summary>
        public DateTime? ReadAt { get; set; }

        /// <summary>
        /// kênh gửi thông báo (InApp, Email, SMS)
        /// </summary>
        public string Channel { get; set; } = string.Empty; 
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending; 
    }
}


