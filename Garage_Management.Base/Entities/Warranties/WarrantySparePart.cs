using Garage_Management.Base.Entities.Inventories;
using System;

namespace Garage_Management.Base.Entities.Warranties
{
    /// <summary>
    /// Bảng WarrantySparePart - Lưu thông tin bảo hành cho phụ tùng đã sử dụng
    /// </summary>
    public class WarrantySparePart
    {
        public int WarrantySparePartId { get; set; }

        /// <summary>
        /// Phụ tùng được bảo hành
        /// </summary>
        public int SparePartId { get; set; }

        /// <summary>
        /// Thông tin phụ tùng
        /// </summary>
        public Inventory Inventory { get; set; } = null!;

        /// <summary>
        /// Chính sách bảo hành áp dụng
        /// </summary>
        public int SparePartWarrantyPolicyId { get; set; }

        /// <summary>
        /// Thông tin chính sách bảo hành
        /// </summary>
        public SparePartWarrantyPolicy SparePartWarrantyPolicy { get; set; } = null!;

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


