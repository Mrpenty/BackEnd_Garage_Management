using Garage_Management.Application.DTOs.ServiceTasks;
using System.Collections.Generic;

namespace Garage_Management.Application.DTOs.Services
{
    public class ServiceResponse
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal? BasePrice { get; set; }
        public string? Description { get; set; }
        public long TotalEstimateMinute { get; set; }
        public List<ServiceTaskResponse> ServiceTasks { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
