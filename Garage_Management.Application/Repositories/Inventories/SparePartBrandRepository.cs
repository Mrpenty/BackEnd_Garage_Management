using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.Inventories
{
    public class SparePartBrandRepository : BaseRepository<SparePartBrand>, ISparePartBrandRepository
    {
        private readonly AppDbContext _context;

        public SparePartBrandRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedResult<SparePartBrand>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = GetAll().AsNoTracking();
            var total = await query.CountAsync(ct);
            var data = await query
                .OrderByDescending(x => x.SparePartBrandId)
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
