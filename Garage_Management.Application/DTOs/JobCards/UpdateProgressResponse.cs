using Garage_Management.Base.Common.Enums;
using System;

namespace Garage_Management.Application.DTOs.JobCards
{
    public class UpdateProgressResponse
    {
        public int JobCardId { get; set; }
        public JobCardStatus StatusJobCard { get; set; }
        public string StatusJobCardName { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; }
        //public string? CompletedSteps { get; set; }
        public string? ProgressNotes { get; set; }
        public DateTime? EndDate { get; set; }
        public List<JobCardServiceProgressDto> Services { get; set; } = new();
    }

    public class JobCardServiceProgressDto
    {
        public int JobCardServiceId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public ServiceStatus StatusService { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public List<JobCardServiceTaskProgressDto> ServiceTasks { get; set; } = new();

    }

    public class JobCardServiceTaskProgressDto
    {
        public int JobCardServiceTaskId { get; set; }
        public int ServiceTaskId { get; set; }
        public string ServiceTaskName { get; set; } = string.Empty;
        public ServiceStatus StatusServiceTask { get; set; }
        public string? Note { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }
}