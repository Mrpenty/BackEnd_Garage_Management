using Garage_Management.Application.DTOs.Services;
using Garage_Management.Application.DTOs.ServiceTasks;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Application.Interfaces.Services.Auth;
using Garage_Management.Application.Services.Auth;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _repo;
        private readonly ICurrentUserService _currentUser;

        public ServiceService(IServiceRepository repo, ICurrentUserService currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        public ServiceService(IServiceRepository repo)
        {
            _repo = repo;
            _currentUser = new NullCurrentUserService();
        }

        public async Task<ServiceResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            if (id <= 0) return null;
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<PagedResult<ServiceResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(page, pageSize, ct);
            return new PagedResult<ServiceResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = paged.PageData.Select(Map).ToList()
            };
        }

        public async Task<ServiceResponse> CreateAsync(ServiceCreateRequest request, CancellationToken ct = default)
        {
            if (!(_currentUser.IsAdmin() || _currentUser.IsInRole("Supervisor") || _currentUser.IsInRole("Accountant")))
                throw new UnauthorizedAccessException("Chỉ Supervisor hoặc Accountant được tạo dịch vụ");

            if (string.IsNullOrWhiteSpace(request.ServiceName))
                throw new InvalidOperationException("ServiceName không hợp lệ");
            var normalizedName = request.ServiceName.Trim();
            if (await _repo.ExistsByNameAsync(normalizedName, ct))
                throw new InvalidOperationException("ServiceName đã tồn tại");

            var entity = new Service
            {
                ServiceName = normalizedName,
                BasePrice = null,
                Description = request.Description,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUser.GetCurrentUserId()
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<ServiceResponse?> UpdatePriceAsync(int id, ServicePriceUpdateRequest request, CancellationToken ct = default)
        {
            if (!(_currentUser.IsAdmin() || _currentUser.IsInRole("Accountant")))
                throw new UnauthorizedAccessException("Chỉ Accountant được cập nhật giá dịch vụ");

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (request.BasePrice <= 0)
                throw new InvalidOperationException("BasePrice phải lớn hơn 0");

            entity.BasePrice = request.BasePrice;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _currentUser.GetCurrentUserId();

            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<ServiceResponse?> UpdateStatusAsync(int id, bool isActive, CancellationToken ct = default)
        {
            if (!(_currentUser.IsAdmin() || _currentUser.IsInRole("Supervisor") || _currentUser.IsInRole("Accountant")))
                throw new UnauthorizedAccessException("Chỉ Supervisor hoặc Accountant được đổi trạng thái dịch vụ");

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (isActive && (entity.BasePrice == null || entity.BasePrice <= 0))
                throw new InvalidOperationException("Không thể kích hoạt dịch vụ chưa có giá (BasePrice)");

            if (entity.IsActive == isActive)
                return Map(entity);

            entity.IsActive = isActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _currentUser.GetCurrentUserId();

            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<ServiceResponse?> DeactivateAsync(int id, CancellationToken ct = default)
        {
            return await UpdateStatusAsync(id, false, ct);
        }

        public async Task<List<ServiceResponse>> GetByVehicleTypeAsync(int vehicleTypeId, CancellationToken ct = default)
        {
            if (vehicleTypeId <= 0)
                throw new InvalidOperationException("VehicleTypeId không hợp lệ");

            var data = await _repo.GetByVehicleTypeAsync(vehicleTypeId, ct);
            return data.Select(Map).ToList();
        }

        public async Task<PagedResult<ServiceVehicleTypePairResponse>> GetServiceVehicleTypePairsAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var paged = await _repo.GetServiceVehicleTypePairsPagedAsync(page, pageSize, ct);
            return new PagedResult<ServiceVehicleTypePairResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = paged.PageData.Select(x => new ServiceVehicleTypePairResponse
                {
                    ServiceId = x.ServiceId,
                    ServiceName = x.Service.ServiceName,
                    ServiceIsActive = x.Service.IsActive,
                    VehicleTypeId = x.VehicleTypeId,
                    VehicleTypeName = x.VehicleType.TypeName,
                    VehicleTypeIsActive = x.VehicleType.IsActive
                }).ToList()
            };
        }

        private static ServiceResponse Map(Service entity)
        {
            var tasks = entity.ServiceTasks ?? new List<ServiceTask>();
            return new ServiceResponse
            {
                ServiceId = entity.ServiceId,
                ServiceName = entity.ServiceName,
                BasePrice = entity.BasePrice,
                Description = entity.Description,
                TotalEstimateMinute = tasks.Sum(x => (long)x.EstimateMinute),
                ServiceTasks = tasks
                    .OrderBy(x => x.TaskOrder)
                    .Select(x => new ServiceTaskResponse
                    {
                        ServiceTaskId = x.ServiceTaskId,
                        ServiceId = x.ServiceId,
                        TaskName = x.TaskName,
                        TaskOrder = x.TaskOrder,
                        EstimateMinute = x.EstimateMinute,
                        Note = x.Note,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    })
                    .ToList(),
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
