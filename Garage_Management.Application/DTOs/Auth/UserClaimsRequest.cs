using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.Auth
{
    public class UserClaimsRequest
    {
        public int UserId { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsAuthenticated { get; set; }
        public string? TokenSource { get; set; } // "cookie" hoặc "header"
    }
}
