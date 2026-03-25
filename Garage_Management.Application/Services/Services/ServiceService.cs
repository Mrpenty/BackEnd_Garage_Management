using Garage_Management.Application.DTOs.Services;
using Garage_Management.Application.DTOs.ServiceTasks;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Services;
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

        public ServiceService(IServiceRepository repo)
        {
            _repo = repo;
        }

        public async Task<ServiceResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
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
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<ServiceResponse?> UpdatePriceAsync(int id, ServicePriceUpdateRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (request.BasePrice <= 0)
                throw new InvalidOperationException("BasePrice phải lớn hơn 0");

            entity.BasePrice = request.BasePrice;
            entity.UpdatedAt = DateTime.UtcNow;

            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<ServiceResponse?> UpdateStatusAsync(int id, bool isActive, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (entity.IsActive == isActive)
                return Map(entity);

            entity.IsActive = isActive;
            entity.UpdatedAt = DateTime.UtcNow;

            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            if (await _repo.HasDependenciesAsync(id, ct))
                throw new InvalidOperationException("Không thể xóa dịch vụ vì đã phát sinh dữ liệu liên quan");

            _repo.Delete(entity);
            await _repo.SaveAsync(ct);
            return true;
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
            return new ServiceResponse
            {
                ServiceId = entity.ServiceId,
                ServiceName = entity.ServiceName,
                BasePrice = entity.BasePrice,
                Description = entity.Description,
                TotalEstimateMinute = entity.ServiceTasks.Sum(x => (long)x.EstimateMinute),
                ServiceTasks = entity.ServiceTasks
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
