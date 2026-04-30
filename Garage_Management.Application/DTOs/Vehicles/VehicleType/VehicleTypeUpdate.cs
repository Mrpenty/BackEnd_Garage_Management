using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Vehicles.VehicleType
{
    public class VehicleTypeUpdate
    {
        [Required]
        [MaxLength(100)]
        public string TypeName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
