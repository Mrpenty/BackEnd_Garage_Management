using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

namespace Garage_Management.Application.Repositories.Inventories
{
    public class SparePartBrandRepository : BaseRepository<SparePartBrand>, ISparePartBrandRepository
    {
        private readonly AppDbContext _context;

        public SparePartBrandRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedResult<SparePartBrand>> GetPagedAsync(ParamQuery query, bool onlyActive = false, CancellationToken ct = default)
        {
            var page = query.Page <= 0 ? 1 : query.Page;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var q = GetAll().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                q = q.Where(x =>
                    (x.BrandName ?? string.Empty).ToLower().Contains(search) ||
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
                "brandname" => desc ? q.OrderByDescending(x => x.BrandName) : q.OrderBy(x => x.BrandName),
                "isactive" => desc ? q.OrderByDescending(x => x.IsActive) : q.OrderBy(x => x.IsActive),
                _ => desc ? q.OrderByDescending(x => x.SparePartBrandId) : q.OrderBy(x => x.SparePartBrandId)
            };

            var total = await q.CountAsync(ct);
            var data = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<SparePartBrand>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
        }

        public Task<bool> HasExistAsync(string brandName, int? excludeId = null, CancellationToken ct = default)
        {
            var name = brandName.Trim().ToLower();
            var query = _context.Set<SparePartBrand>()
                .AsNoTracking()
                .Where(x => x.BrandName.ToLower() == name);

            if (excludeId.HasValue)
                query = query.Where(x => x.SparePartBrandId != excludeId.Value);

            return query.AnyAsync(ct);
        }

        public Task<bool> HasSparePartsAsync(int brandId, CancellationToken ct = default)
        {
            return _context.Set<Inventory>()
                .AsNoTracking()
                .AnyAsync(x => x.SparePartBrandId == brandId, ct);
        }
    }
}
