using Garage_Management.Application.Interfaces.Repositories.RepairEstimaties;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.RepairEstimaties;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.RepairEstimaties
{
    public class RepairEstimateServiceRepository : IRepairEstimateServiceRepository
    {
        private readonly AppDbContext _context;

        public RepairEstimateServiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<RepairEstimateService>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Set<RepairEstimateService>().AsNoTracking();
            var total = await query.CountAsync(ct);
            var data = await query
                .OrderByDescending(x => x.RepairEstimateId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<RepairEstimateService>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
        }

        public Task<RepairEstimateService?> GetByIdAsync(int repairEstimateId, int serviceId, CancellationToken ct = default)
        {
            return _context.Set<RepairEstimateService>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.RepairEstimateId == repairEstimateId && x.ServiceId == serviceId, ct);
        }

        public Task<RepairEstimateService?> GetTrackedByIdAsync(int repairEstimateId, int serviceId, CancellationToken ct = default)
        {
            return _context.Set<RepairEstimateService>()
                .FirstOrDefaultAsync(x => x.RepairEstimateId == repairEstimateId && x.ServiceId == serviceId, ct);
        }

        public async Task AddAsync(RepairEstimateService entity, CancellationToken ct = default)
        {
            await _context.Set<RepairEstimateService>().AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(RepairEstimateService entity, CancellationToken ct = default)
        {
            _context.Set<RepairEstimateService>().Update(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(RepairEstimateService entity, CancellationToken ct = default)
        {
            _context.Set<RepairEstimateService>().Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}
