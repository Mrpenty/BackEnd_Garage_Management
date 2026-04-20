using Garage_Management.Application.Interfaces.Services.Auth;

namespace Garage_Management.Application.Services.Auth
{
    /// <summary>
    /// Fallback implementation dùng cho unit test: hành xử như Admin (không filter branch).
    /// Không đăng ký vào DI — chỉ dùng trong overloaded constructor.
    /// </summary>
    public class NullCurrentUserService : ICurrentUserService
    {
        public int? GetCurrentUserId() => null;
        public int? GetCurrentEmployeeId() => null;
        public int? GetCurrentCustomerId() => null;
        public int? GetCurrentBranchId() => null;
        public bool IsAdmin() => true;
        public bool IsInRole(string role) => false;
    }
}
