using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Inventories;
using System;

namespace Garage_Management.Base.Entities.RepairEstimaties
{
    /// <summary>
    /// Bảng trung gian: RepairEstimateSparePart
    /// Liệt kê các phụ tùng trong một báo giá sửa chữa
    /// </summary>
    public class RepairEstimateSparePart
    {
       
        public int RepairEstimateId { get; set; }

        public int SparePartId { get; set; }
        public RepairEstimateApprovalStatus Status { get; set; } = RepairEstimateApprovalStatus.WaitingApproval;

        /// <summary>
        /// Báo giá liên quan
        /// </summary>
        public RepairEstimate RepairEstimate { get; set; } = null!;

        /// <summary>
        /// Phụ tùng được báo giá
        /// </summary>
        public Inventory Inventory { get; set; } = null!;

        /// <summary>
        /// Số lượng phụ tùng dự kiến sử dụng
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Đơn giá dự kiến
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Thành tiền = Quantity * UnitPrice
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}


