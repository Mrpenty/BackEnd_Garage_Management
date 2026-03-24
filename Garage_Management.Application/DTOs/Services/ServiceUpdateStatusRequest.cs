using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Services
{
    public class ServiceUpdateStatusRequest
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
