using Garage_Management.Base.Common.Enums;

namespace Garage_Management.Application.DTOs.JobCardServices
{
    public class JobCardServiceUpdateRequest
    {
        public int? JobCardId { get; set; }
        public int? ServiceId { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public ServiceStatus? Status { get; set; }
        public int? SourceInspectionItemId { get; set; }
    }
}
