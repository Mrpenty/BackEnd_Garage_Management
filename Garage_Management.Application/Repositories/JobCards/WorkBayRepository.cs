using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Garage_Management.Application.Interfaces.Repositories;
    using Garage_Management.Base.Data;
    using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
namespace Garage_Management.Application.Repositories.JobCards
{
    
        public class WorkBayRepository : BaseRepository<WorkBay>, IWorkBayRepository
        {
            private readonly AppDbContext _context;

            public WorkBayRepository(AppDbContext context) : base(context)
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
        }
    }


