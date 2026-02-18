using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Services;
using Microsoft.EntityFrameworkCore;
using Garage_Management.Base.Entities;

namespace Garage_Management.Application.Repositories.Services
   

{


    namespace Garage_Management.Application.Repositories.Services
    {
        public class ServiceRepository : IServiceRepository
        {
            private readonly AppDbContext _context;

            public ServiceRepository(AppDbContext context)
            {
                _context = context;
            }

            public IQueryable<Service> Query()
                => _context.Services.AsQueryable();

            public async Task<Service?> GetByIdAsync(int id)
                => await _context.Services
                    .FirstOrDefaultAsync(x => x.ServiceId == id);

            public async Task SaveAsync(CancellationToken cancellationToken)
                => await _context.SaveChangesAsync(cancellationToken);
        }
    }

}
