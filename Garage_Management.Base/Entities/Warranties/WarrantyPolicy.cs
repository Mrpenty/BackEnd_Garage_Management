using Garage_Management.Base.Common.Base;
using System;
using System.Collections.Generic;

namespace Garage_Management.Base.Entities.Warranties
{
    /// <summary>
    /// Bảng WarrantyPolicy - Chính sách bảo hành chung (cho dịch vụ hoặc phụ tùng)
    /// </summary>
    public class WarrantyPolicy : AuditableEntity
    {
       
        public int WarrantyPolicyId { get; set; }

        /// <summary>
        /// Tên / tiêu đề chính sách bảo hành
        /// </summary>
        public string PolicyName { get; set; } = string.Empty;

        /// <summary>
        /// Số tháng bảo hành
        /// </summary>
        public int WarrantyMonths { get; set; }

        /// <summary>
        /// Điều khoản và điều kiện bảo hành
        /// </summary>
        public string? TermsAndConditions { get; set; }

        /// <summary>
        /// Các bảo hành dịch vụ áp dụng chính sách này
        /// </summary>
        public ICollection<WarrantyService> WarrantyServices { get; set; } = new List<WarrantyService>();

        /// <summary>
        /// Các bảo hành phụ tùng áp dụng chính sách này
        /// </summary>
        public ICollection<WarrantySparePart> WarrantySpareParts { get; set; } = new List<WarrantySparePart>();
    }
}


