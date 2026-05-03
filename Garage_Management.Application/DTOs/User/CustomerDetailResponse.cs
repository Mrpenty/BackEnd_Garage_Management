using System;
using System.Collections.Generic;

namespace Garage_Management.Application.DTOs.User
{
    public class CustomerDetailResponse
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<VehicleDto> Vehicles { get; set; } = new();
        public List<CustomerJobCardSummaryDto> RepairHistory { get; set; } = new();
    }

    public class CustomerJobCardSummaryDto
    {
        public int JobCardId { get; set; }

        public int VehicleId { get; set; }
        public string? LicensePlate { get; set; }

        public int BranchId { get; set; }
        public string? BranchName { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int Status { get; set; }
        public string StatusName { get; set; } = string.Empty;

        public int ProgressPercentage { get; set; }
        public string? Note { get; set; }

        public int ServiceCount { get; set; }
        public int SparePartCount { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
