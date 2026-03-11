using Garage_Management.Application.Interfaces.Repositories.Warranties;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Warranties;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.Warranties
{
    public class WarrantyServiceRepository : BaseRepository<WarrantyService>, IWarrantyServiceRepository
    {
        public WarrantyServiceRepository(AppDbContext context) : base(context) { }

        public async Task<PagedResult<WarrantyService>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = GetAll().AsNoTracking();
            var total = await query.CountAsync(ct);
            var data = await query
                .OrderByDescending(x => x.WarrantyServiceId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<WarrantyService>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
        }
    }
}
