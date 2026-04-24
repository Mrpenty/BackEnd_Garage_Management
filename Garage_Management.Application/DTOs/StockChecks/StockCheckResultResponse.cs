namespace Garage_Management.Application.DTOs.StockChecks
{
    /// <summary>
    /// Kết quả sau khi hoàn tất 1 phiên kiểm kê.
    /// </summary>
    public class StockCheckResultResponse
    {
        /// <summary>
        /// Mã phiên kiểm kê (định danh chung cho tất cả adjustment cùng phiên).
        /// Format: KK-yyyyMMddHHmmss
        /// </summary>
        public string ReceiptCode { get; set; } = string.Empty;

        public DateTime CheckDate { get; set; }
        public string? Scope { get; set; }
        public int BranchId { get; set; }

        public int TotalItems { get; set; }
        public int DiscrepanciesAdjusted { get; set; }
        public int ShortageCount { get; set; }
        public int SurplusCount { get; set; }

        public List<StockCheckAdjustmentResponse> Adjustments { get; set; } = new();
    }

    public class StockCheckAdjustmentResponse
    {
        public int StockTransactionId { get; set; }
        public int SparePartId { get; set; }
        public string? PartCode { get; set; }
        public string PartName { get; set; } = string.Empty;
        public int StockSystem { get; set; }
        public int StockActual { get; set; }

        /// <summary>
        /// Delta = StockActual - StockSystem. Âm = thiếu (shortage), dương = thừa (surplus).
        /// </summary>
        public int Delta { get; set; }
        public string? Reason { get; set; }
    }
}
