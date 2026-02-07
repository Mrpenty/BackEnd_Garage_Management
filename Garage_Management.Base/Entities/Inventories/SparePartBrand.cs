using System.Collections.Generic;

namespace Garage_Management.Base.Entities.Inventories
{
    /// <summary>
    /// Bảng SparePartBrand - Hãng / nhà sản xuất phụ tùng
    /// </summary>
    public class SparePartBrand
    {
        public int SparePartBrandId { get; set; }

        /// <summary>
        /// Tên hãng phụ tùng
        /// </summary>
        public string BrandName { get; set; } = string.Empty;

        /// <summary>
        /// Ghi chú / mô tả thêm về hãng
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Danh sách phụ tùng thuộc hãng này
        /// </summary>
        public ICollection<Inventory> SpareParts { get; set; } = new List<Inventory>();
    }
}


