using Azure.Core;
using Garage_Management.Application.DTOs.Auth;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Format;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Types;

namespace Garage_Management.Application.Services.Accounts
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IGenerateToken _tokenGenerator;
        private readonly ISmsService _smsService;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;


        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            IGenerateToken tokenGenerator,
            ISmsService smsService,
            IConfiguration configuration,
            IUserRepository userRepository,
            ICustomerRepository customerRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenGenerator = tokenGenerator;
            _smsService = smsService;           
            _configuration = configuration;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
        }

        public Task<ApiResponse<User>> ForgotPasswordAsync(string Phone, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<LoginResponse>> LoginCustomerAsync(CustomerLoginRequest dto, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(dto.PhoneNumber)
                    ?? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber, cancellationToken);

            if (user == null)
            {
                return new ApiResponse<LoginResponse>{Success = false, Message = "MSG04"};
            }
            if (!user.IsActive)
            {
                return new ApiResponse<LoginResponse>{Success = false,Message = "Tài khoản của bạn đã bị khóa." };
            }

            if (dto.UseOtp)
            {
                if (string.IsNullOrEmpty(dto.Otp))
                {
                    return new ApiResponse<LoginResponse>{Success = false,Message = "MSG01"};
                }

                var formatted = new FormatPhoneNumber().FormatPhoneNumberHepler(user.PhoneNumber);
                var verifyResult = await _smsService.VerifyOtpAsync(formatted, dto.Otp);
                bool isValid = verifyResult.IsValid;
                string verifyMessage = verifyResult.Message;

                if (!isValid)
                {
                    return ApiResponse<LoginResponse>.ErrorResponse(verifyMessage);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);

                var token = _tokenGenerator.GenerateJwtToken(user);
                var refreshToken = _tokenGenerator.GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);

                return ApiResponse<LoginResponse>.SuccessResponse(new LoginResponse
                {
                    UserId = user.Id,
                    FullName = user.UserName ?? "",
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "Customer",
                    AccessToken = token,
                    RefreshToken = refreshToken
                }, "Đăng nhập thành công");
            }

            // Login bằng password
            if (string.IsNullOrEmpty(dto.Password))
            {
                return new ApiResponse<LoginResponse>{Success = false,Message = "MSG01"};
            }
            var signInResult = await _signInManager.PasswordSignInAsync(user,dto.Password,isPersistent: false,lockoutOnFailure: false);

            if (signInResult.Succeeded)
            {
                var token = _tokenGenerator.GenerateJwtToken(user);
                var refreshToken = _tokenGenerator.GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(user);

                return ApiResponse<LoginResponse>.SuccessResponse(new LoginResponse
                {
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    UserId = user.Id,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    FullName = user.UserName ?? "",
                    Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "Customer"
                }, "Đăng nhập thành công");
            }

            return new ApiResponse<LoginResponse>{Success = false,Message = "MSG02"};
        }

        public async Task<ApiResponse<LoginResponse>> LoginStaffAsync(StaffLoginRequest dto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                return new ApiResponse<LoginResponse>{Success = false,Message = "Hãy nhập đầy đủ các trường thiếu"};
            }
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new ApiResponse<LoginResponse>{Success = false,Message = "Email không chính xác." };
            }
            if (!user.IsActive)
            {
                return new ApiResponse<LoginResponse>{Success = false,Message = "Tài khoản của bạn đã bị khóa." };
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.All(r => r == "Customer"))
            {
                return new ApiResponse<LoginResponse>{Success = false,Message = "Tài khoản không có quyền truy cập khu vực nhân viên"};
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user,dto.Password,lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return new ApiResponse<LoginResponse>{Success = true,Message = "Đăng nhập thành công"};
            }
            return new ApiResponse<LoginResponse>{Success = false,Message = "thông tin không chính xác." };
        }

        public async Task<ApiResponse<object>> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return ApiResponse<object>.SuccessResponse("Đăng xuất thành công");
        }

        public async Task<ApiResponse<User>> RegisterCustomerAsync(CustomerRegisterRequest dto, CancellationToken cancellationToken = default)
        {
            if (await _userRepository.ExistsByPhoneNumberAsync(dto.PhoneNumber, cancellationToken))
                return ApiResponse<User>.ErrorResponse("Số điện thoại này đã được đăng ký. Vui lòng đăng nhập hoặc sử dụng số khác.");

            if (dto.Password.Length < 9 || !dto.Password.Any(char.IsUpper) ||
                !dto.Password.Any(char.IsLower) || !dto.Password.Any(char.IsDigit))
                return ApiResponse<User>.ErrorResponse("Mật khẩu phải có ít nhất 9 ký tự, bao gồm 1 chữ hoa, 1 chữ thường và 1 chữ số.");

            var sendResult = await _smsService.SendOtpAsync(dto.PhoneNumber);
            if (!sendResult.Success)
                return ApiResponse<User>.ErrorResponse(sendResult.Message);

            var user = new User
            {
                UserName = dto.PhoneNumber,
                PhoneNumber = dto.PhoneNumber,
                Email = $"{dto.PhoneNumber}@customer.local",
                PhoneNumberConfirmed = false,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                return ApiResponse<User>.ErrorResponse(string.Join("; ", createResult.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            var customer = new Customer
            {
                UserId = user.Id,
                FirstName = dto.FirstName ?? "",
                LastName = dto.LastName ?? "",
                CreatedAt = DateTime.UtcNow
            };
            await _customerRepository.AddAsync(customer, cancellationToken);
            await _customerRepository.SaveAsync(cancellationToken);

            return ApiResponse<User>.SuccessResponse(user, "Tạo tài khoản thành công");
        }

        public async Task<ApiResponse<User>> SendOtpLoginAsync(string phoneOrEmail, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(phoneOrEmail);

            var user = await _userManager.FindByEmailAsync(phoneOrEmail)
                    ?? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneOrEmail, cancellationToken);

            if (user == null || string.IsNullOrEmpty(user.PhoneNumber))
            {
                return new ApiResponse<User> { Success = false, Message = "Không tìm thấy tài khoản nào với số điện thoại/email này." };
            }

            var (success, message) = await _smsService.SendOtpAsync(user.PhoneNumber);
            try
            {
                await _smsService.SendOtpAsync(user.PhoneNumber);
                return new ApiResponse<User>{Success = true,Message = "Đã gửi mã OTP thành công"};
            }
            catch (Exception ex)
            {
                return new ApiResponse<User>{Success = false,Message = $"{"MSG04"}: {ex.Message}" };
            }
        }

        public async Task<ApiResponse<User>> ChangePasswordAsync(ChangePasswordRequest dto, CancellationToken cancellationToken = default)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.OldPassword) ||
                string.IsNullOrWhiteSpace(dto.NewPassword) ||
                string.IsNullOrWhiteSpace(dto.ConfirmPassword))
            {
                return ApiResponse<User>.ErrorResponse("Vui lòng nhập đầy đủ thông tin");
            }

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return ApiResponse<User>.ErrorResponse("Mật khẩu xác nhận không khớp"); 
            }

            var userId = _userManager.GetUserId(_signInManager.Context.User);
            if (string.IsNullOrEmpty(userId))
            {
                return ApiResponse<User>.ErrorResponse("Người dùng chưa đăng nhập");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse<User>.ErrorResponse("Không tìm thấy người dùng");
            }

            if (!user.IsActive)
            {
                return ApiResponse<User>.ErrorResponse("Tài khoản của bạn đã bị khóa."); 
            }

            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, dto.OldPassword);
            if (!isCurrentPasswordValid)
            {
                return ApiResponse<User>.ErrorResponse("Mật khẩu hiện tại không chính xác");
            }

            //// Validate new password strength (BR-01)
            //var passwordValidationResult = await ValidatePasswordStrength(dto.NewPassword);
            //if (!passwordValidationResult.Success)
            //{
            //    return ApiResponse<User>.ErrorResponse(passwordValidationResult.Message);
            //}

            // Check if new password is same as current password
            if (dto.OldPassword == dto.NewPassword)
            {
                return ApiResponse<User>.ErrorResponse("Mật khẩu mới phải khác mật khẩu hiện tại");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                var errors = string.Join("; ", changePasswordResult.Errors.Select(e => e.Description));
                return ApiResponse<User>.ErrorResponse($"Không thể thay đổi mật khẩu: {errors}");
            }

            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = user.Id;
            await _userManager.UpdateAsync(user);
            return ApiResponse<User>.SuccessResponse(user, "Đổi mật khẩu thành công"); 
        }

        public async Task<ApiResponse<User>> VerifyOtpAndActivateAsync(VerifyOtpRequest request, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return ApiResponse<User>.ErrorResponse("Không tìm thấy tài khoản");

            if (user.IsActive)
                return ApiResponse<User>.ErrorResponse("Tài khoản đã được kích hoạt");

            var phoneNumber = user.PhoneNumber;
            if (string.IsNullOrEmpty(phoneNumber))
                return ApiResponse<User>.ErrorResponse("Số điện thoại của tài khoản không hợp lệ");
            var formatted = new FormatPhoneNumber().FormatPhoneNumberHepler(phoneNumber);
            if (string.IsNullOrEmpty(formatted))
                return ApiResponse<User>.ErrorResponse("Số điện thoại không hợp lệ");

            var verifyResult = await _smsService.VerifyOtpAsync(formatted, request.Otp);
            if (!verifyResult.IsValid)
                return ApiResponse<User>.ErrorResponse(verifyResult.Message);

            // Kích hoạt tài khoản & xác nhận số điện thoại
            user.IsActive = true;
            user.PhoneNumberConfirmed = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return ApiResponse<User>.SuccessResponse(null, "Tài khoản đã được kích hoạt thành công. Vui lòng đăng nhập.");
        }
        public async Task<ApiResponse<object>> ResendOtpAsync(ResendOtpRequest request, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            var (success, message) = await _smsService.SendOtpAsync(request.PhoneNumber);

            if (success)
            {
                return ApiResponse<object>.SuccessResponse("Đã gửi lại mã OTP thành công");
            }
            else
            {
                return ApiResponse<object>.ErrorResponse(message);
            }
        }
    }
}
