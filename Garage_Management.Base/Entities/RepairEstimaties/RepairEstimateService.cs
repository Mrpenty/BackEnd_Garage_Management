using System;

namespace Garage_Management.Base.Entities.RepairEstimaties
{
    /// <summary>
    /// Bảng trung gian: RepairEstimateService
    /// Liệt kê các dịch vụ trong một báo giá sửa chữa
    /// </summary>
    public class RepairEstimateService
    {
     
        public int RepairEstimateId { get; set; }

       
        public int ServiceId { get; set; }

        /// <summary>
        /// Báo giá liên quan
        /// </summary>
        public RepairEstimate RepairEstimate { get; set; } = null!;

        /// <summary>
        /// Dịch vụ được báo giá
        /// </summary>
        public Service Service { get; set; } = null!;

        /// <summary>
        /// Đơn giá dự kiến cho dịch vụ
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Số lượng (thường là 1)
        /// </summary>
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// Thành tiền = UnitPrice * Quantity
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}


