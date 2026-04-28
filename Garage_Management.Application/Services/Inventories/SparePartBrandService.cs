using Garage_Management.Application.DTOs.Inventories.SparePartBrands;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services.Auth;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Application.Services.Auth;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Inventories;
using System;
using System.Linq;

namespace Garage_Management.Application.Services.Inventories
{
    public class SparePartBrandService : ISparePartBrandService
    {
        private readonly ISparePartBrandRepository _repo;
        private readonly ICurrentUserService _currentUser;

        public SparePartBrandService(ISparePartBrandRepository repo, ICurrentUserService currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        public SparePartBrandService(ISparePartBrandRepository repo)
        {
            _repo = repo;
            _currentUser = new NullCurrentUserService();
        }

        public async Task<SparePartBrandResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            if (id <= 0) return null;
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<PagedResult<SparePartBrandResponse>> GetPagedAsync(ParamQuery query, bool onlyActive = false, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(query, onlyActive, ct);
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
            if (!_currentUser.IsInRole("Supervisor"))
                throw new UnauthorizedAccessException("Chỉ Supervisor được tạo hãng phụ tùng");

            if (string.IsNullOrWhiteSpace(request.BrandName))
                throw new InvalidOperationException("Tên hãng phụ tùng không được để trống");

            var name = request.BrandName.Trim();
            if (name.Length > 100)
                throw new InvalidOperationException("Tên hãng phụ tùng không được vượt quá 100 ký tự");

            var description = request.Description?.Trim();
            if (description != null && description.Length > 500)
                throw new InvalidOperationException("Mô tả không được vượt quá 500 ký tự");

            if (await _repo.HasExistAsync(name, null, ct))
                throw new InvalidOperationException("Hãng phụ tùng đã tồn tại");

            var entity = new SparePartBrand
            {
                BrandName = name,
                Description = description,
                CreatedBy = _currentUser.GetCurrentUserId()
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<SparePartBrandResponse?> UpdateAsync(int id, SparePartBrandUpdateRequest request, CancellationToken ct = default)
        {
            if (!_currentUser.IsInRole("Supervisor"))
                throw new UnauthorizedAccessException("Chỉ Supervisor được cập nhật hãng phụ tùng");

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.DeletedAt != null) return null;

            var hasChildren = await _repo.HasSparePartsAsync(id, ct);
            var inputName = (request.BrandName ?? string.Empty).Trim();
            var currentName = (entity.BrandName ?? string.Empty).Trim();
            var isNameChanged = !string.Equals(inputName, currentName, StringComparison.OrdinalIgnoreCase);

            // Nếu có dữ liệu con: bỏ qua BrandName từ request, chỉ cập nhật Description.
            if (!hasChildren)
            {
                if (string.IsNullOrWhiteSpace(inputName))
                    throw new InvalidOperationException("Tên hãng phụ tùng không được để trống");
                if (inputName.Length > 100)
                    throw new InvalidOperationException("Tên hãng phụ tùng không được vượt quá 100 ký tự");
            }

            var description = request.Description?.Trim();
            if (description != null && description.Length > 500)
                throw new InvalidOperationException("Mô tả không được vượt quá 500 ký tự");

            if (!hasChildren && isNameChanged && await _repo.HasExistAsync(inputName, entity.SparePartBrandId, ct))
                throw new InvalidOperationException("Hãng phụ tùng đã tồn tại, không thể đổi tên");

            if (!hasChildren)
                entity.BrandName = inputName;

            entity.Description = description;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _currentUser.GetCurrentUserId();

            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            if (!_currentUser.IsInRole("Supervisor"))
                throw new UnauthorizedAccessException("Chỉ Supervisor được xóa hãng phụ tùng");

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.DeletedAt != null) return false;

            if (await _repo.HasSparePartsAsync(id, ct))
                throw new InvalidOperationException("Không thể xóa hãng phụ tùng vì đang có phụ tùng liên kết");

            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = _currentUser.GetCurrentUserId();
            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return true;
        }

        private static SparePartBrandResponse Map(SparePartBrand entity)
        {
            return new SparePartBrandResponse
            {
                SparePartBrandId = entity.SparePartBrandId,
                BrandName = entity.BrandName,
                Description = entity.Description,
                IsActive = entity.IsActive,
            };
        }
    }
}
