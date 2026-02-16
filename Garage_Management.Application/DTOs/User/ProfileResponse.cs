using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.User
{
    public class ProfileResponse
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string> Roles { get; set; } = new();

        public string? FullName { get; set; }

        public CustomerInfo? Customer { get; set; }
        public EmployeeInfo? Employee { get; set; }

        public class CustomerInfo
        {
            public int CustomerId { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class EmployeeInfo
        {
            public int EmployeeId { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Position { get; set; }     
            public DateTime? HireDate { get; set; }
        }
    }
}
