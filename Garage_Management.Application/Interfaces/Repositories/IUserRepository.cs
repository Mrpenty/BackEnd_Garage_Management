using Garage_Management.Application.DTOs.User;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        /// <summary>
        /// Tạo mới một tài khoản User
        /// </summary>
        Task<User> CreateAsync(User entity, CancellationToken cancellationToken);

        /// <summary>
        /// Cập nhật thông tin User
        /// </summary>
        Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default);
        /// <summary>
        /// Lấy thông tin người dùng dựa trên tên đăng nhập (username)
        /// </summary>
        Task<User?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Cập nhật vai trò (role) cho người dùng
        /// </summary>
        Task<bool> UpdateRole(User user);
        /// <summary>
        /// Lưu hoặc cập nhật token xác thực
        /// </summary>
        Task<bool> SetAuthenTokenAsync(User user, string authProvider, string authTokenName, string token);

        /// <summary>
        /// Kiểm tra xem mật khẩu người dùng nhập có khớp với mật khẩu đã lưu (đã hash) hay không.
        /// </summary>
        Task<bool> CheckPasswordAsync(User user, string password);

        /// <summary>
        /// Tạo token dùng để reset mật khẩu (thường là token tạm thời, có thời hạn).
        /// Token này sẽ được gửi qua email hoặc SMS cho người dùng
        /// </summary>
        Task<string> GeneratePasswordResetTokenAsync(User user);

        /// <summary>
        /// Thực hiện reset mật khẩu bằng token đã được cấp trước đó
        /// </summary>
        Task<bool> ResetPasswordAsync(User user, string token, string newPassword);

        /// <summary>
        /// Lấy tên vai trò (role name) hiện tại của người dùng
        /// </summary>
        Task<string?> GetRoleNameByUser(User user);

        /// <summary>
        /// Kiểm tra xem Email đã tồn tại chưa
        /// </summary>
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Kiểm tra xem PhoneNumber đã tồn tại chưa
        /// </summary>
        Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Xóa thông tin cache liên quan đến người dùng (sau khi cập nhật thông tin, role..).
        /// </summary>
        Task RemoveCached(User user);

        /// <summary>
        /// Xóa mềm (soft delete) một User
        /// Đánh dấu IsDeleted = true, DeletedAt = now, DeletedBy = user thực hiện
        /// </summary>
        Task<bool> SoftDeleteAsync(int userId, int? deletedBy, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lấy danh sách người dùng có phân trang và filter theo các tiêu chí trong ParamQuery (như tên, email, vai trò...)
        /// </summary>
        /// <returns></returns>
        Task<PagedResult<User>> GetPagedAsync(ParamQuery query, CancellationToken ct = default);

        // Nếu cần filter role riêng
        Task<List<string>> GetUserRolesAsync(int userId, CancellationToken ct = default);
    }
}