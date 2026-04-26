namespace Garage_Management.Application.DTOs.StockChecks
{
    /// <summary>
    /// Snapshot tồn kho hệ thống cho 1 phụ tùng tại thời điểm bắt đầu kiểm kê.
    /// </summary>
    public class StockCheckItemSnapshotResponse
    {
        public int SparePartId { get; set; }
        public string? PartCode { get; set; }
        public string PartName { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? SparePartBrandId { get; set; }
        public string? SparePartBrandName { get; set; }

        /// <summary>
        /// Số lượng theo hệ thống (Inventory.Quantity hiện tại)
        /// </summary>
        public int StockSystem { get; set; }

        public int BranchId { get; set; }
    }
}
