using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Services;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.Services
{
    public class ServiceTaskRepository : BaseRepository<ServiceTask>, IServiceTaskRepository
    {
        private readonly DbContext _context;
        public ServiceTaskRepository(AppDbContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<PagedResult<ServiceTask>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = GetAll().AsNoTracking();
            var total = await query.CountAsync(ct);
            var data = await query
                .OrderByDescending(x => x.ServiceTaskId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<ServiceTask>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
        }
        public Task<bool> HasExistAsync(int serviceId, int taskOrder, int? excludeId = null, CancellationToken ct = default)
        {
            var query = _context.Set<ServiceTask>()
                .AsNoTracking()
                .Where(x => x.ServiceId == serviceId && x.TaskOrder == taskOrder);

            if (excludeId.HasValue)
                query = query.Where(x => x.ServiceTaskId != excludeId.Value);

            return query.AnyAsync(ct);
        }

        public async Task<List<ServiceTask>> GetByServiceIdAsync(int serviceId, CancellationToken ct = default)
        {
            return await _context.Set<ServiceTask>()
                .AsNoTracking()
                .Where(x => x.ServiceId == serviceId)
                .OrderBy(x => x.TaskOrder)
                .ThenBy(x => x.ServiceTaskId)
                .ToListAsync(ct);
        }


    }
}
