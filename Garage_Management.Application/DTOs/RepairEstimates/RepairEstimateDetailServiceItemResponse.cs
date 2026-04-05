using Garage_Management.Base.Common.Enums;

namespace Garage_Management.Application.DTOs.RepairEstimates
{
    public class RepairEstimateDetailServiceItemResponse
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public RepairEstimateApprovalStatus Status { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
