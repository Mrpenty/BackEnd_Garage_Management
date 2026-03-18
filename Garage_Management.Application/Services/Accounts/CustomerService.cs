using Garage_Management.Application.DTOs.User;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Interfaces.Services.Accounts;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.Accounts
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVehicleRepository _vehicleRepository; 
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CustomerService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerService(
            ICustomerRepository customerRepository,
            IUserRepository userRepository,
            IVehicleRepository vehicleRepository,
            UserManager<User> userManager,
            ILogger<CustomerService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _vehicleRepository = vehicleRepository;
            _userManager = userManager;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResponse<PagedResult<CustomerDto>>> GetPagedAsync(ParamQuery query, CancellationToken ct = default)
        {
            try
            {
                var q = _customerRepository.GetAll()
                    .Include(c => c.User)                   
                    .Include(c => c.Vehicles)                
                        .ThenInclude(v => v.Brand)           
                        .ThenInclude(v => v.Models)           
                    .AsQueryable();

                // Lọc theo từ khóa (tên, sđt, email, biển số)
                if (!string.IsNullOrWhiteSpace(query.Search))
                {
                    var search = query.Search.Trim().ToLower();

                    q = q.Where(c =>
                        (c.FirstName ?? "").ToLower().Contains(search) ||
                        (c.LastName ?? "").ToLower().Contains(search) ||
                        (c.User != null && (c.User.PhoneNumber ?? "").Contains(search)) ||
                        (c.User != null && (c.User.Email ?? "").ToLower().Contains(search)) ||
                        c.Vehicles.Any(v => (v.LicensePlate ?? "").ToLower().Contains(search))
                    );
                }

                // Sắp xếp theo CreatedAt giảm dần (mới nhất trước)
                q = q.OrderByDescending(c => c.CreatedAt);

                // Đếm tổng số record trước khi phân trang
                var total = await q.CountAsync(ct);

                // Phân trang & mapping sang DTO
                var items = await q
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .Select(c => new CustomerDto
                    {
                        CustomerId = c.CustomerId,
                        FullName = $"{(c.FirstName ?? "").Trim()} {(c.LastName ?? "").Trim()}".Trim(),
                        PhoneNumber = c.User != null ? (c.User.PhoneNumber ?? "") : "",
                        Email = c.User != null ? (c.User.Email ?? "") : "",
                        Address = c.Address ?? "",
                        UserId = c.UserId,
                        CreatedAt = c.CreatedAt,
                        Vehicles = c.Vehicles.Select(v => new VehicleDto
                        {
                            VehicleId = v.VehicleId,
                            LicensePlate = v.LicensePlate ?? "",
                            Brand = v.Brand != null ? v.Brand.BrandName ?? "" : "",     // ← sửa lỗi ở đây
                            Model = v.Model != null ? v.Model.ModelName ?? "" : "",
                            Year = v.Year
                        }).ToList()
                    })
                    .ToListAsync(ct);

                var pagedResult = new PagedResult<CustomerDto>
                {
                    PageData = items,
                    Total = total,
                    Page = query.Page,
                    PageSize = query.PageSize
                };

                return ApiResponse<PagedResult<CustomerDto>>.SuccessResponse(pagedResult, "Lấy danh sách khách hàng thành công");
            }
            catch (Exception )
            {
                return ApiResponse<PagedResult<CustomerDto>>.ErrorResponse("Có lỗi xảy ra khi lấy danh sách khách hàng");
            }
        }

        public async Task<ApiResponse<CustomerDto>> CreateCustomerByReceptionistAsync(CreateCustomerRequest request,CancellationToken ct = default)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || !httpContext.User.Identity?.IsAuthenticated == true)
            {
                return ApiResponse<CustomerDto>.ErrorResponse("Không tìm thấy thông tin người dùng đăng nhập");
            }

            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var receptionistUserId))
            {
                return ApiResponse<CustomerDto>.ErrorResponse("Không thể xác định ID của nhân viên đang thực hiện");
            }
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return ApiResponse<CustomerDto>.ErrorResponse("Vui lòng nhập đầy đủ họ, tên và số điện thoại.");
            }

            // 2. Kiểm tra trùng số điện thoại
            if (await _customerRepository.GetAll().AnyAsync(c => c.User.PhoneNumber == request.PhoneNumber, ct))
            {
                return ApiResponse<CustomerDto>.ErrorResponse("Số điện thoại này đã được đăng ký.");
            }

            // 3. Tạo User (tùy chọn - nếu muốn khách hàng có thể đăng nhập sau)
            User? newUser = null;
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                newUser = new User
                {
                    UserName = request.PhoneNumber,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = receptionistUserId
                };

                var createUserResult = await _userManager.CreateAsync(newUser, "TempPass123!"); // mật khẩu tạm, nên thay bằng OTP flow
                if (!createUserResult.Succeeded)
                {
                    return ApiResponse<CustomerDto>.ErrorResponse(string.Join("; ", createUserResult.Errors.Select(e => e.Description)));
                }

                await _userManager.AddToRoleAsync(newUser, "Customer");
            }

            if (newUser == null)
            {
                return ApiResponse<CustomerDto>.ErrorResponse("Không thể tạo tài khoản người dùng cho khách hàng.");
            }

            // 4. Tạo Customer
            var customer = new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address = request.Address,
                UserId = newUser.Id,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = receptionistUserId
            };

            _customerRepository.Add(customer);
            await _customerRepository.SaveAsync(ct);

            var dto = new CustomerDto
            {
                CustomerId = customer.CustomerId,
                FullName = $"{customer.FirstName} {customer.LastName}".Trim(),
                PhoneNumber = request.PhoneNumber,
                Email = newUser?.Email,
                Address = customer.Address,
                UserId = customer.UserId,
                CreatedAt = customer.CreatedAt,
                //Vehicles = (await _vehicleRepository.GetAll()
                //    .Where(v => v.CustomerId == customer.CustomerId)
                //    .Select(v => new VehicleDto
                //    {
                //        VehicleId = v.VehicleId,
                //        LicensePlate = v.LicensePlate,    
                //        Brand = v.Brand.BrandName,
                //        Model = v.Model.ModelName,
                //        Year = v.Year,
                //    }).ToListAsync(ct))
            };

            return ApiResponse<CustomerDto>.SuccessResponse(dto, "Tạo khách hàng thành công");
        }
    }
}