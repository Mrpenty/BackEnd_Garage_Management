using Garage_Management.Base.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Appointments
{
    public class AppointmentCreateRequest
    {
        [Required]
        public int CustomerId { get; set; }
        public int? VehicleId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public string? Description { get; set; }
    }
}
