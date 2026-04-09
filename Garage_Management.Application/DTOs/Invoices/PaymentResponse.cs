using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.Invoices
{
    public class PaymentResponse
    {
        public int InvoiceId { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? TransactionId { get; set; }
        public string? Message { get; set; }
    }
}
