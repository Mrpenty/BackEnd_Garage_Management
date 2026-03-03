using Garage_Management.Base.Common.Base;

namespace Garage_Management.Base.Entities.Services
{
    /// <summary>
    /// Bảng ServiceWarrantyPolicy - Chính sách bảo hành áp dụng cho dịch vụ hoặc công việc
    /// </summary>
    public class ServiceWarrantyPolicy : AuditableEntity
    {
        public int PolicyId { get; set; }

        /// <summary>
        /// Tên chính sách bảo hành 
        /// </summary>
        public string PolicyName { get; set; } = string.Empty;

        /// <summary>
        /// Thời gian bảo hành tính bằng tháng
        /// </summary>
        public int? DurationMonths { get; set; }

        /// <summary>
        /// Giới hạn số km bảo hành
        /// </summary>
        public int? MileageLimit { get; set; }

        /// <summary>
        /// Điều kiện áp dụng bảo hành
        /// </summary>
        public string? TermsAndConditions { get; set; }

        /// <summary>
        /// Các dịch vụ sử dụng chính sách bảo hành này
        /// </summary>
        public ICollection<Service> Services { get; set; } = new List<Service>();

        /// <summary>
        /// Các bảo hành dịch vụ áp dụng chính sách này
        /// </summary>
        public ICollection<Garage_Management.Base.Entities.Warranties.WarrantyService> WarrantyServices { get; set; } = new List<Garage_Management.Base.Entities.Warranties.WarrantyService>();
    }
}
