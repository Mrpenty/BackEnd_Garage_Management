namespace Garage_Management.Application.DTOs.JobCards
{
    public class JobCardSparePartResponse
    {
        public int JobCardId { get; set; }
        public int SparePartId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsUnderWarranty { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
