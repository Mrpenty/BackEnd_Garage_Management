using System;

namespace Garage_Management.Base.Entities.Warranties
{
    /// <summary>
    /// Bảng WarrantyService - Lưu thông tin bảo hành cho dịch vụ đã thực hiện
    /// </summary>
    public class WarrantyService
    {
      
        public int WarrantyServiceId { get; set; }

        /// <summary>
        /// Dịch vụ được bảo hành
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// Thông tin dịch vụ
        /// </summary>
        public Service Service { get; set; } = null!;

        /// <summary>
        /// Chính sách bảo hành áp dụng
        /// </summary>
        public int WarrantyPolicyId { get; set; }

        /// <summary>
        /// Thông tin chính sách bảo hành
        /// </summary>
        public WarrantyPolicy WarrantyPolicy { get; set; } = null!;

        /// <summary>
        /// Ngày bắt đầu hiệu lực bảo hành
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày kết thúc hiệu lực bảo hành
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Mô tả / ghi chú thêm
        /// </summary>
        public string? Description { get; set; }
    }
}


