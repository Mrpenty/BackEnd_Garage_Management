using Garage_Management.Application.DTOs.Vehicles.VehicleBrand;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Vehiclies;
using System;
using System.Linq;

namespace Garage_Management.Application.Services.Vehicles
{
    public class VehicleBrandService : IVehicleBrandService
    {
        private readonly IVehicleBrandRepository _repo;

        public VehicleBrandService(IVehicleBrandRepository repo)
        {
            _repo = repo;
        }

        public async Task<VehicleBrandResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<PagedResult<VehicleBrandResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(page, pageSize, ct);
            return new PagedResult<VehicleBrandResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = paged.PageData.Select(Map).ToList()
            };
        }

        public async Task<VehicleBrandResponse> CreateAsync(VehicleBrandCreateRequest request, CancellationToken ct = default)
        {
            var entity = new VehicleBrand
            {
                BrandName = request.BrandName
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<VehicleBrandResponse?> UpdateAsync(int id, VehicleBrandUpdate request, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (await _repo.HasVehiclesAsync(id, ct))
                throw new InvalidOperationException("Không thể cập nhật hãng xe vì đang có xe máy liên kết");

            entity.BrandName = request.BrandName;
            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            if (await _repo.HasVehiclesAsync(id, ct))
                throw new InvalidOperationException("Không thể xóa hãng xe vì đang có xe máy liên kết");

            _repo.Delete(entity);
            await _repo.SaveAsync(ct);
            return true;
        }

        private static VehicleBrandResponse Map(VehicleBrand entity)
        {
            return new VehicleBrandResponse
            {
                BrandId = entity.BrandId,
                BrandName = entity.BrandName
            };
        }

    }
}
