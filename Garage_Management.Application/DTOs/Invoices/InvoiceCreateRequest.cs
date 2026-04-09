using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Invoices
{
    public class InvoiceCreateRequest
    {
        [Required]
        public int JobCardId { get; set; }

        public DateTime? InvoiceDate { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ServiceTotal { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal SparePartTotal { get; set; }

        [MaxLength(50)]
        public string? PaymentMethod { get; set; }
    }
}
