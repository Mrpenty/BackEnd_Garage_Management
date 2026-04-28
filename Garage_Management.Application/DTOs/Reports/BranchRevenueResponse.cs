namespace Garage_Management.Application.DTOs.Reports
{
    public class BranchRevenueResponse
    {
        public int BranchId { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int InvoiceCount { get; set; }
        public decimal ServiceTotal { get; set; }
        public decimal SparePartTotal { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
