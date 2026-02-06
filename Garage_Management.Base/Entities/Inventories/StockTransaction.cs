using Garage_Management.Base.Common.Base;
using System;

namespace Garage_Management.Base.Entities.Inventories
{
    /// <summary>
    /// Bảng StockTransaction - Giao dịch nhập / xuất kho phụ tùng
    /// </summary>
    public class StockTransaction : AuditableEntity
    {
       
        public int StockTransactionId { get; set; }

        /// <summary>
        /// Phụ tùng được nhập/xuất
        /// </summary>
        public int SparePartId { get; set; }

        /// <summary>
        /// Thông tin phụ tùng liên quan
        /// </summary>
        public Inventory Inventory { get; set; } = null!;

        /// <summary>
        /// Số lượng thay đổi (+ nhập, - xuất)
        /// </summary>
        public int QuantityChange { get; set; }

        /// <summary>
        /// Giá trên mỗi đơn vị trong giao dịch (giá nhập hoặc giá xuất kho)
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Ghi chú (ví dụ: nhập hàng, xuất cho phiếu sửa chữa #123, trả hàng,...)
        /// </summary>
        public string? Note { get; set; }
    }
}


