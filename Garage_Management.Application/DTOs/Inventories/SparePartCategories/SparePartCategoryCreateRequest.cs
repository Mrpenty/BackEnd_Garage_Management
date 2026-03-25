using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Inventories.SparePartCategories
{
    public class SparePartCategoryCreateRequest
    {
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
