using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.JobCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
