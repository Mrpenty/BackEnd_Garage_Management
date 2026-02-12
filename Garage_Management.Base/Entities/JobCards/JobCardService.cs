using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garage_Management.Base.Entities.JobCards
{
    /// <summary>
    /// Bảng trung gian: JobCardService - Dòng dịch vụ được thực hiện trong một phiếu sửa chữa (JobCard)
    /// </summary>
    public class JobCardService : AuditableEntity
    {
        public int JobCardServiceId { get; set; }
        public int JobCardId { get; set; }
        public JobCard JobCard { get; set; } = null!;

        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        /// <summary>
        /// Mô tả cụ thể trong phiếu (có thể khác mô tả chung của Service)
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Giá thực tế áp dụng cho dịch vụ này trong phiếu
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// Trạng thái của dịch vụ trong phiếu
        /// </summary>
        public ServiceStatus Status { get; set; } = ServiceStatus.Pending;

        public int? SourceInspectionItemId { get; set; }
        /// <summary>
        /// các công việc cụ thể được thực hiện trong dịch vụ này
        /// </summary>
        public ICollection<JobCardServiceTask> ServiceTasks { get; set; } = new List<JobCardServiceTask>();
    }
}