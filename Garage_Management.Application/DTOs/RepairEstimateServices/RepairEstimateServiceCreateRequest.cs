namespace Garage_Management.Application.DTOs.RepairEstimateServices
{
    public class RepairEstimateServiceCreateRequest
    {
        public int RepairEstimateId { get; set; }
        public int ServiceId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal TotalAmount { get; set; }
    }
}
