using Garage_Management.Application.DTOs.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Entities.Services;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Appointments
{
    public class AppointmentCreateRequest
    {
        [Required]
        public int CustomerId { get; set; }
        public int? VehicleId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public List<int> ServiceIds { get; set; } = new();
        public List<int> SparePartsIds { get; set; } = new();

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        public string? Description { get; set; }
    }
}
