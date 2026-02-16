using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Vehicles
{
    public class VehicleUpdateRequest
    {
        public int? BrandId { get; set; }
        public int? ModelId { get; set; }
        [MaxLength(10)]
        public string? LicensePlate { get; set; }
        [Range(1900, 2100)]
        public int? Year { get; set; }
        [MaxLength(20)]
        public string? Vin { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
