using Garage_Management.Application.Interfaces.Repositories.RepairEstimaties;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.RepairEstimaties;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.RepairEstimaties
{
    public class RepairEstimateRepository : IRepairEstimateRepository
    {
        private readonly AppDbContext _context;

        public RepairEstimateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RepairEstimate>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.RepairEstimates
                .AsNoTracking()
                .OrderByDescending(x => x.RepairEstimateId)
                .ToListAsync(ct);
        }

        public async Task<RepairEstimate?> GetByIdAsync(int repairEstimateId, CancellationToken ct = default)
        {
            return await _context.RepairEstimates
                .AsNoTracking()
                .Include(x => x.Services)
                    .ThenInclude(x => x.Service)
                .Include(x => x.SpareParts)
                    .ThenInclude(x => x.Inventory)
                .FirstOrDefaultAsync(x => x.RepairEstimateId == repairEstimateId, ct);
        }

        public async Task<RepairEstimate?> GetTrackedByIdAsync(int repairEstimateId, CancellationToken ct = default)
        {
            return await _context.RepairEstimates
                .Include(x => x.Services)
                    .ThenInclude(x => x.Service)
                .Include(x => x.SpareParts)
                    .ThenInclude(x => x.Inventory)
                .FirstOrDefaultAsync(x => x.RepairEstimateId == repairEstimateId, ct);
        }

        public async Task<List<RepairEstimate>> GetByJobCardIdAsync(int jobCardId, CancellationToken ct = default)
        {
            return await _context.RepairEstimates
                .AsNoTracking()
                .Include(x => x.Services)
                    .ThenInclude(x => x.Service)
                .Include(x => x.SpareParts)
                    .ThenInclude(x => x.Inventory)
                .Where(x => x.JobCardId == jobCardId)
                .OrderByDescending(x => x.RepairEstimateId)
                .ToListAsync(ct);
        }

        public async Task AddAsync(RepairEstimate entity, CancellationToken ct = default)
        {
            await _context.RepairEstimates.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(RepairEstimate entity, CancellationToken ct = default)
        {
            _context.RepairEstimates.Update(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}
