using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Entities.JobCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Entities.Accounts
{
    /// <summary>
    /// Bảng Employee - Lưu thông tin chi tiết của nhân viên trong gara
    /// </summary>
    public class 
        Employee : AuditableEntity
    {
        public int EmployeeId { get; set; }

        // Liên kết với tài khoản hệ thống (User)
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        // Thông tin cá nhân
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Tên đầy đủ (có thể được tính toán hoặc lưu riêng để search nhanh)
        public string FullName => $"{FirstName} {LastName}".Trim();


        public string? EmployeeCode { get; set; } // Mã nhân viên (NV001, THU-001, ...)

        // Chức vụ / Vị trí công việc
        public string? Position { get; set; } // Ví dụ: "Kỹ thuật viên", "Lễ tân", "Quản lý gara", "Kế toán"


        // Trạng thái hoạt động
        public bool IsActive { get; set; } = true;

        // Thời gian làm việc (có thể mở rộng sau)
        public DateTime? StartWorkingDate { get; set; } 

        // Audit fields bổ sung navigation cho CreatedBy/UpdatedBy/DeletedBy
        public User? CreatedByUser { get; set; }
        public User? UpdatedByUser { get; set; }
        public User? DeletedByUser { get; set; }

        // Navigation properties
        /// <summary>
        /// Các phiếu sửa chữa mà nhân viên này được phân công làm thợ máy
        /// </summary>
        public ICollection<JobCardMechanic> AssignedJobCards { get; set; } = new List<JobCardMechanic>();
        public ICollection<JobCard> CreatedJobCards { get; set; } = new List<JobCard>();


        /// <summary>
        /// Các phiếu sửa chữa mà lễ tân tạo ra 
        /// </summary>
       // public ICollection<JobCard> CreatedJobCards { get; set; } = new List<JobCard>();

        /// <summary>
        /// Các phiếu sửa chữa mà supervisor giám sát
        /// </summary>
        public ICollection<JobCard> SupervisedJobCards { get; set; } = new List<JobCard>();

        /// <summary>
        /// Lịch hẹn do nhân viên tạo/tiếp nhận
        /// </summary>
        public ICollection<Appointment> CreatedAppointments { get; set; } = new List<Appointment>();
    }
}
