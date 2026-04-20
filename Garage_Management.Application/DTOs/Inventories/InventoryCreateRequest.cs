using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Iventories
{
    public class InventoryCreateRequest
    {
        [MaxLength(50)]
        public string? PartCode { get; set; }

        [Required]
        [MaxLength(200)]
        public string PartName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Unit { get; set; }

        public int? CategoryId { get; set; }
        public int? SparePartBrandId { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue)]
        public int? MinQuantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? LastPurchasePrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SellingPrice { get; set; }

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Chi nhánh. Non-admin luôn lấy từ token, admin bắt buộc truyền.
        /// </summary>
        public int? BranchId { get; set; }
    }
}
