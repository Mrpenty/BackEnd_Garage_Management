namespace Garage_Management.Application.DTOs.RepairEstimates
{
    public class RepairEstimateResponse
    {
        public int RepairEstimateId { get; set; }
        public int JobCardId { get; set; }
        public decimal ServiceTotal { get; set; }
        public decimal SparePartTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
