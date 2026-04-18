using Garage_Management.Base.Common.Enums;

namespace Garage_Management.Application.DTOs.JobCardServices
{
    public class JobCardServiceResponse
    {
        public string? ServiceName { get; set; }
        public int JobCardServiceId { get; set; }
        public int JobCardId { get; set; }
        public int ServiceId { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public ServiceStatus Status { get; set; }
        public int? SourceInspectionItemId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<JobCardServiceTaskDto> ServiceTasks { get; set; } = new();

    }
    public class JobCardServiceTaskDto
    {
        public int JobCardServiceTaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public ServiceStatus Status { get; set; }
        public string StatusName => Status.ToString();
    }

}
