namespace Garage_Management.Application.DTOs.StockChecks
{
    /// <summary>
    /// Trả lại chi tiết 1 phiên kiểm kê đã thực hiện (truy vấn theo ReceiptCode).
    /// </summary>
    public class StockCheckSessionResponse
    {
        public string ReceiptCode { get; set; } = string.Empty;
        public DateTime CheckDate { get; set; }
        public int BranchId { get; set; }
        public int? CreatedBy { get; set; }

        public int DiscrepanciesAdjusted { get; set; }
        public int ShortageCount { get; set; }
        public int SurplusCount { get; set; }

        public List<StockCheckAdjustmentResponse> Adjustments { get; set; } = new();
    }
}
