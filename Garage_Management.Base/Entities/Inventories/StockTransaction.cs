using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Branches;
using Garage_Management.Base.Entities.JobCards;
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
        /// Chi nhánh thực hiện giao dịch
        /// </summary>
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        /// <summary>
        /// Phụ tùng được nhập/xuất
        /// </summary>
        public int SparePartId { get; set; }
        /// <summary>
        /// Mã phiếu
        /// </summary>
        public string? ReceiptCode { get; set; }
        /// <summary>
        /// Loại giao dịch
        /// </summary>
        public TransactionType TransactionType { get; set; }
        /// <summary>
        /// Nhà cung cấp
        /// </summary>
        public int? SupplierId { get; set; }

        /// <summary>
        /// Thông tin nhà cung cấp
        /// </summary>
        public Supplier? Supplier { get; set; }

        /// <summary>
        /// Phiếu sửa chữa
        /// </summary>
        public int? JobCardId {  get; set; }

        /// <summary>
        /// Thông tin JobCard
        /// </summary>
        public JobCard? JobCard { get; set; }

        /// <summary>
        /// Thông tin phụ tùng liên quan
        /// </summary>
        public Inventory Inventory { get; set; } = null!;

        /// <summary>
        /// Số lô
        /// </summary>
        public string? LotNumber {get; set; }

        /// <summary>
        /// Số lượng thay đổi (+ nhập, - xuất)
        /// </summary>
        public int QuantityChange { get; set; }

        /// <summary>
        /// Số seri
        /// </summary>
        public string? SerialNumber { get; set; }   

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


