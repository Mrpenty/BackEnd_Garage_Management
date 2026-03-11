using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.ServiceTasks
{
    public class ServiceTaskCreateRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "ServiceId must be greater than 0")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "TaskName is required")]
        [MaxLength(200, ErrorMessage = "TaskName max length is 200")]
        public string TaskName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "TaskOrder must be greater than 0")]
        public int TaskOrder { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "EstimateMinute must be >= 0")]
        public int EstimateMinute { get; set; }

        [MaxLength(500, ErrorMessage = "Note max length is 500")]
        public string? Note { get; set; }
    }
}
