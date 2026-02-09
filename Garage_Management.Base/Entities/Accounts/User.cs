using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Entities.Accounts
{
    /// <summary>
    /// Bảng User - Tài khoản đăng nhập hệ thống cho nhân viên / khách hàng
    /// </summary>
    public class User : IdentityUser<int>
    {
        public string Username { get; set; } = string.Empty;

        ///// <summary>
        ///// Địa chỉ email dùng cho tài khoản 
        ///// </summary>

        //public string Email { get; set; } = string.Empty;

        ///// <summary>
        ///// Số điện thoại liên hệ cho tài khoản
        ///// </summary>

        //public string? PhoneNumber { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        /// <summary>
        /// Vai trò (Role) của người dùng trong hệ thống
        /// </summary>
        public int RoleId { get; set; }
      
        public Role Role { get; set; } = null!;

        /// <summary>
        /// Cờ đánh dấu tài khoản đang hoạt động hay đã bị khóa
        /// </summary>
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// Thời điểm tạo bản ghi.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Người tạo bản ghi (UserId).
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Thời điểm cập nhật gần nhất.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Người cập nhật gần nhất 
        /// </summary>
        public int? UpdatedBy { get; set; }

        /// <summary>
        /// Thời điểm xóa mềm 
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Người thực hiện xóa mềm 
        /// </summary>
        public int? DeletedBy { get; set; }

        // Navigation properties
        /// <summary>
        /// Các thông báo mà người dùng nhận được
        /// </summary>
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }
}
