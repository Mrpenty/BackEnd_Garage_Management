using Garage_Management.Base.Common.Enums;

namespace Garage_Management.Application.DTOs.JobCards
{
    public class UpdateServiceStatusDto
    {
        public int JobCardServiceId { get; set; }
        public ServiceStatus StatusService { get; set; }
    }
    public class ServiceTaskUpdateDto
    {
        public int ServiceTaskId { get; set; }
        public ServiceStatus StatusServiceTask { get; set; }
       
    }
}