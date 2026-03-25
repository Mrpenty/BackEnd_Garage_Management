using Garage_Management.Application.DTOs.User;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Application.Repositories.Accounts;
using Garage_Management.Base.Common.Format;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;
using static Garage_Management.Application.DTOs.User.UserRequest;

namespace Garage_Management.Application.Services.Accounts
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmployeeRepository _employeeRepository;  
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;

        public UserService(
            UserManager<User> userManager,
            ICustomerRepository customerRepository,
            IEmployeeRepository employeeRepository,
            RoleManager<IdentityRole<int>> roleManager,
            ILogger<UserService> logger,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
            _roleManager = roleManager;
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<ApiResponse<ProfileResponse>> GetCurrentUserProfileAsync(ClaimsPrincipal userClaims)
        {
            var userIdStr = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                return ApiResponse<ProfileResponse>.ErrorResponse("Không tìm thấy thông tin người dùng");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return ApiResponse<ProfileResponse>.ErrorResponse("Không tìm thấy tài khoản");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var profile = new ProfileResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt ?? user.CreatedAt,
                Roles = roles.ToList(),
                FullName = user.UserName ?? user.Email ?? ""
            };

            // Nếu là Customer
            if (roles.Contains("Customer"))
            {
                var customer = await _customerRepository.GetByUserIdAsync(user.Id);
                if (customer != null)
                {
                    profile.Customer = new ProfileResponse.CustomerInfo
                    {
                        CustomerId = customer.CustomerId,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        CreatedAt = customer.CreatedAt
                    };

                    profile.FullName = $"{customer.FirstName} {customer.LastName}".Trim();
                }
            }
            // Nếu là Employee/Staff
            else
            {
                var employee = await _employeeRepository.GetByUserIdAsync(user.Id);
                if (employee != null)
                {
                    profile.Employee = new ProfileResponse.EmployeeInfo
                    {
                        EmployeeId = employee.EmployeeId,
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        Position = employee.Position,
                    };

                    profile.FullName = $"{employee.FirstName} {employee.LastName}".Trim();
                }
            }

            return ApiResponse<ProfileResponse>.SuccessResponse(profile, "Lấy thông tin profile thành công");
        }

        public async Task<ApiResponse<PagedResult<UserRequest>>> GetPagedAsync(ParamQuery query, CancellationToken ct = default)
        {
            var formatter = new FormatDateTime();
            try
            {
                var pagedUsers = await _userRepository.GetPagedAsync(query, ct);

                var userDtos = new List<UserRequest>();
                foreach (var u in pagedUsers.PageData)
                {
                    var roles = await _userRepository.GetUserRolesAsync(u.Id, ct);

                    // Lấy FullName từ Customer (ưu tiên)
                    var customerFullName = await _customerRepository.GetAll()
                        .Where(c => c.UserId == u.Id)
                        .Select(c => $"{c.FirstName} {c.LastName}".Trim())
                        .FirstOrDefaultAsync(ct);

                    EmployeeInfo? employeeInfo = null;
                    var employee = await _employeeRepository.GetAll()
                        .FirstOrDefaultAsync(e => e.UserId == u.Id, ct);

                    if (employee != null)
                    {
                        employeeInfo = new UserRequest.EmployeeInfo
                        {
                            EmployeeId = employee.EmployeeId,
                            FirstName = employee.FirstName,
                            LastName = employee.LastName,
                            Position = employee.Position,
                        };
                    }

                    var fullName = !string.IsNullOrWhiteSpace(customerFullName)
                        ? customerFullName
                        : employee != null
                            ? $"{employee.FirstName} {employee.LastName}".Trim()
                            : "";

                    userDtos.Add(new UserRequest
                    {
                        UserId = u.Id,
                        UserName = u.UserName ?? "",
                        Email = u.Email ?? "",
                        PhoneNumber = u.PhoneNumber ?? "",
                        IsActive = u.IsActive,
                        CreatedAt = formatter.FormatToDdMmYyyy(u.CreatedAt.ToString("o")) ?? "",
                        UpdatedAt = u.UpdatedAt.HasValue ? formatter.FormatToDdMmYyyy(u.UpdatedAt.Value.ToString("o")) ?? "" : "",
                        Roles = roles,
                        FullName = fullName,
                        Employee = employeeInfo 
                    });
                }

                var result = new PagedResult<UserRequest>
                {
                    PageData = userDtos,
                    Total = pagedUsers.Total,
                    Page = pagedUsers.Page,
                    PageSize = pagedUsers.PageSize
                };

                return ApiResponse<PagedResult<UserRequest>>.SuccessResponse(result, "Lấy danh sách người dùng thành công");
            }
            catch (Exception)
            {
                return ApiResponse<PagedResult<UserRequest>>.ErrorResponse("Có lỗi xảy ra khi lấy danh sách người dùng");
            }
        }
    }
}
