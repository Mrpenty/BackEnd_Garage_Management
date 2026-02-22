using Garage_Management.Base.Common.Enums;

namespace Garage_Management.Application.DTOs.JobCardServices
{
    public class JobCardServiceCreateRequest
    {
        public int JobCardId { get; set; }
        public int ServiceId { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public ServiceStatus Status { get; set; } = ServiceStatus.Pending;
        public int? SourceInspectionItemId { get; set; }
    }
}
