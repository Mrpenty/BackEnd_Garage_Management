using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Vehicles
{
    public class VehicleCreateRequest
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public int ModelId { get; set; }
        [MaxLength(11)]
        public string? LicensePlate { get; set; }
        [Range(1900, 2100)]
        public int? Year { get; set; }
        [MaxLength(20)]
        public string? Vin { get; set; }
        public int? CreatedBy { get; set; }
    }
}
