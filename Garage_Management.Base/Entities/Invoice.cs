using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Entities.Branches;
using Garage_Management.Base.Entities.JobCards;
using System;
using System.Collections.Generic;

namespace Garage_Management.Base.Entities
{
    /// <summary>
    /// Bảng Invoice - Hóa đơn thanh toán cho phiếu sửa chữa
    /// </summary>
    public class Invoice : AuditableEntity
    {
        
        public int InvoiceId { get; set; }

        /// <summary>
        /// Chi nhánh phát hành hóa đơn (denormalize từ JobCard để query report nhanh)
        /// </summary>
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        public int JobCardId { get; set; }

        public JobCard JobCard { get; set; } = null!;

        /// <summary>
        /// Ngày xuất hóa đơn
        /// </summary>
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Tổng tiền dịch vụ
        /// </summary>
        public decimal ServiceTotal { get; set; }

        /// <summary>
        /// Tổng tiền phụ tùng
        /// </summary>
        public decimal SparePartTotal { get; set; }

        /// <summary>
        /// Tổng tiền khách hàng phải thanh toán
        /// </summary>
        public decimal GrandTotal { get; set; }

        /// <summary>
        /// Trạng thái thanh toán (đã thanh toán / chưa thanh toán / thanh toán một phần...)
        /// </summary>
        public string? PaymentStatus { get; set; }

        /// <summary>
        /// Hình thức thanh toán (tiền mặt, chuyển khoản, thẻ...)
        /// </summary>
        public string? PaymentMethod { get; set; }
    }
}


