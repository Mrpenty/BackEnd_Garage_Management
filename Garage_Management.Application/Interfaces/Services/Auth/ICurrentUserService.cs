namespace Garage_Management.Application.Interfaces.Services.Auth
{
    /// <summary>
    /// Đọc thông tin người dùng hiện tại từ HttpContext (JWT claims).
    /// Dùng trong service layer để filter dữ liệu theo chi nhánh.
    /// </summary>
    public interface ICurrentUserService
    {
        int? GetCurrentUserId();
        int? GetCurrentEmployeeId();
        int? GetCurrentCustomerId();

        /// <summary>
        /// BranchId của nhân viên đang login. Null nếu là Customer hoặc chưa đăng nhập.
        /// </summary>
        int? GetCurrentBranchId();

        bool IsAdmin();
        bool IsInRole(string role);
    }
}
