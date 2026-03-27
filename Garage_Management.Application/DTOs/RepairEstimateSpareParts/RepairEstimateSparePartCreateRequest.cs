namespace Garage_Management.Application.DTOs.RepairEstimateSpareParts
{
    public class RepairEstimateSparePartCreateRequest
    {
        public int RepairEstimateId { get; set; }
        public int SparePartId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
