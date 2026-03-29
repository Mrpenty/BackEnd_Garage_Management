using Garage_Management.Base.Common.Enums;

namespace Garage_Management.Application.DTOs.RepairEstimates
{
    public class RepairEstimateDetailResponse
    {
        public int RepairEstimateId { get; set; }
        public int JobCardId { get; set; }
        public RepairEstimateApprovalStatus Status { get; set; }
        public decimal ServiceTotal { get; set; }
        public decimal SparePartTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<RepairEstimateDetailServiceItemResponse> Services { get; set; } = new();
        public List<RepairEstimateDetailSparePartItemResponse> SpareParts { get; set; } = new();
    }
}
