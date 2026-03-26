using Garage_Management.Base.Common.Enums;
using System;

namespace Garage_Management.Application.DTOs.JobCards
{
    public class ViewRepairProgressResponse
    {
        public int JobCardId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhoneNumber { get; set; }
        public string VehicleLicensePlate { get; set; } = string.Empty;
        public string VehicleBrand { get; set; } = string.Empty;
        public string VehicleModel { get; set; } = string.Empty;
        public JobCardStatus Status { get; set; }
        public string StatusJobCardName { get; set; } = string.Empty;

        public int ProgressPercentage { get; set; }
       // public string? CompletedSteps { get; set; }
        public string? ProgressNotes { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public long EstimatedJobCardMinutesRemaining { get; set; }
        public string? EstimatedCompletionTime { get; set; } // Tổng thời gian ước tính còn lại
        public List<ServiceProgressDto> Services { get; set; } = new();
        public string? AssignedMechanic { get; set; }
        public string? Supervisor { get; set; }
    }

    public class ServiceProgressDto
    {
        public int JobCardServiceId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public ServiceStatus Status { get; set; }
        public string ServiceStatusName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long EstimatedMinutesRemaining { get; set; } // Thời gian ước tính còn lại cho service này
        public List<TaskProgressDto> Tasks { get; set; } = new();
    }

    public class TaskProgressDto
    {
        public int JobCardServiceTaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public ServiceStatus Status { get; set; }
        public string ServiceTaskStatusName { get; set; } = string.Empty;

        public int EstimateMinute { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
