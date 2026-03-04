using Garage_Management.Base.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Appointments
{
    public class AppointmentStatusUpdateRequest
    {
        [Required]
        public AppointmentStatus Status { get; set; }
    }
}
