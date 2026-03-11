using Garage_Management.Application.DTOs.Inventories.SparePartBrands;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Inventories;
using System;
using System.Linq;

namespace Garage_Management.Application.Services.Inventories
{
    public class SparePartBrandService : ISparePartBrandService
    {
        private readonly ISparePartBrandRepository _repo;

        public SparePartBrandService(ISparePartBrandRepository repo)
        {
            _repo = repo;
        }

        public async Task<SparePartBrandResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<PagedResult<SparePartBrandResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(page, pageSize, ct);
            return new PagedResult<SparePartBrandResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = paged.PageData.Select(Map).ToList()
            };
        }

        public async Task<SparePartBrandResponse> CreateAsync(SparePartBrandCreateRequest request, CancellationToken ct = default)
        {
            var entity = new SparePartBrand
            {
                BrandName = request.BrandName,
                Description = request.Description
            };

            if (await _repo.HasExistAsync(entity.BrandName, null, ct))
                throw new InvalidOperationException("Hãng phụ tùng đã tồn tại");

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<SparePartBrandResponse?> UpdateAsync(int id, SparePartBrandUpdateRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (await _repo.HasSparePartsAsync(id, ct))
                throw new InvalidOperationException("Không thể cập nhật hãng phụ tùng vì đang có phụ tùng liên kết");

            entity.BrandName = request.BrandName;
            entity.Description = request.Description;

            if (await _repo.HasExistAsync(entity.BrandName, entity.SparePartBrandId, ct))
                throw new InvalidOperationException("Hãng phụ tùng đã tồn tại");

            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            if (await _repo.HasSparePartsAsync(id, ct))
                throw new InvalidOperationException("Không thể xóa hãng phụ tùng vì đang có phụ tùng liên kết");

            _repo.Delete(entity);
            await _repo.SaveAsync(ct);
            return true;
        }

        private static SparePartBrandResponse Map(SparePartBrand entity)
        {
            return new SparePartBrandResponse
            {
                SparePartBrandId = entity.SparePartBrandId,
                BrandName = entity.BrandName,
                Description = entity.Description
            };
        }
    }
}
