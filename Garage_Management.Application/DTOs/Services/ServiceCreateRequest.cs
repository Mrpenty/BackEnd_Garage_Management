using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Services
{
    public class ServiceCreateRequest
    {
        [Required]
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
