using Garage_Management.Application.DTOs.Iventories;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services.Auth;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Application.Services.Auth;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Services.Inventories
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repo;
        private readonly ISparePartCategoryRepository _categoryRepo;
        private readonly ISparePartBrandRepository _brandRepo;
        private readonly ICurrentUserService _currentUser;

        public InventoryService(
            IInventoryRepository repo,
            ISparePartCategoryRepository categoryRepo,
            ISparePartBrandRepository brandRepo,
            ICurrentUserService currentUser)
        {
            _repo = repo;
            _categoryRepo = categoryRepo;
            _brandRepo = brandRepo;
            _currentUser = currentUser;
        }

        public async Task<ApiResponse<PagedResult<InventoryResponse>>> GetPagedAsync(ParamQuery query, CancellationToken ct = default)
        {
            try
            {
                var page = query.Page <= 0 ? 1 : query.Page;
                var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

                var q = _repo.Query()
                    .AsNoTracking()
                    .Include(x => x.SparePartBrand)
                    .Include(x => x.SparePartCategory)
                    .AsQueryable();

                if (!_currentUser.IsAdmin())
                {
                    var branchId = _currentUser.GetCurrentBranchId();
                    if (branchId.HasValue)
                    {
                        q = q.Where(x => x.BranchId == branchId.Value);
                    }
                }

                if (!string.IsNullOrWhiteSpace(query.Search))
                {
                    var search = query.Search.Trim().ToLower();
                    q = q.Where(x =>
                        (x.PartName ?? string.Empty).ToLower().Contains(search) ||
                        (x.PartCode ?? string.Empty).ToLower().Contains(search) ||
                        (x.SparePartBrand != null && (x.SparePartBrand.BrandName ?? string.Empty).ToLower().Contains(search)) ||
                        (x.SparePartCategory != null && (x.SparePartCategory.CategoryName ?? string.Empty).ToLower().Contains(search))
                    );
                }

                if (!string.IsNullOrWhiteSpace(query.Filter))
                {
                    var filter = query.Filter.Trim().ToLower();
                    if (filter == "active")
                        q = q.Where(x => x.IsActive);
                    else if (filter == "inactive")
                        q = q.Where(x => !x.IsActive);
                    else if (int.TryParse(filter, out var brandId))
                        q = q.Where(x => x.SparePartBrandId == brandId);
                }

                var orderBy = (query.OrderBy ?? string.Empty).Trim().ToLower();
                var desc = string.Equals(query.SortOrder, "DESC", StringComparison.OrdinalIgnoreCase);

                q = orderBy switch
                {
                    "partname" => desc ? q.OrderByDescending(x => x.PartName) : q.OrderBy(x => x.PartName),
                    "partcode" => desc ? q.OrderByDescending(x => x.PartCode) : q.OrderBy(x => x.PartCode),
                    "brandname" => desc ? q.OrderByDescending(x => x.SparePartBrand != null ? x.SparePartBrand.BrandName : string.Empty) : q.OrderBy(x => x.SparePartBrand != null ? x.SparePartBrand.BrandName : string.Empty),
                    "categoryname" => desc ? q.OrderByDescending(x => x.SparePartCategory != null ? x.SparePartCategory.CategoryName : string.Empty) : q.OrderBy(x => x.SparePartCategory != null ? x.SparePartCategory.CategoryName : string.Empty),
                    "quantity" => desc ? q.OrderByDescending(x => x.Quantity) : q.OrderBy(x => x.Quantity),
                    "sellingprice" => desc ? q.OrderByDescending(x => x.SellingPrice ?? 0) : q.OrderBy(x => x.SellingPrice ?? 0),
                    "lastpurchaseprice" => desc ? q.OrderByDescending(x => x.LastPurchasePrice ?? 0) : q.OrderBy(x => x.LastPurchasePrice ?? 0),
                    _ => q.OrderByDescending(x => x.CreatedAt)
                };

                var total = await q.CountAsync(ct);

                var items = await q
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(MapExpression())
                    .ToListAsync(ct);

                var paged = new PagedResult<InventoryResponse>
                {
                    Page = page,
                    PageSize = pageSize,
                    Total = total,
                    PageData = items
                };

                return ApiResponse<PagedResult<InventoryResponse>>.SuccessResponse(paged, "OK");
            }
            catch
            {
                return ApiResponse<PagedResult<InventoryResponse>>.ErrorResponse("Có lỗi xảy ra khi lấy danh sách phụ tùng");
            }
        }

        public async Task<InventoryResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdWithDetailsAsync(id, ct);
            if (entity == null) return null;
            EnsureCanAccess(entity.BranchId);
            return Map(entity);
        }

        public async Task<InventoryResponse> CreateAsync(InventoryCreateRequest request, CancellationToken ct = default)
        {
            // Validation tên phụ tùng
            if (string.IsNullOrWhiteSpace(request.PartName))
                throw new InvalidOperationException("PartName không hợp lệ");

            // Validation số lượng
            if (request.Quantity < 0)
                throw new InvalidOperationException("Quantity không hợp lệ");

            // Validation ngưỡng tồn tối thiểu
            if (request.MinQuantity.HasValue && request.MinQuantity.Value < 0)
                throw new InvalidOperationException("MinQuantity không hợp lệ");

            // Validation giá nhập gần nhất
            if (request.LastPurchasePrice.HasValue && request.LastPurchasePrice.Value < 0)
                throw new InvalidOperationException("LastPurchasePrice không hợp lệ");

            // Validation giá bán
            if (request.SellingPrice.HasValue && request.SellingPrice.Value <= 0)
                throw new InvalidOperationException("SellingPrice không hợp lệ");

            // Validation CategoryId tồn tại (nếu có)
            if (request.CategoryId.HasValue)
            {
                var category = await _categoryRepo.GetByIdAsync(request.CategoryId.Value);
                if (category == null)
                    throw new InvalidOperationException("CategoryId không tồn tại");
            }

            // Validation SparePartBrandId tồn tại (nếu có)
            if (request.SparePartBrandId.HasValue)
            {
                var brand = await _brandRepo.GetByIdAsync(request.SparePartBrandId.Value);
                if (brand == null)
                    throw new InvalidOperationException("SparePartBrandId không tồn tại");
            }

            var branchId = ResolveBranchIdForCreate(request.BranchId);

            // Validation PartCode duy nhất trong phạm vi chi nhánh (nếu có)
            var partCode = string.IsNullOrWhiteSpace(request.PartCode) ? null : request.PartCode.Trim();
            if (partCode != null)
            {
                var duplicate = await _repo.Query()
                    .AsNoTracking()
                    .AnyAsync(x => x.PartCode == partCode && x.BranchId == branchId, ct);
                if (duplicate)
                    throw new InvalidOperationException("PartCode đã tồn tại");
            }

            var entity = new Inventory
            {
                BranchId = branchId,
                PartCode = partCode,
                PartName = request.PartName.Trim(),
                Unit = string.IsNullOrWhiteSpace(request.Unit) ? null : request.Unit.Trim(),
                CategoryId = request.CategoryId,
                SparePartBrandId = request.SparePartBrandId,
                Quantity = request.Quantity,
                MinQuantity = request.MinQuantity,
                LastPurchasePrice = request.LastPurchasePrice,
                SellingPrice = request.SellingPrice,
                IsActive = HasActiveSellingPrice(request.SellingPrice),
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);

            var detail = await _repo.GetByIdWithDetailsAsync(entity.SparePartId, ct);
            return detail == null ? Map(entity) : Map(detail);
        }

        private int ResolveBranchIdForCreate(int? requestedBranchId)
        {
            if (_currentUser.IsAdmin())
            {
                if (requestedBranchId is not { } adminBranch || adminBranch <= 0)
                    throw new InvalidOperationException("Admin phải chỉ định BranchId khi tạo phụ tùng");
                return adminBranch;
            }
            var scoped = _currentUser.GetCurrentBranchId();
            if (!scoped.HasValue)
                throw new UnauthorizedAccessException("Không xác định được chi nhánh từ tài khoản hiện tại");
            return scoped.Value;
        }

        private void EnsureCanAccess(int branchId)
        {
            if (_currentUser.IsAdmin()) return;
            var scoped = _currentUser.GetCurrentBranchId();
            if (scoped.HasValue && scoped.Value == branchId) return;
            throw new UnauthorizedAccessException("Không có quyền truy cập phụ tùng của chi nhánh khác");
        }

        public async Task<InventoryResponse?> UpdateAsync(int id, InventoryUpdateRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new InvalidOperationException("Id không tồn tại");
            EnsureCanAccess(entity.BranchId);

            if (request.PartName != null)
            {
                if (string.IsNullOrWhiteSpace(request.PartName))
                    throw new InvalidOperationException("PartName không hợp lệ");
                entity.PartName = request.PartName.Trim();
            }

            if (request.Unit != null)
                entity.Unit = string.IsNullOrWhiteSpace(request.Unit) ? null : request.Unit.Trim();

            if (request.CategoryId.HasValue)
            {
                var category = await _categoryRepo.GetByIdAsync(request.CategoryId.Value);
                if (category == null)
                    throw new InvalidOperationException("CategoryId không tồn tại");
                entity.CategoryId = request.CategoryId;
            }

            if (request.SparePartBrandId.HasValue)
            {
                var brand = await _brandRepo.GetByIdAsync(request.SparePartBrandId.Value);
                if (brand == null)
                    throw new InvalidOperationException("SparePartBrandId không tồn tại");
                entity.SparePartBrandId = request.SparePartBrandId;
            }

            if (request.PartCode != null)
            {
                var newCode = request.PartCode.Trim();
                if (newCode.Length == 0)
                {
                    entity.PartCode = null;
                }
                else if (newCode != entity.PartCode)
                {
                    var duplicate = await _repo.Query()
                        .AsNoTracking()
                        .AnyAsync(x => x.PartCode == newCode && x.BranchId == entity.BranchId && x.SparePartId != entity.SparePartId, ct);
                    if (duplicate)
                        throw new InvalidOperationException("PartCode đã tồn tại");
                    entity.PartCode = newCode;
                }
            }

            if (request.MinQuantity.HasValue)
            {
                if (request.MinQuantity.Value < 0) throw new InvalidOperationException("Số lượng phụ tùng tối thiểu không hợp lệ");
                entity.MinQuantity = request.MinQuantity.Value;
            }
            if (request.LastPurchasePrice.HasValue)
            {
                if (request.LastPurchasePrice.Value < 0) throw new InvalidOperationException("Giá mua cuối cùng không hợp lệ");
                entity.LastPurchasePrice = request.LastPurchasePrice.Value;
            }
            if (request.SellingPrice.HasValue)
            {
                if (request.SellingPrice.Value <= 0) throw new InvalidOperationException("Giá bán hiện tại phải lớn hơn 0");
                entity.SellingPrice = request.SellingPrice.Value;
                entity.IsActive = true;
            }

            entity.UpdatedAt = DateTime.UtcNow;
            _repo.Update(entity);
            await _repo.SaveAsync(ct);

            var detail = await _repo.GetByIdWithDetailsAsync(entity.SparePartId, ct);
            return detail == null ? Map(entity) : Map(detail);
        }

        public async Task<InventoryResponse?> UpdateStatusAsync(int id, bool isActive, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new InvalidOperationException("Id không tồn tại");
            EnsureCanAccess(entity.BranchId);

            if (isActive && !HasActiveSellingPrice(entity.SellingPrice))
                throw new InvalidOperationException("Không thể kích hoạt phụ tùng chưa có giá bán (SellingPrice)");

            if (entity.IsActive == isActive)
                return await GetByIdAsync(id, ct);

            entity.IsActive = isActive;
            entity.UpdatedAt = DateTime.UtcNow;

            _repo.Update(entity);
            await _repo.SaveAsync(ct);

            return await GetByIdAsync(id, ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            EnsureCanAccess(entity.BranchId);

            if (await _repo.HasDependenciesAsync(id, ct))
                throw new InvalidOperationException("Không thể xóa phụ tùng vì đã phát sinh dữ liệu liên quan");

            _repo.Delete(entity);
            await _repo.SaveAsync(ct);
            return true;
        }

        public async Task<List<InventoryResponse>> GetByBrandIdAsync(int brandId, CancellationToken ct = default)
        {
            var data = await _repo.GetByBrandIdAsync(brandId, ct);
            return data.Select(Map).ToList();
        }

        private static System.Linq.Expressions.Expression<Func<Inventory, InventoryResponse>> MapExpression()
        {
            return x => new InventoryResponse
            {
                SparePartId = x.SparePartId,
                PartCode = x.PartCode,
                PartName = x.PartName,
                Unit = x.Unit,
                CategoryId = x.CategoryId,
                CategoryName = x.SparePartCategory != null ? x.SparePartCategory.CategoryName : null,
                SparePartBrandId = x.SparePartBrandId,
                SparePartBrandName = x.SparePartBrand != null ? x.SparePartBrand.BrandName : null,
                Quantity = x.Quantity,
                MinQuantity = x.MinQuantity,
                LastPurchasePrice = x.LastPurchasePrice,
                SellingPrice = x.SellingPrice,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            };
        }

        private static InventoryResponse Map(Inventory entity)
        {
            return new InventoryResponse
            {
                SparePartId = entity.SparePartId,
                PartCode = entity.PartCode,
                PartName = entity.PartName,
                Unit = entity.Unit,
                CategoryId = entity.CategoryId,
                CategoryName = entity.SparePartCategory?.CategoryName,
                SparePartBrandId = entity.SparePartBrandId,
                SparePartBrandName = entity.SparePartBrand?.BrandName,
                Quantity = entity.Quantity,
                MinQuantity = entity.MinQuantity,
                LastPurchasePrice = entity.LastPurchasePrice,
                SellingPrice = entity.SellingPrice,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        private static bool HasActiveSellingPrice(decimal? sellingPrice)
        {
            return sellingPrice.HasValue && sellingPrice.Value > 0;
        }
    }
}
