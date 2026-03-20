using Garage_Management.Base.Common.Enums;
using System;

namespace Garage_Management.Application.DTOs.JobCards
{
    public class UpdateProgressResponse
    {
        public int JobCardId { get; set; }
        public JobCardStatus Status { get; set; }
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
        public ServiceStatus Status { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }
}