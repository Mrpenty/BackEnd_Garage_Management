using Garage_Management.Base.Entities.Accounts;
using System;

namespace Garage_Management.Base.Entities.JobCards
{
    /// <summary>
    /// Bảng JobCardLog - Nhật ký thay đổi / hoạt động trên phiếu sửa chữa
    /// </summary>
    public class JobCardLog
    {
        public int JobCardLogId { get; set; }

        /// <summary>
        /// Phiếu sửa chữa liên quan
        /// </summary>
        public int JobCardId { get; set; }

        /// <summary>
        /// Thông tin phiếu sửa chữa
        /// </summary>
        public JobCard JobCard { get; set; } = null!;

        /// <summary>
        /// Thời điểm diễn ra hành động
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Người thực hiện hành động
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Tài khoản người dùng thực hiện
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Mô tả hành động (tạo phiếu, cập nhật trạng thái, thêm dịch vụ, v.v.)
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Ghi chú chi tiết thêm (nếu cần)
        /// </summary>
        public string? Note { get; set; }
    }
}


