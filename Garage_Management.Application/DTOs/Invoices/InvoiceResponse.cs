namespace Garage_Management.Application.DTOs.Invoices
{
    public class InvoiceResponse
    {
        public int InvoiceId { get; set; }
        public int JobCardId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal ServiceTotal { get; set; }
        public decimal SparePartTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentMethod { get; set; }

        // Thong tin tu JobCard
        public string? CustomerName { get; set; }
        public string? VehicleLicensePlate { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
