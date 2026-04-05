using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.Services;

namespace Garage_Management.Base.Entities.JobCards
{
    /// <summary>
    /// Bảng JobCardServiceTask - Chi tiết các công việc cụ thể được thực hiện trong phiếu sửa chữa
    /// (liên kết giữa JobCard và ServiceTask)
    /// </summary>
    public class JobCardServiceTask
    {
        public int JobCardServiceTaskId { get; set; }

        public int JobCardId { get; set; }
        public JobCard JobCard { get; set; } = null!;

        public int ServiceTaskId { get; set; }
        public ServiceTask ServiceTask { get; set; } = null!;

        /// <summary>
        /// Thứ tự thực hiện trong phiếu (có thể khác với thứ tự mặc định của ServiceTask)
        /// </summary>
        public int TaskOrder { get; set; }

        /// <summary>
        /// Trạng thái của công việc cụ thể này
        /// </summary>
        public ServiceStatus Status { get; set; } = ServiceStatus.Pending;

        /// <summary>
        /// Có phải công việc tùy chọn không (khách hàng có thể chọn/không chọn)
        /// </summary>
        public bool? IsOptional { get; set; } = false;

        /// <summary>
        /// Thời gian bắt đầu thực hiện công việc này
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// Thời gian hoàn thành công việc này
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Ghi chú thực tế khi thực hiện công việc
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// Thợ máy được giao thực hiện công việc này
        /// </summary>
        public int? PerformedById { get; set; }
        public Employee? PerformedBy { get; set; }
        public int? JobCardServiceId { get; set; }
        public JobCardService? JobCardService { get; set; }

    }
}