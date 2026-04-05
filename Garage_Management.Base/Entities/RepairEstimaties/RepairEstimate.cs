using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.JobCards;
using System;
using System.Collections.Generic;

namespace Garage_Management.Base.Entities.RepairEstimaties
{
    /// <summary>
    /// Bảng RepairEstimate - Báo giá sửa chữa cho khách hàng
    /// </summary>
    public class RepairEstimate : AuditableEntity
    {
       
        public int RepairEstimateId { get; set; }

        /// <summary>
        /// Phiếu sửa chữa liên quan 
        /// </summary>
        public int JobCardId { get; set; }
        public RepairEstimateApprovalStatus Status { get; set; } = RepairEstimateApprovalStatus.WaitingApproval;

        /// <summary>
        /// Thông tin phiếu sửa chữa
        /// </summary>
        public JobCard JobCard { get; set; } = null!;

        /// <summary>
        /// Tổng tiền dịch vụ trong báo giá
        /// </summary>
        public decimal ServiceTotal { get; set; }

        /// <summary>
        /// Tổng tiền phụ tùng trong báo giá
        /// </summary>
        public decimal SparePartTotal { get; set; }

        /// <summary>
        /// Tổng tiền dự kiến khách hàng phải trả
        /// </summary>
        public decimal GrandTotal { get; set; }

        /// <summary>
        /// Ghi chú bổ sung cho báo giá
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// Các dòng dịch vụ trong báo giá
        /// </summary>
        public ICollection<RepairEstimateService> Services { get; set; } = new List<RepairEstimateService>();

        /// <summary>
        /// Các dòng phụ tùng trong báo giá
        /// </summary>
        public ICollection<RepairEstimateSparePart> SpareParts { get; set; } = new List<RepairEstimateSparePart>();
    }
}


