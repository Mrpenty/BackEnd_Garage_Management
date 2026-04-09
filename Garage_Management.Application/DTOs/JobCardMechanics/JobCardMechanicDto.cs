using Garage_Management.Application.DTOs.Services;
using Garage_Management.Application.DTOs.ServiceTasks;
using Garage_Management.Base.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.JobCardMechanics
{
    public class JobCardMechanicDto
    {
        public int JobCardMechanicId { get; set; }   

        // Thông tin phân công
        public int JobCardId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public MechanicAssignmentStatus MechanicAssignmenStatus { get; set; }
        public string? Note { get; set; }

        // Thông tin JobCard
        public DateTime JobCardStartDate { get; set; }
        public DateTime? JobCardEndDate { get; set; }
        public JobCardStatus JobCardStatus { get; set; }
        public int ProgressPercentage { get; set; }
        public string? ProgressNotes { get; set; }
        public string? JobCardDescription { get; set; } = string.Empty;
         
        public string? Supervisor{ get; set; } = string.Empty;

        // Thông tin Khách hàng
        public int CustomerId { get; set; }
        public string CustomerFullName { get; set; } = string.Empty;
        public string? CustomerPhone { get; set; }

        // Thông tin Xe
        public int VehicleId { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public string? VehicleBrand { get; set; }
        public string? VehicleModel { get; set; }

        // Thông tin lịch hẹn (nếu có)
        public int? AppointmentId { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string? AppointmentNote { get; set; }

        public List<ServiceJobCardMechanicResponse> Services { get; set; } = new();
        public List<SparePartJobCardMechanicResponse> SpareParts { get; set; } = new();
        public List<AppointmentSparePartResponse>? AppointmentSpareParts { get; set; }
    }

    public class ServiceJobCardMechanicResponse
    {
        public int JobCardServiceId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ServiceStatus ServiceStatus { get; set; }
        public string ServiceStatusName { get; set; } = string.Empty;
        public long TotalEstimateMinute { get; set; }
        public List<ServiceTaskJobCardMechanicResponse> ServiceTasks { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ServiceTaskJobCardMechanicResponse
    {
        public int JobCardServicedTaskId { get; set; }
        public int ServiceTaskId { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public int TaskOrder { get; set; }
        public int EstimateMinute { get; set; }
        public ServiceStatus ServiceTaskStatus { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ServiceTaskStatusName { get; set; } = string.Empty;
    }

    public class SparePartJobCardMechanicResponse
    {
        public int SparePartId { get; set; }
        public string PartName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsUnderWarranty { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AppointmentSparePartResponse
    {
        public int SparePartId { get; set; }
        public string PartName { get; set; } = string.Empty;
    }

}
