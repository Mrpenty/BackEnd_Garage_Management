namespace Garage_Management.Application.DTOs.RepairEstimateServices
{
    public class RepairEstimateServiceUpdateRequest
    {
        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
