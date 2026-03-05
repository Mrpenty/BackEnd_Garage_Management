using Garage_Management.Base.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Appointments
{
    public class AppointmentCreateRequest
    {
        public int? CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public int? VehicleId { get; set; }
        public int? VehicleModelId { get; set; }
        public string? CustomVehicleBrand { get; set; }
        public string? CustomVehicleModel { get; set; }
        public string? LicensePlate { get; set; }   
        public DateTime AppointmentDateTime { get; set; }
        public List<int> ServiceIds { get; set; } = new();
        public List<int> SparePartsIds { get; set; } = new();

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public string? Description { get; set; }
    }
}
