namespace Garage_Management.Application.DTOs.RepairEstimates
{
    public class RepairEstimateCreateRequest
    {
        public int JobCardId { get; set; }
        public string? Note { get; set; }
        public List<RepairEstimateCreateServiceItemRequest> Services { get; set; } = new();
        public List<RepairEstimateCreateSparePartItemRequest> SpareParts { get; set; } = new();
    }

    public class RepairEstimateCreateServiceItemRequest
    {
        public int ServiceId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class RepairEstimateCreateSparePartItemRequest
    {
        public int SparePartId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
