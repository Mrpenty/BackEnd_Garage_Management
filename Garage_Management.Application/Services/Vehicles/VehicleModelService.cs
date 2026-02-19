using Garage_Management.Application.DTOs.Vehicles.VehicleModel;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Interfaces.Services.Vehiclies;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Vehiclies;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.Vehicles
{
    public class VehicleModelService : IVehicleModelService
    {
        private readonly IVehicleModelRepository _repo;
        private readonly IVehicleBrandRepository _brandRepo;

        public VehicleModelService(IVehicleModelRepository repo, IVehicleBrandRepository brandRepo)
        {
            _repo = repo;
            _brandRepo = brandRepo;
        }

        public async Task<VehicleModelResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<PagedResult<VehicleModelResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(page, pageSize, ct);
            return new PagedResult<VehicleModelResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = paged.PageData.Select(Map).ToList()
            };
        }

        public async Task<VehicleModelResponse> CreateAsync(VehicleModelCreateRequest request, CancellationToken ct = default)
        {
            var brand = await _brandRepo.GetByIdAsync(request.BrandId);
            if (brand == null)
                throw new InvalidOperationException("BrandId không tồn tại");

            if (await _repo.ExistsAsync(request.BrandId, request.ModelName, null, ct))
                throw new InvalidOperationException("ModelName đã tồn tại trong brand này");

            var entity = new VehicleModel
            {
                BrandId = request.BrandId,
                ModelName = request.ModelName.Trim(),
                IsActive = request.isActive
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<VehicleModelResponse?> UpdateAsync(int id, VehicleModelUpdate request, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            var brand = await _brandRepo.GetByIdAsync(request.BrandId);
            if (brand == null)
                throw new InvalidOperationException("BrandId không tồn tại");

            if (await _repo.ExistsAsync(request.BrandId, request.ModelName, id, ct))
                throw new InvalidOperationException("ModelName đã tồn tại");

            if (await _repo.HasVehiclesAsync(id, ct))
                throw new InvalidOperationException("Không thể cập nhật vì đang có xe liên kết");

            entity.BrandId = request.BrandId;
            entity.ModelName = request.ModelName.Trim();
            entity.IsActive = request.isActive;
            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<bool> DeActiveAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            if (await _repo.HasVehiclesAsync(id, ct))
                throw new InvalidOperationException("Không thể xóa vì đang có xe liên kết");

            entity.IsActive = false;
            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return true;
        }

        private static VehicleModelResponse Map(VehicleModel entity)
        {
            return new VehicleModelResponse
            {
                ModelId = entity.ModelId,
                BrandId = entity.BrandId,
                ModelName = entity.ModelName,
                isActive = entity.IsActive
            };
        }
    }
}
