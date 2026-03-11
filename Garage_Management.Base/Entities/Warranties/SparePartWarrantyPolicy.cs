using Garage_Management.Base.Common.Base;

namespace Garage_Management.Base.Entities.Warranties
{
    /// <summary>
    /// Bảng SparePartWarrantyPolicy - Chính sách bảo hành áp dụng cho phụ tùng
    /// </summary>
    public class SparePartWarrantyPolicy : AuditableEntity
    {
        public int PolicyId { get; set; }

        /// <summary>
        /// Tên chính sách bảo hành
        /// </summary>
        public string PolicyName { get; set; } = string.Empty;

        /// <summary>
        /// Thời gian bảo hành (tháng)
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
        /// Các bảo hành phụ tùng áp dụng chính sách này
        /// </summary>
        public ICollection<WarrantySparePart> WarrantySpareParts { get; set; } = new List<WarrantySparePart>();
    }
}
