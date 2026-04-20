using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Branches
{
    public class BranchUpdateRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        public int? ManagerEmployeeId { get; set; }
    }
}
