using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Entities.JobCards;
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
    public class User : AuditableEntity
    {
       
        public int UserId { get; set; }

      
        public string Username { get; set; } = string.Empty;

        
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Địa chỉ email dùng cho tài khoản 
        /// </summary>
       
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Số điện thoại liên hệ cho tài khoản
        /// </summary>
   
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Vai trò (Role) của người dùng trong hệ thống
        /// </summary>
        public int RoleId { get; set; }
      
        public Role Role { get; set; } = null!;

        /// <summary>
        /// Cờ đánh dấu tài khoản đang hoạt động hay đã bị khóa
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation properties
        /// <summary>
        /// Các thông báo mà người dùng nhận được
        /// </summary>
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        /// <summary>
        /// Các phiếu sửa chữa do người dùng này tạo
        /// </summary>
        public ICollection<JobCard> CreatedJobCards { get; set; } = new List<JobCard>();

        /// <summary>
        /// Các phiếu sửa chữa do người dùng này cập nhật
        /// </summary>
        public ICollection<JobCard> UpdatedJobCards { get; set; } = new List<JobCard>();

        /// <summary>
        /// Các phân công thợ máy mà người dùng này là thợ được gán
        /// </summary>
        public ICollection<JobCardMechanic> AssignedMechanics { get; set; } = new List<JobCardMechanic>();
    }
}
