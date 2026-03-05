using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Inventories.SparePartBrands
{
    public class SparePartBrandCreateRequest
    {
        [Required]
        public string BrandName { get; set; } = string.Empty;
        [MaxLength(255)]

        public string? Description { get; set; }
    }
}
