using Garage_Management.Application.DTOs.JobCardMechanics;
using Garage_Management.Application.DTOs.JobCardServices;
using Garage_Management.Application.DTOs.User;
using Garage_Management.Base.Common.Enums;

namespace Garage_Management.Application.DTOs.JobCards
{
    public class JobCardDto
    {
        public int JobCardId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? SupervisorName { get; set; }
        public int VehicleId { get; set; }
        public int? AppointmentId { get; set; }
        public int? CustomerId { get; set; }
        public int? WorkbayId { get; set; }
        public decimal QueueOrder { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public JobCardStatus Status { get; set; }
        public int ProgressPercentage { get; set; }
        public string? CompletedSteps { get; set; }
        public string? ProgressNotes { get; set; }
        public string? Note { get; set; }
        public int? SupervisorId { get; set; }
        public int? CreatedByEmployeeId { get; set; }
        public List<JobCardServiceResponse> Services { get; set; } = new();
        public List<JobCardMechanicView>? Mechanics { get; set; } = new();
        public List<JobCardSparePartView> SpareParts { get; set; } = new();
        public List<VehicleDto> Vehicles { get; set; } = new();
    }
    public class JobCardSparePartView
    {
        public int SparePartId { get; set; }
        public string SparePartName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsUnderWarranty { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
