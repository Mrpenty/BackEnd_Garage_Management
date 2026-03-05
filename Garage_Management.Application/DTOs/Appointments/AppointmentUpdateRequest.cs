using Garage_Management.Base.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Appointments
{
    public class AppointmentUpdateRequest
    {
        public int? CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public int? VehicleId { get; set; }
        public int? VehicleModelId { get; set; }
        [Required]
        public int? UpdatedBy { get; set; }
        [Required]
        public DateTime? AppointmentDateTime { get; set; }
        public AppointmentStatus? Status { get; set; }
        public string? Description { get; set; }
    }
}
