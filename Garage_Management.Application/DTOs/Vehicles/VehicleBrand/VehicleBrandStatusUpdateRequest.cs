using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Vehicles.VehicleBrand
{
    public class VehicleBrandStatusUpdateRequest
    {
        [Required]
        public bool IsActive { get; set; }
    }
}

