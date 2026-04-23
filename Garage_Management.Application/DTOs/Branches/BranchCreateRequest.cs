using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Branches
{
    public class BranchCreateRequest
    {
        [Required]
        [MaxLength(20)]
        public string BranchCode { get; set; } = string.Empty;

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

        public bool IsActive { get; set; } = true;
    }
}
