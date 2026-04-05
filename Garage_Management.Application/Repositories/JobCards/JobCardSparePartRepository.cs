using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.JobCards
{
    public class JobCardSparePartRepository : IJobCardSparePartRepository
    {
        private readonly AppDbContext _context;

        public JobCardSparePartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(JobCardSparePart entity, CancellationToken cancellationToken)
        {
            await _context.JobCardSpareParts.AddAsync(entity, cancellationToken);
        }

        public IQueryable<JobCardSparePart> Query()
            => _context.JobCardSpareParts.AsQueryable();

        public async Task SaveAsync(CancellationToken cancellationToken)
            => await _context.SaveChangesAsync(cancellationToken);

        public void Delete(JobCardSparePart entity)
            => _context.JobCardSpareParts.Remove(entity);

        public async Task<JobCardSparePart?> GetByIdAsync(int jobCardId, int sparePartId, CancellationToken cancellationToken = default)
            => await _context.JobCardSpareParts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.JobCardId == jobCardId && x.SparePartId == sparePartId, cancellationToken);

        public async Task<List<JobCardSparePart>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
            => await _context.JobCardSpareParts
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

        public async Task<List<JobCardSparePart>> GetByJobCardIdAsync(int jobCardId, CancellationToken cancellationToken = default)
            => await _context.JobCardSpareParts
                .AsNoTracking()
                .Where(x => x.JobCardId == jobCardId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
    }
}
