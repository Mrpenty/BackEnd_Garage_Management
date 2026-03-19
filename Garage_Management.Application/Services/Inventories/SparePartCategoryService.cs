using Garage_Management.Application.DTOs.Inventories.SparePartCategories;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Inventories;

namespace Garage_Management.Application.Services.Inventories
{
    public class SparePartCategoryService : ISparePartCategoryService
    {
        private readonly ISparePartCategoryRepository _repo;

        public SparePartCategoryService(ISparePartCategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<SparePartCategoryResponse>> GetPagedAsync(ParamQuery query, bool onlyActive = false, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(query, onlyActive, ct);
            return new PagedResult<SparePartCategoryResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = paged.PageData.Select(Map).ToList()
            };
        }

        public async Task<SparePartCategoryResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<SparePartCategoryResponse> CreateAsync(SparePartCategoryCreateRequest request, CancellationToken ct = default)
        {
            var name = (request.CategoryName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Phải nhập tên cho nhóm phụ tùng");

            if (await _repo.HasExistAsync(name, null, ct))
                throw new InvalidOperationException("Nhóm phụ tùng đã tồn tại");

            var entity = new SparePartCategory
            {
                CategoryName = name,
                Description = request.Description,
                IsActive = request.IsActive
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<SparePartCategoryResponse?> UpdateAsync(int id, SparePartCategoryUpdateRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            var inputName = (request.CategoryName ?? string.Empty).Trim();
            var currentName = (entity.CategoryName ?? string.Empty).Trim();
            var isNameChanged = !string.Equals(inputName, currentName, StringComparison.OrdinalIgnoreCase);
            var hasChildren = await _repo.HasSparePartsAsync(id, ct);

            if (hasChildren && isNameChanged)
                throw new InvalidOperationException("Đã có dữ liệu liên quan, chỉ được cập nhật mô tả");

            if (!hasChildren && isNameChanged && await _repo.HasExistAsync(inputName, entity.CategoryId, ct))
                throw new InvalidOperationException("Nhóm phụ tùng đã tồn tại");

            if (!hasChildren)
                entity.CategoryName = inputName;

            entity.Description = request.Description;

            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            if (await _repo.HasSparePartsAsync(id, ct))
                throw new InvalidOperationException("Không thể xóa nhóm phụ tùng vì đang có phụ tùng liên kết");

            _repo.Delete(entity);
            await _repo.SaveAsync(ct);
            return true;
        }
        public async Task<SparePartCategoryResponse> UpdateStatusAsync (int id, bool isActive, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (entity.IsActive != isActive)
            {
                entity.IsActive = isActive;
                _repo.Update(entity);
                await _repo.SaveAsync(ct);
            }
            return Map(entity);
        }
        private static SparePartCategoryResponse Map(SparePartCategory entity)
        {
            return new SparePartCategoryResponse
            {
                CategoryId = entity.CategoryId,
                CategoryName = entity.CategoryName,
                Description = entity.Description,
                IsActive = entity.IsActive
            };
        }
    }
}
