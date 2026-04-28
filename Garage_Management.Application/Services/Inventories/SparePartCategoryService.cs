using Garage_Management.Application.DTOs.Inventories.SparePartCategories;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services.Auth;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Application.Services.Auth;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Inventories;

namespace Garage_Management.Application.Services.Inventories
{
    public class SparePartCategoryService : ISparePartCategoryService
    {
        private readonly ISparePartCategoryRepository _repo;
        private readonly ICurrentUserService _currentUser;

        public SparePartCategoryService(ISparePartCategoryRepository repo, ICurrentUserService currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        public SparePartCategoryService(ISparePartCategoryRepository repo)
        {
            _repo = repo;
            _currentUser = new NullCurrentUserService();
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
            if (id <= 0) return null;
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<SparePartCategoryResponse> CreateAsync(SparePartCategoryCreateRequest request, CancellationToken ct = default)
        {
            if (!_currentUser.IsInRole("Supervisor"))
                throw new UnauthorizedAccessException("Chỉ Supervisor được tạo nhóm phụ tùng");

            var name = (request.CategoryName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Phải nhập tên cho nhóm phụ tùng");
            if (name.Length > 100)
                throw new InvalidOperationException("Tên nhóm phụ tùng không được vượt quá 100 ký tự");

            var description = request.Description?.Trim();
            if (description != null && description.Length > 255)
                throw new InvalidOperationException("Mô tả không được vượt quá 255 ký tự");

            if (await _repo.HasExistAsync(name, null, ct))
                throw new InvalidOperationException("Nhóm phụ tùng đã tồn tại");

            var entity = new SparePartCategory
            {
                CategoryName = name,
                Description = description,
                IsActive = request.IsActive,
                CreatedBy = _currentUser.GetCurrentUserId()
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<SparePartCategoryResponse?> UpdateAsync(int id, SparePartCategoryUpdateRequest request, CancellationToken ct = default)
        {
            if (!_currentUser.IsInRole("Supervisor"))
                throw new UnauthorizedAccessException("Chỉ Supervisor được cập nhật nhóm phụ tùng");

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            var inputName = (request.CategoryName ?? string.Empty).Trim();
            var currentName = (entity.CategoryName ?? string.Empty).Trim();
            var isNameChanged = !string.Equals(inputName, currentName, StringComparison.OrdinalIgnoreCase);
            var hasChildren = await _repo.HasSparePartsAsync(id, ct);

            if (!hasChildren)
            {
                if (string.IsNullOrWhiteSpace(inputName))
                    throw new InvalidOperationException("Phải nhập tên cho nhóm phụ tùng");
                if (inputName.Length > 100)
                    throw new InvalidOperationException("Tên nhóm phụ tùng không được vượt quá 100 ký tự");
            }

            var description = request.Description?.Trim();
            if (description != null && description.Length > 255)
                throw new InvalidOperationException("Mô tả không được vượt quá 255 ký tự");

            if (hasChildren && isNameChanged)
                throw new InvalidOperationException("Đã có dữ liệu liên quan, chỉ được cập nhật mô tả");

            if (!hasChildren && isNameChanged && await _repo.HasExistAsync(inputName, entity.CategoryId, ct))
                throw new InvalidOperationException("Nhóm phụ tùng đã tồn tại");

            if (!hasChildren)
                entity.CategoryName = inputName;

            entity.Description = description;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _currentUser.GetCurrentUserId();

            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<SparePartCategoryResponse?> UpdateStatusAsync(int id, bool isActive, CancellationToken ct = default)
        {
            if (!_currentUser.IsInRole("Supervisor"))
                throw new UnauthorizedAccessException("Chỉ Supervisor được đổi trạng thái nhóm phụ tùng");

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (entity.IsActive != isActive)
            {
                entity.IsActive = isActive;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = _currentUser.GetCurrentUserId();
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
