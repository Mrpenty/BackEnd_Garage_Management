using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Inventories.SparePartBrands
{
    public class SparePartBrandUpdateRequest
    {
        [Required]
        public string BrandName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
