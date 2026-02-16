using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Services
{
    public class ServiceUpdateRequest
    {
        [Required]
        public string? ServiceName { get; set; }
        public decimal? BasePrice { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
