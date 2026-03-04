using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.User
{
    public class UserRequest
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; }
        public string? UpdatedAt { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string? FullName { get; set; }
        public EmployeeInfo? Employee { get; set; }

        public class EmployeeInfo
        {
            public int EmployeeId { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Position { get; set; } 
            
        }
    }
}