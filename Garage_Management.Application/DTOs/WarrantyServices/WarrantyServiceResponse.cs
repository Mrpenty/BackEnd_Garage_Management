using System;

namespace Garage_Management.Application.DTOs.WarrantyServices
{
    public class WarrantyServiceResponse
    {
        public int WarrantyServiceId { get; set; }
        public int ServiceId { get; set; }
        public int WarrantyPolicyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Description { get; set; }
    }
}
