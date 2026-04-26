using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.StockChecks
{
    public class StockCheckSubmitRequest
    {
        /// <summary>
        /// Ngày giờ thực hiện kiểm kê (mặc định = hiện tại nếu null)
        /// </summary>
        public DateTime? CheckDate { get; set; }

        /// <summary>
        /// Phạm vi kiểm kê (mô tả tự do để ghi chú: "All", "Category: Phanh", "Specific parts: BG-001,LOC-001"...)
        /// </summary>
        [MaxLength(200)]
        public string? Scope { get; set; }

        /// <summary>
        /// Danh sách phụ tùng được kiểm kê (toàn bộ items của phiên, kể cả không chênh lệch)
        /// </summary>
        [Required]
        [MinLength(1)]
        public List<StockCheckItemRequest> Items { get; set; } = new();
    }

    public class StockCheckItemRequest
    {
        [Required]
        public int SparePartId { get; set; }

        /// <summary>
        /// Số lượng đếm thực tế bởi Stocker
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int StockActual { get; set; }

        /// <summary>
        /// Lý do chênh lệch (Hao hụt, Mất, Sai sót nhập liệu, Hỏng hóc, Khác...)
        /// Bắt buộc khi có chênh lệch (delta != 0).
        /// </summary>
        [MaxLength(500)]
        public string? Reason { get; set; }
    }
}
