using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.Invoices
{
    public class CreatePaymentRequest
    {
        /// <summary>
        /// Id của hóa đơn cần thanh toán
        /// </summary>
        public int InvoiceId { get; set; }  
    }
}
