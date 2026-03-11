using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.JobCards
{
    public class JobCardServiceRepository : BaseRepository<JobCardService>, IJobCardServiceRepository
    {
        public JobCardServiceRepository(AppDbContext context) : base(context) { }

        public async Task<PagedResult<JobCardService>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = GetAll().AsNoTracking();
            var total = await query.CountAsync(ct);
            var data = await query
                .OrderByDescending(x => x.JobCardServiceId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<JobCardService>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
        }
    }
}
