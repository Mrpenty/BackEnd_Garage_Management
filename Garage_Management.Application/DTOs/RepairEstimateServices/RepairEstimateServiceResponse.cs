using Garage_Management.Base.Common.Enums;

namespace Garage_Management.Application.DTOs.RepairEstimateServices
{
    public class RepairEstimateServiceResponse
    {
        public int RepairEstimateId { get; set; }
        public int ServiceId { get; set; }
        public RepairEstimateApprovalStatus Status { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
