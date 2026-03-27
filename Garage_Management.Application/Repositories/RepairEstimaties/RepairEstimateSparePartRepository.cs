using Garage_Management.Application.Interfaces.Repositories.RepairEstimaties;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.RepairEstimaties;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.RepairEstimaties
{
    public class RepairEstimateSparePartRepository : IRepairEstimateSparePartRepository
    {
        private readonly AppDbContext _context;

        public RepairEstimateSparePartRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<bool> RepairEstimateExistsAsync(int repairEstimateId, CancellationToken ct = default)
        {
            return _context.RepairEstimates
                .AsNoTracking()
                .AnyAsync(x => x.RepairEstimateId == repairEstimateId, ct);
        }

        public Task<RepairEstimateSparePart?> GetByIdAsync(int repairEstimateId, int sparePartId, CancellationToken ct = default)
        {
            return _context.Set<RepairEstimateSparePart>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.RepairEstimateId == repairEstimateId && x.SparePartId == sparePartId, ct);
        }

        public async Task AddAsync(RepairEstimateSparePart entity, CancellationToken ct = default)
        {
            await _context.Set<RepairEstimateSparePart>().AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }
    }
}
