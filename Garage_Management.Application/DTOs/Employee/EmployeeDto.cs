using Garage_Management.Base.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.Employee
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public string? BranchName { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? EmployeeCode { get; set; }
        public string? Position { get; set; }
        public WorkingStatus Status { get; set; } = WorkingStatus.Available;
        public string WorkingEmpStatus { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? StartWorkingDate { get; set; }

        // Thông tin tài khoản User
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }

    public class CreateEmployeeRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
       // public string? EmployeeCode { get; set; }
        public string? Position { get; set; }
        public string Role { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime? StartWorkingDate { get; set; }

        /// <summary>
        /// Chi nhánh của nhân viên. Admin bắt buộc phải chỉ định khi tạo.
        /// </summary>
        public int BranchId { get; set; }
    }
}
