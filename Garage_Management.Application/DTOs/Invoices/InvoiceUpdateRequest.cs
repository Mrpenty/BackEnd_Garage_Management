using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Invoices
{
    public class InvoiceUpdateRequest
    {
        public DateTime? InvoiceDate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ServiceTotal { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SparePartTotal { get; set; }

        [MaxLength(50)]
        public string? PaymentStatus { get; set; }

        [MaxLength(50)]
        public string? PaymentMethod { get; set; }
    }
}
