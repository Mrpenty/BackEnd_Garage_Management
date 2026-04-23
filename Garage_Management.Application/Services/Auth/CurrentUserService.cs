using Garage_Management.Application.Interfaces.Services.Auth;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Security.Claims;

namespace Garage_Management.Application.Services.Auth
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public int? GetCurrentUserId()
        {
            var value = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) ? id : null;
        }

        public int? GetCurrentEmployeeId()
        {
            var value = User?.FindFirst("EmployeeId")?.Value;
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) ? id : null;
        }

        public int? GetCurrentCustomerId()
        {
            var value = User?.FindFirst("CustomerId")?.Value;
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) ? id : null;
        }

        public int? GetCurrentBranchId()
        {
            var value = User?.FindFirst("BranchId")?.Value;
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) ? id : null;
        }

        public bool IsAdmin() => IsInRole("Admin");

        public bool IsInRole(string role)
        {
            return User?.IsInRole(role) ?? false;
        }
    }
}
