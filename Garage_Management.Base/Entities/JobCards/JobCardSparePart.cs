using Garage_Management.Base.Entities.Inventories;
using System;

namespace Garage_Management.Base.Entities.JobCards
{
    /// <summary>
    /// Bảng trung gian: JobCardSparePart
    /// Lưu các phụ tùng được sử dụng trên một phiếu sửa chữa (JobCard)
    /// </summary>
    public class JobCardSparePart
    {
       
        public int JobCardId { get; set; }

        /// <summary>
        /// Mã phụ tùng được xuất dùng
        /// </summary>
        public int SparePartId { get; set; }

        /// <summary>
        /// Phiếu sửa chữa liên quan
        /// </summary>
        public JobCard JobCard { get; set; } = null!;

        /// <summary>
        /// Phụ tùng được sử dụng
        /// </summary>
        public Inventory Inventory { get; set; } = null!;

        /// <summary>
        /// Số lượng phụ tùng sử dụng
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Đơn giá thực tế tại thời điểm xuất kho
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Thành tiền = Quantity * UnitPrice
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Có áp dụng bảo hành cho phụ tùng này hay không
        /// </summary>
        public bool IsUnderWarranty { get; set; }

        /// <summary>
        /// Ghi chú thêm (nếu có)
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// Thời điểm thêm dòng phụ tùng vào phiếu
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}


