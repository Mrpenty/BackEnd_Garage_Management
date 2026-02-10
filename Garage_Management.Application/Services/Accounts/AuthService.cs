using Garage_Management.Application.DTOs.Auth;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.Accounts
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ISmsService _smsService;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole<int>> roleManager,
            ISmsService smsService,
            IConfiguration configuration,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _smsService = smsService;           
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public Task<ApiResponse<User>> ForgotPasswordAsync(string emailOrPhone, CancellationToken cancellationToken = default)
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
                return new ApiResponse<LoginResponse>{Success = false,Message = "MSG03"};
            }

            if (dto.UseOtp)
            {
                if (string.IsNullOrEmpty(dto.Otp))
                {
                    return new ApiResponse<LoginResponse>{Success = false,Message = "MSG01"};
                }

                //if (!await ValidateOtpAsync(user.Id, dto.Otp))
                //{
                //    return new ApiResponse<LoginResponse>
                //    {
                //        Success = false,
                //        Message = "Mã OTP không hợp lệ hoặc đã hết hạn"
                //    };
                //}

                await _signInManager.SignInAsync(user, isPersistent: false);

                return new ApiResponse<LoginResponse>{Success = true,Message = "Đăng nhập thành công"};
            }

            // Login bằng password
            if (string.IsNullOrEmpty(dto.Password))
            {
                return new ApiResponse<LoginResponse>{Success = false,Message = "MSG01"};
            }
            var signInResult = await _signInManager.PasswordSignInAsync(user,dto.Password,isPersistent: false,lockoutOnFailure: false);

            if (signInResult.Succeeded)
            {
                return new ApiResponse<LoginResponse>{Success = true,Message = "Đăng nhập thành công"};
            }

            return new ApiResponse<LoginResponse>{Success = false,Message = "MSG02"};
        }

        public async Task<ApiResponse<LoginResponse>> LoginStaffAsync(StaffLoginRequest dto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                return new ApiResponse<LoginResponse>{Success = false,Message = "MSG01"};
            }
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new ApiResponse<LoginResponse>{Success = false,Message = "MSG02"};
            }
            if (!user.IsActive)
            {
                return new ApiResponse<LoginResponse>{Success = false,Message = "MSG03"};
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
            return new ApiResponse<LoginResponse>{Success = false,Message = "MSG02"};
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public Task<ApiResponse<User>> RegisterCustomerAsync(CustomerRegisterRequest dto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<User>> SendOtpAsync(string phoneOrEmail, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(phoneOrEmail);

            var user = await _userManager.FindByEmailAsync(phoneOrEmail)
                    ?? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneOrEmail, cancellationToken);

            if (user == null || string.IsNullOrEmpty(user.PhoneNumber))
            {
                return new ApiResponse<User>{Success = false,Message = "MSG10"};
            }
            var otp = new Random().Next(100000, 999999).ToString();
            try
            {
                await _smsService.SendOtpAsync(user.PhoneNumber, otp);
                return new ApiResponse<User>{Success = true,Message = "Đã gửi mã OTP thành công"};
            }
            catch (Exception ex)
            {
                return new ApiResponse<User>{Success = false,Message = $"{"MSG04"}: {ex.Message}" };
            }
        }
    }
}
