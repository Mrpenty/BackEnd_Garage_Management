using Garage_Management.Application.DTOs.Auth;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Đăng nhập cho khách hàng (hỗ trợ cả OTP và mật khẩu)
        /// </summary>
        Task<ApiResponse<LoginResponse>> LoginCustomerAsync(CustomerLoginRequest dto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gửi mã OTP đến số điện thoại của khách hàng
        /// </summary>
        Task<ApiResponse<User>> SendOtpLoginAsync(string phoneOrEmail, CancellationToken cancellationToken = default);
        /// <summary>
        /// Duyệt OTP đến số điện thoại dk của khách hàng
        /// </summary>
        Task<ApiResponse<User>> VerifyOtpAndActivateAsync(VerifyOtpRequest request, CancellationToken ct = default);

        /// <summary>
        /// Gửi lại mã OTP đến số điện thoại của khách hàng
        /// </summary>
        Task<ApiResponse<object>> ResendOtpAsync(ResendOtpRequest request, CancellationToken ct = default);
        /// <summary>
        /// Đăng nhập cho nhân viên gara (Email + Password)
        /// </summary>
        Task<ApiResponse<LoginResponse>> LoginStaffAsync(StaffLoginRequest dto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Đăng ký tự động tài khoản khách hàng
        /// </summary>
        Task<ApiResponse<User>> RegisterCustomerAsync(CustomerRegisterRequest dto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Đăng xuất người dùng hiện tại
        /// </summary>
        Task<ApiResponse<object>> LogoutAsync();

        /// <summary>
        /// Yêu cầu đặt lại mật khẩu (gửi link hoặc OTP reset)
        /// </summary>
        Task<ApiResponse<User>> ForgotPasswordAsync(string emailOrPhone, CancellationToken cancellationToken = default);

        /// <summary>
        /// Thực hiện đặt lại mật khẩu mới bằng token
        /// </summary>
        Task<ApiResponse<User>> ChangePasswordAsync(ChangePasswordRequest dto, CancellationToken cancellationToken = default);
    }
}
