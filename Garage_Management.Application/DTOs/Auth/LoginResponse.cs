using Garage_Management.Base.Entities.Accounts;

namespace Garage_Management.Application.DTOs.Auth
{
    public class LoginResponse
    {
        public User User { get; set; } = null!;
        public string AccessToken { get; set; } = string.Empty;   
        public string RefreshToken { get; set; } = string.Empty;   
    }
}
