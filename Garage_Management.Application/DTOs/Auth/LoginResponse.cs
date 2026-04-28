using Garage_Management.Base.Entities.Accounts;

namespace Garage_Management.Application.DTOs.Auth
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string Role { get; set; } = string.Empty;           // hoặc List<string> Roles nếu hỗ trợ multi-role
        public bool IsActive { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
