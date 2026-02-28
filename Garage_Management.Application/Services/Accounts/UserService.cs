using Garage_Management.Application.DTOs.User;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.Accounts
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmployeeRepository _employeeRepository;  
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserService(
            UserManager<User> userManager,
            ICustomerRepository customerRepository,
            IEmployeeRepository employeeRepository,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _customerRepository = customerRepository;
            _employeeRepository = employeeRepository;
            _roleManager = roleManager;
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
    }
}
