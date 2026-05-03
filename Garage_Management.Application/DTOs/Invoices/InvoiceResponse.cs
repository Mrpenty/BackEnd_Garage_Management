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

        // Chi tiết dịch vụ + phụ tùng (chỉ điền khi gọi GetByIdAsync)
        public List<InvoiceServiceItemDto> Services { get; set; } = new();
        public List<InvoiceSparePartItemDto> SpareParts { get; set; } = new();
    }

    public class InvoiceServiceItemDto
    {
        public int JobCardServiceId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class InvoiceSparePartItemDto
    {
        public int SparePartId { get; set; }
        public string? PartCode { get; set; }
        public string PartName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsUnderWarranty { get; set; }
        public string? Note { get; set; }
    }
}
