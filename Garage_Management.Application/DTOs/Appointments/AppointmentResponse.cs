using Garage_Management.Application.DTOs.Services;
using Garage_Management.Application.DTOs.Iventories;
using Garage_Management.Base.Common.Enums;

namespace Garage_Management.Application.DTOs.Appointments
{
    public class AppointmentResponse
    {
        public int AppointmentId { get; set; }
        public int? CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public int? VehicleId { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public long TotalEstimateMinute { get; set; }
        public List<ServiceResponse> Services { get; set; } = new();
        public List<InventoryResponse> SpareParts { get; set; } = new();
        public DateTime AppointmentDateTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
