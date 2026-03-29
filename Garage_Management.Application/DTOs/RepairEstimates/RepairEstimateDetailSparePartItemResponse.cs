namespace Garage_Management.Application.DTOs.RepairEstimates
{
    public class RepairEstimateDetailSparePartItemResponse
    {
        public int SparePartId { get; set; }
        public string SparePartName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
