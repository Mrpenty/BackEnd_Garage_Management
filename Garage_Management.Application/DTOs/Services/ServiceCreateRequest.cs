using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Services
{
    public class ServiceCreateRequest
    {
        [Required]
        public string ServiceName { get; set; } = string.Empty;
        [Required]
        public decimal BasePrice { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
