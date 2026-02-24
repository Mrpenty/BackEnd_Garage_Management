using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Accounts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garage_Management.Base.Entities.JobCards
{
    /// <summary>
    /// Bảng trung gian: JobCardMechanic
    /// </summary>
    public class JobCardMechanic
    {
        public int JobCardId { get; set; }
        public int EmployeeId { get; set; }    

        /// <summary>
        /// Phiếu sửa chữa mà thợ này được phân công
        /// </summary>
        public JobCard JobCard { get; set; } = null!;

        /// <summary>
        /// Thợ máy  được phân công cho phiếu này
        /// </summary>
        public Employee Employee { get; set; } = null!;   


        /// <summary>
        /// Thời điểm phân công thợ vào phiếu
        /// </summary>
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Người phân công (thường là quản lý hoặc lễ tân)
        /// </summary>
        public int? AssignedBy { get; set; }

        public Employee? AssignedByUser { get; set; }

        /// <summary>
        /// Vai trò cụ thể của thợ trong phiếu này (nếu cần phân biệt)
        /// Ví dụ: "Chính", "Hỗ trợ", "Kiểm tra cuối", "Thay thế lốp"...
        /// </summary>
        //public string? RoleInJob { get; set; }

        /// <summary>
        /// Ghi chú riêng cho thợ này trong phiếu (nếu cần)
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// Trạng thái công việc của thợ này trên phiếu
        /// </summary>
        public MechanicAssignmentStatus Status { get; set; } = MechanicAssignmentStatus.Assigned;

        /// <summary>
        /// Thời gian thợ bắt đầu làm việc thực tế (nếu theo dõi chi tiết)
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// Thời gian thợ hoàn thành phần việc của mình
        /// </summary>
        public DateTime? CompletedAt { get; set; }
    }
}
