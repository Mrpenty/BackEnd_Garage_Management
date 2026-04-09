namespace Garage_Management.Base.Common.Models.Invoices
{
    public class InvoiceQuery : ParamQuery
    {
        /// <summary>
        /// Lọc theo trạng thái thanh toán: "Unpaid", "Paid", "Failed"
        /// </summary>
        public string? PaymentStatus { get; set; }

        /// <summary>
        /// Lọc theo CustomerId
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// Lọc theo JobCardId
        /// </summary>
        public int? JobCardId { get; set; }
    }
}
