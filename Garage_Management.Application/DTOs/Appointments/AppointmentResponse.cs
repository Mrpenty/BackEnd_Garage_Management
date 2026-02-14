using Garage_Management.Base.Common.Enums;

namespace Garage_Management.Application.DTOs.Appointments
{
    public class AppointmentResponse
    {
        public int AppointmentId { get; set; }
        public int CustomerId { get; set; }
        public int? VehicleId { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
