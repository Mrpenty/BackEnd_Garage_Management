using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Data;
    using Garage_Management.Base.Entities.JobCards;
    
    using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Garage_Management.Application.Repositories.JobCards
{
    
        public class WorkBayRepository : IWorkBayRepository
        {
            private readonly AppDbContext _context;

            public WorkBayRepository(AppDbContext context)
            {
                _context = context;
            }

            public async Task<WorkBay?> GetByIdAsync(int id)
                => await _context.WorkBay
                    .FirstOrDefaultAsync(x => x.Id == id);

            public IQueryable<WorkBay> Query()
                => _context.WorkBay.AsQueryable();

            public async Task AddAsync(WorkBay entity, CancellationToken cancellationToken)
                => await _context.WorkBay.AddAsync(entity, cancellationToken);

            public async Task SaveAsync(CancellationToken cancellationToken)
                => await _context.SaveChangesAsync(cancellationToken);
        public async Task<List<WorkBay>> GetByStatusAsync(
       WorkBayStatus? status,
       CancellationToken cancellationToken)
        {
            return await _context.WorkBay
                .Where(x => !status.HasValue || x.Status == status.Value)
                .ToListAsync(cancellationToken);
        }

        
    }
    }


