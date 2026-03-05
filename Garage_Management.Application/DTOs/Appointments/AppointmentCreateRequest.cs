using Garage_Management.Base.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Appointments
{
    public class AppointmentCreateRequest
    {
        public int? CustomerId { get; set; }
        [MaxLength(20)]
        public string? FirstName { get; set; }
        [MaxLength(20)]
        public string? LastName { get; set; }
        [MaxLength(11)]
        public string? Phone { get; set; }
        public int? VehicleId { get; set; }
        public int? VehicleModelId { get; set; }
        [MaxLength(30)]
        public string? CustomVehicleBrand { get; set; }
        [MaxLength(30)]
        public string? CustomVehicleModel { get; set; }
        [MaxLength(11)]
        public string? LicensePlate { get; set; }
        [Required]   
        public DateTime AppointmentDateTime { get; set; }
        public List<int> ServiceIds { get; set; } = new();
        public List<int> SparePartsIds { get; set; } = new();

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
