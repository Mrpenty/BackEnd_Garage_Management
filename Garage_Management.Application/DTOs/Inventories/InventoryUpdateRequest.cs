using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Iventories
{
    public class InventoryUpdateRequest
    {
        [MaxLength(50)]
        public string? PartCode { get; set; }

        [MaxLength(200)]
        public string? PartName { get; set; }

        [MaxLength(50)]
        public string? Unit { get; set; }

        public int? CategoryId { get; set; }
        public int? SparePartBrandId { get; set; }

        [Range(0, int.MaxValue)]
        public int? MinQuantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? LastPurchasePrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SellingPrice { get; set; }
    }
}
