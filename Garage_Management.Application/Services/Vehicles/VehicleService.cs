using Garage_Management.Application.DTOs.Appointments;
using Garage_Management.Application.DTOs.Vehicles;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Interfaces.Services.Vehiclies;
using Garage_Management.Application.Repositories.Accounts;
using Garage_Management.Base.Common.Format;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.Vehicles
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _repo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public VehicleService(IVehicleRepository repo, ICustomerRepository customerRepo, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _customerRepo = customerRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<VehicleResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task <ApiResponse<PagedResult<VehicleResponse>>> GetMyVehicle(int page, int pageSize, CancellationToken ct = default)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || !httpContext.User.Identity?.IsAuthenticated == true)
            {
                return ApiResponse<PagedResult<VehicleResponse>>.ErrorResponse("Vui lòng đăng nhập để xem lịch hẹn");
            }

            // Lấy UserId từ claims (NameIdentifier thường là ID của User)
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var currentUserId))
            {
                return ApiResponse<PagedResult<VehicleResponse>>.ErrorResponse("Không thể xác định thông tin người dùng");
            }

            var userRoles = httpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            if (!userRoles.Contains("Customer"))
            {
                return ApiResponse<PagedResult<VehicleResponse>>.ErrorResponse("khách hàng chỉ có thể xem lịch hẹn cá nhân");
            }
            var customer = await _customerRepo.GetAll().FirstAsync(c => c.UserId == currentUserId, ct);

            if (customer == null)
            {
                return ApiResponse<PagedResult<VehicleResponse>>.SuccessResponse(
                    new PagedResult<VehicleResponse>
                    {
                        PageData = new List<VehicleResponse>(),
                        Total = 0,
                        Page = page,
                        PageSize = pageSize
                    },
                    "Bạn chưa có thông tin khách hàng nào trong hệ thống"
                );
            }

            int customerId = customer.CustomerId;
            var pagedData = await _repo.GetByCustomerIdAsync(page, pageSize, customerId, ct);

            if (pagedData == null || pagedData.PageData == null || !pagedData.PageData.Any())
            {
                return ApiResponse<PagedResult<VehicleResponse>>.SuccessResponse(
                    new PagedResult<VehicleResponse>
                    {
                        PageData = new List<VehicleResponse>(),
                        Total = 0,
                        Page = page,
                        PageSize = pageSize
                    },
                    "Bạn chưa có phương tiện nào"
                );
            }

            var vehicleResponses = pagedData.PageData.Select(Map).ToList();

            var result = new PagedResult<VehicleResponse>
            {
                PageData = vehicleResponses,
                Total = pagedData.Total,
                Page = pagedData.Page,
                PageSize = pagedData.PageSize
            };

            return ApiResponse<PagedResult<VehicleResponse>>.SuccessResponse(result, $"Lấy danh sách phương tiện thành công");
        }

        public async Task<ApiResponse<PagedResult<VehicleResponse>>> GetPagedAsync(ParamQuery query, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(query, ct);  

            if (!paged.Success)
            {
                return ApiResponse<PagedResult<VehicleResponse>>.ErrorResponse(paged.Message);
            }

            var responseData = paged.Data.PageData.Select(Map).ToList();  

            var result = new PagedResult<VehicleResponse>
            {
                Page = paged.Data.Page,
                PageSize = paged.Data.PageSize,
                Total = paged.Data.Total,
                PageData = responseData
            };

            if(result.PageData == null || !result.PageData.Any())
            {
                return ApiResponse<PagedResult<VehicleResponse>>.SuccessResponse(result, "Không có phương tiện nào");
            }

            return ApiResponse<PagedResult<VehicleResponse>>.SuccessResponse(result, "Lấy danh sách phương tiện thành công");
        }

        public async Task<VehicleResponse> CreateAsync(VehicleCreateRequest request, CancellationToken ct = default)
        {
            if (request.CustomerId <= 0)
                throw new InvalidOperationException("Thiếu CustomerId");

            var customer = await _customerRepo.GetByIdAsync(request.CustomerId);
            if (customer == null)
                throw new InvalidOperationException("CustomerId không tồn tại");

            var entity = new Vehicle
            {
                CustomerId = request.CustomerId,
                BrandId = request.BrandId,
                ModelId = request.ModelId,
                LicensePlate = request.LicensePlate,
                Year = request.Year,
                Vin = request.Vin,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<VehicleResponse?> UpdateAsync(int id, VehicleUpdateRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;
            //Cần suy nghĩ thêm về logic

            //if (await _repo.HasAppointmentsAsync(id, ct))
            //{
            //    throw new InvalidOperationException("Không thể cập nhật vì đang có xe liên kết");
            //}
            if (request.BrandId.HasValue) entity.BrandId = request.BrandId.Value;
            if (request.ModelId.HasValue) entity.ModelId = request.ModelId.Value;
            if (request.LicensePlate != null) entity.LicensePlate = request.LicensePlate;
            if (request.Year.HasValue) entity.Year = request.Year.Value;
            if (request.Vin != null) entity.Vin = request.Vin;
            if (request.UpdatedBy.HasValue) entity.UpdatedBy = request.UpdatedBy.Value;
            entity.UpdatedAt = DateTime.UtcNow;

            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            if(await _repo.HasAppointmentsAsync(id, ct))
            {
                throw new InvalidOperationException("Không thể xóa vì đang có xe liên kết");
            }

            _repo.Delete(entity);
            await _repo.SaveAsync(ct);
            return true;
        }

        private static VehicleResponse Map(Vehicle entity)
        {
            var formatter = new FormatDateTime();
            return new VehicleResponse
            {
                VehicleId = entity.VehicleId,
                CustomerId = entity.CustomerId,
                BrandId = entity.BrandId,
                BrandName = entity.Brand != null ? (entity.Brand.BrandName ?? string.Empty) : string.Empty,
                ModelId = entity.ModelId,
                ModelName = entity.Model != null ? (entity.Model.ModelName ?? string.Empty) : string.Empty,
                LicensePlate = entity.LicensePlate,
                Year = entity.Year,
                Vin = entity.Vin,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                CreatedAt = formatter.FormatToDdMmYyyy(entity.CreatedAt.ToString("o")) ?? "",
                UpdatedAt = entity.UpdatedAt.HasValue? formatter.FormatToDdMmYyyy(entity.UpdatedAt.Value.ToString("o")) ?? "": ""
            };
        }
    }
}
