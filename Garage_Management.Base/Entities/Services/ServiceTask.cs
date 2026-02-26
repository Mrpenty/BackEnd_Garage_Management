using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Entities.JobCards;

namespace Garage_Management.Base.Entities.Services
{
    // <summary>
    /// Bảng ServiceTask - Các công việc / nhiệm vụ chi tiết thuộc một dịch vụ
    /// (VD: Thay lọc dầu, Kiểm tra má phanh, Cân bằng lốp)
    /// </summary>
    public class ServiceTask : AuditableEntity
    {
        public int ServiceTaskId { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        /// <summary>
        /// Tên công việc cụ thể
        /// </summary>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// Thứ tự thực hiện trong dịch vụ (nếu cần sắp xếp)
        /// </summary>
        public int TaskOrder { get; set; } = 0;

        /// <summary>
        /// Thời gian ước tính thực hiện công việc này (phút)
        /// </summary>
        public int EstimateMinute { get; set; }

        /// <summary>
        /// Ghi chú hoặc hướng dẫn thực hiện công việc
        /// </summary>
        public string? Note { get; set; }
        /// <summary>
        /// Các dòng công việc được sử dụng trong phiếu sửa chữa
        /// </summary>
        public ICollection<JobCardServiceTask> JobCardServiceTasks { get; set; } = new List<JobCardServiceTask>();
    }
} 