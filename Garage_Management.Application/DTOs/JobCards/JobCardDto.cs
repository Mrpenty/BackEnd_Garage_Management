using Garage_Management.Application.DTOs.JobCardMechanics;
using Garage_Management.Application.DTOs.JobCardServices;
using Garage_Management.Base.Common.Enums;

namespace Garage_Management.Application.DTOs.JobCards
{
    public class JobCardDto
    {
        public int JobCardId { get; set; }
        public int? AppointmentId { get; set; }
        public int CustomerId { get; set; }
        public int VehicleId { get; set; }
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
        public List<JobCardMechanicView> Mechanics { get; set; } = new();
    }
}
