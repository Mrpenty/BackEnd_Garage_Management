using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.Inventories
{
    public class SparePartCategoryRepository : BaseRepository<SparePartCategory>, ISparePartCategoryRepository
    {
        private readonly AppDbContext _context;

        public SparePartCategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedResult<SparePartCategory>> GetPagedAsync(ParamQuery query, bool onlyActive = false, CancellationToken ct = default)
        {
            var page = query.Page <= 0 ? 1 : query.Page;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            var q = GetAll().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                q = q.Where(x =>
                    (x.CategoryName ?? string.Empty).ToLower().Contains(search) ||
                    (x.Description ?? string.Empty).ToLower().Contains(search));
            }

            if (onlyActive)
            {
                q = q.Where(x => x.IsActive);
            }
            else if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var filter = query.Filter.Trim().ToLower();
                if (bool.TryParse(filter, out var isActive))
                    q = q.Where(x => x.IsActive == isActive);
            }

            var orderBy = (query.OrderBy ?? string.Empty).Trim().ToLower();
            var desc = string.Equals(query.SortOrder, "DESC", StringComparison.OrdinalIgnoreCase);
            q = orderBy switch
            {
                "categoryname" => desc ? q.OrderByDescending(x => x.CategoryName) : q.OrderBy(x => x.CategoryName),
                "isactive" => desc ? q.OrderByDescending(x => x.IsActive) : q.OrderBy(x => x.IsActive),
                _ => desc ? q.OrderByDescending(x => x.CategoryId) : q.OrderBy(x => x.CategoryId)
            };

            var total = await q.CountAsync(ct);
            var data = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<SparePartCategory>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
        }

        public Task<bool> HasExistAsync(string categoryName, int? excludeId = null, CancellationToken ct = default)
        {
            var name = categoryName.Trim().ToLower();
            var q = _context.Set<SparePartCategory>()
                .AsNoTracking()
                .Where(x => x.CategoryName.ToLower() == name);

            if (excludeId.HasValue)
                q = q.Where(x => x.CategoryId != excludeId.Value);

            return q.AnyAsync(ct);
        }

        public Task<bool> HasSparePartsAsync(int categoryId, CancellationToken ct = default)
        {
            return _context.Set<Inventory>()
                .AsNoTracking()
                .AnyAsync(x => x.CategoryId == categoryId, ct);
        }
    }
}
