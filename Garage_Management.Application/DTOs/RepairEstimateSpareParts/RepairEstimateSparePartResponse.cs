namespace Garage_Management.Application.DTOs.RepairEstimateSpareParts
{
    public class RepairEstimateSparePartResponse
    {
        public int RepairEstimateId { get; set; }
        public int SparePartId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
