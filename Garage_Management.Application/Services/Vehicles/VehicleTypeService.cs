using Garage_Management.Application.DTOs.Vehicles.VehicleType;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Interfaces.Services.Vehiclies;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Vehiclies;
using System;
using System.Linq;

namespace Garage_Management.Application.Services.Vehicles
{
    public class VehicleTypeService : IVehicleTypeService
    {
        private readonly IVehicleTypeRepository _repo;

        public VehicleTypeService(IVehicleTypeRepository repo)
        {
            _repo = repo;
        }

        public async Task<VehicleTypeResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<PagedResult<VehicleTypeResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(page, pageSize, ct);
            return new PagedResult<VehicleTypeResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = paged.PageData.Select(Map).ToList()
            };
        }

        public async Task<VehicleTypeResponse> CreateAsync(VehicleTypeCreateRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.TypeName))
                throw new InvalidOperationException("TypeName không hợp lệ");

            var typeName = request.TypeName.Trim();
            if (await _repo.ExistsByTypeNameAsync(typeName, null, ct))
                throw new InvalidOperationException("Loại xe này đã tồn tại, nhập tên khác");

            var entity = new VehicleType
            {
                TypeName = typeName,
                Description = request.Description?.Trim(),
                IsActive = request.IsActive
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<VehicleTypeResponse?> DeactivateAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (!entity.IsActive)
                return Map(entity);

            entity.IsActive = false;
            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<VehicleTypeResponse?> ActivateAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (entity.IsActive)
                return Map(entity);

            entity.IsActive = true;
            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            if (await _repo.HasModelsAsync(id, ct) || await _repo.HasServiceMappingsAsync(id, ct))
                throw new InvalidOperationException("Không thể xóa loại xe vì đã phát sinh dữ liệu liên quan");

            _repo.Delete(entity);
            await _repo.SaveAsync(ct);
            return true;
        }

        private static VehicleTypeResponse Map(VehicleType entity)
        {
            return new VehicleTypeResponse
            {
                VehicleTypeId = entity.VehicleTypeId,
                TypeName = entity.TypeName,
                Description = entity.Description,
                IsActive = entity.IsActive
            };
        }
    }
}
