using Garage_Management.Base.Common.Enums;

namespace Garage_Management.Application.DTOs.JobCardServiceTasks
{
    public class JobCardServiceTaskResponse
    {
        public int JobCardServiceTaskId { get; set; }
        public int JobCardId { get; set; }
        public int ServiceTaskId { get; set; }
        public int TaskOrder { get; set; }
        public ServiceStatus Status { get; set; }
        public bool? IsOptional { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Note { get; set; }
        public int? PerformedById { get; set; }
    }
}
