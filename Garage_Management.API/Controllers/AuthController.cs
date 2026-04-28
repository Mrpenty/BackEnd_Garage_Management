using Garage_Management.Application.DTOs.Auth;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Garage_Management.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenCookieService _tokenCookieService;
       

        public AuthController(IAuthService authService, ITokenCookieService tokenCookieService)
        {
            _authService = authService;
            _tokenCookieService = tokenCookieService;
        }

        /// <summary>
        /// Đăng nhập cho khách hàng (hỗ trợ cả mật khẩu và OTP qua SMS)
        /// </summary>
        /// <param name="request">Thông tin đăng nhập (số điện thoạil + password hoặc OTP)</param>
        /// <remarks>
        /// - Nếu useOtp = true → cần gửi OTP trước bằng endpoint /send-otp
        /// - Nếu useOtp = false → dùng mật khẩu
        /// - Trả về token + refresh token nếu thành công
        /// </remarks>
        [HttpPost("customer/login")]
        public async Task<IActionResult> LoginCustomer([FromBody] CustomerLoginRequest request)
        {
            try
            {
                var result = await _authService.LoginCustomerAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<LoginResponse>.ErrorResponse(ex.Message));
            }
            catch
            {
                return StatusCode(500, ApiResponse<LoginResponse>.ErrorResponse("Lỗi hệ thống"));
            }
        }

        /// <summary>
        /// Gửi mã OTP đến số điện thoại hoặc email của khách hàng
        /// <param name="request">Số điện thoại hoặc email</param>
        /// - Sử dụng Twilio Verify để gửi OTP tự động
        /// - OTP có hiệu lực 1P tùy cấu hình Twilio
        /// </summary>
        [HttpPost("send-otp-Forlogin")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            try
            {
                var result = await _authService.SendOtpLoginAsync(request.PhoneOrEmail);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Lỗi hệ thống"));
            }
        }
        /// <summary>
        /// Đăng ký tài khoản khách hàng mới (self-registration)
        /// </summary>
        /// <param name="request">Thông tin đăng ký (số điện thoại, mật khẩu, họ tên?)</param>
        /// - Tự động gán role "Customer"
        /// <remarks>
        [HttpPost("customer/register")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegisterRequest request)
        {
            try
            {
                var result = await _authService.RegisterCustomerAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<User>.ErrorResponse(ex.Message));
            }
            catch
            {
                return StatusCode(500, ApiResponse<User>.ErrorResponse("Lỗi hệ thống"));
            }
        }
        /// <summary>
        /// Xác thực OTP và kích hoạt tài khoản khách hàng (sau khi đăng ký hoặc khi số điện thoại chưa verify)
        /// </summary>
        /// <param name="request">UserId + OTP</param>
        /// <remarks>
        /// - Sau khi verify OTP thành công: 
        ///   - Set PhoneNumberConfirmed = true
        ///   - Set IsActive = true
        /// - Khách hàng có thể đăng nhập ngay sau đó
        /// </remarks>
        [HttpPost("customer/verifyPhonenumber")]
        public async Task<IActionResult> VerifyCustomerAccount([FromBody] VerifyOtpRequest request)
        {
            try
            {
                var result = await _authService.VerifyOtpAndActivateAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Lỗi hệ thống"));
            }
        }
        /// <summary>
        /// Gửi lại mã OTP cho khách hàng (dùng khi không nhận được hoặc OTP hết hạn)
        /// </summary>
        [HttpPost("customer/resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
        {
            try
            {
                var result = await _authService.ResendOtpAsync(request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Lỗi hệ thống"));
            }
        }
        
        /// <summary>
        /// Đăng nhập cho nhân viên gara (email + password)
        /// </summary>
        /// <param name="request">Email và mật khẩu của nhân viên</param>
        /// <remarks>
        /// - Chỉ chấp nhận tài khoản có role khác "Customer"
        /// </remarks>
        [HttpPost("staff/login")]
        public async Task<IActionResult> LoginStaff([FromBody] StaffLoginRequest request)
        {
            try
            {
                var result = await _authService.LoginStaffAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<LoginResponse>.ErrorResponse(ex.Message));
            }
            catch
            {
                return StatusCode(500, ApiResponse<LoginResponse>.ErrorResponse("Lỗi hệ thống"));
            }
        }

        /// <summary>
        /// Đăng xuất người dùng hiện tại
        /// </summary>
        /// <remarks>
        /// - Xóa session cookie (nếu dùng cookie auth)
        /// - Nếu dùng JWT → client cần xóa token ở phía client
        /// </remarks>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return Ok(new { message = "Đăng xuất thành công" });
        }

        /// <summary>
        /// Yêu cầu đặt lại mật khẩu (forgot password)
        /// </summary>
        /// <param name="request">Email hoặc số điện thoại</param>
        /// <remarks>
        /// - Gửi link reset password qua email hoặc SMS
        /// - Token reset có thời hạn (thường 1-2 giờ)
        /// </remarks>
        /// <response code="200">Đã gửi link/SMS đặt lại mật khẩu</response>
        /// <response code="400">Không tìm thấy tài khoản (MSG10)</response>
        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _authService.ForgotPasswordAsync(request.Email);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Đặt lại mật khẩu mới 
        /// </summary>
        /// <param name="request"> mật khẩu mới</param>
        /// <remarks>
        /// - Mật khẩu mới phải đáp ứng yêu cầu bảo mật (MSG09 nếu không hợp lệ)
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _authService.ChangePasswordAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}

