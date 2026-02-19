using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.JobCards;
namespace Garage_Management.Application.Repositories.JobCards
{
   

        public class JobCardServiceRepository : IJobCardServiceRepository
        {
            private readonly AppDbContext _context;

            public JobCardServiceRepository(AppDbContext context)
            {
                _context = context;
            }

            public async Task AddAsync(JobCardService entity, CancellationToken cancellationToken)
            {
                await _context.JobCardServices.AddAsync(entity, cancellationToken);
            }

            public IQueryable<JobCardService> Query()
                => _context.JobCardServices.AsQueryable();

            public async Task SaveAsync(CancellationToken cancellationToken)
                => await _context.SaveChangesAsync(cancellationToken);
        }
    }


