namespace Garage_Management.Application.DTOs.ServiceTasks
{
    public class ServiceTaskResponse
    {
        public int ServiceTaskId { get; set; }
        public int ServiceId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public int TaskOrder { get; set; }
        public int EstimateMinute { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
