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
        public MechanicAssignmentStatus Status { get; set; }
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
    }
}
