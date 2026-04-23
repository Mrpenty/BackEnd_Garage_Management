using Garage_Management.Application.DTOs.Employee;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services.Accounts;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.Accounts
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IUserRepository _userRepo;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public EmployeeService(
            IEmployeeRepository employeeRepo,
            IUserRepository userRepo,
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _employeeRepo = employeeRepo;
            _userRepo = userRepo;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _roleManager = roleManager;
        }

        public async Task<ApiResponse<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeRequest request, CancellationToken ct = default)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var currentUserId = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value != null
                ? int.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value)
                : 0;

            // Kiểm tra quyền
            if (!httpContext?.User?.IsInRole("Admin") == true)
                return ApiResponse<EmployeeDto>.ErrorResponse("Chỉ Admin mới được tạo nhân viên");

            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName)  || string.IsNullOrWhiteSpace(request.Role))
            {
                return ApiResponse<EmployeeDto>.ErrorResponse("Vui lòng nhập đầy đủ thông tin bắt buộc ");
            }
            if (request.BranchId <= 0)
            {
                return ApiResponse<EmployeeDto>.ErrorResponse("Phải chọn chi nhánh cho nhân viên");
            }
            if (await _userRepo.ExistsByEmailAsync(request.Email))
                return ApiResponse<EmployeeDto>.ErrorResponse("Email đã tồn tại");
            if (await _userRepo.ExistsByPhoneNumberAsync(request.PhoneNumber))
                return ApiResponse<EmployeeDto>.ErrorResponse("Số điện thoại đã tồn tại");
            // Kiểm tra Role có tồn tại trong hệ thống không
            var role = request.Role.ToUpper();
            if (!await _roleManager.RoleExistsAsync(role))
                return ApiResponse<EmployeeDto>.ErrorResponse($"Role '{request.Role}' không tồn tại trong hệ thống");

            // Tạo User trước
            var user = new User
            {
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                IsActive = true,
                CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                CreatedBy = currentUserId
            };

            var createUserResult = await _userManager.CreateAsync(user, "TempPass123!");
            if (!createUserResult.Succeeded)
            {
                var errors = string.Join("; ", createUserResult.Errors.Select(e => e.Description));
                return ApiResponse<EmployeeDto>.ErrorResponse($"Tạo tài khoản thất bại: {errors}");
            }
            // Gán role cho user
            var addRoleResult = await _userManager.AddToRoleAsync(user, request.Role.ToUpper());
            if (!addRoleResult.Succeeded)
            {
                var roleErrors = string.Join("; ", addRoleResult.Errors.Select(e => e.Description));
                return ApiResponse<EmployeeDto>.ErrorResponse($"Gán role thất bại: {roleErrors}");
            }
            Employee employee = null!;
            bool saved = false;

            // Tạo Employee
            do
            {
                var employeeCode = await GenerateEmployeeCodeAsync(role);

                employee = new Employee
                {
                    UserId = user.Id,
                    BranchId = request.BranchId,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    EmployeeCode = employeeCode,
                    Position = request.Position,
                    Status = WorkingStatus.Available,
                    IsActive = true,
                    StartWorkingDate = request.StartWorkingDate ?? TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                    CreatedBy = currentUserId,
                    CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
                };

                try
                {
                    _employeeRepo.Add(employee);
                    await _employeeRepo.SaveAsync(ct);
                    saved = true;
                }
                catch (DbUpdateException)
                {
                    saved = false;
                }

            } while (!saved);

            var response = new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                UserId = employee.UserId,
                BranchId = employee.BranchId,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                FullName = employee.FullName,
                EmployeeCode = employee.EmployeeCode,
                Position = employee.Position,
                Status = employee.Status,
                WorkingEmpStatus = employee.Status.ToString(),
                IsActive = employee.IsActive,
                StartWorkingDate = employee.StartWorkingDate,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email
            };

            return ApiResponse<EmployeeDto>.SuccessResponse(response, "Tạo nhân viên thành công");
        }
        private async Task<string> GenerateEmployeeCodeAsync(string role)
        {
            string prefix = role.ToUpper() switch
            {
                "MECHANIC" => "ME",
                "SUPERVISOR" => "SV",
                "RECEPTIONIST" => "RC",
                "STOCKER" => "ST",
                "ADMIN" => "AD",
                "ACCOUNTANT" => "AC",
                _ => "EM"
            };

            // Lấy code lớn nhất hiện tại
            var lastEmployee = await _employeeRepo
                .GetAll()
                .Where(e => e.EmployeeCode.StartsWith(prefix))
                .OrderByDescending(e => e.EmployeeCode)
                .FirstOrDefaultAsync();
            int nextNumber = 1;

            if (lastEmployee != null)
            {
                var numberPart = lastEmployee.EmployeeCode.Split('-').Last();
                if (int.TryParse(numberPart, out int num))
                {
                    nextNumber = num + 1;
                }
            }

            return $"{prefix}-{nextNumber:D3}";
        }
    }
}
