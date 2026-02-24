using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Services;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.Services
{
    public class ServiceRepository : BaseRepository<Service>, IServiceRepository
    {
        private readonly AppDbContext _context;
        public ServiceRepository(AppDbContext context) : base(context) 
        { 
            _context = context;
        }
        

        public async Task<PagedResult<Service>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = GetAll().AsNoTracking();
            var total = await query.CountAsync(ct);
            var data = await query
                .OrderByDescending(x => x.ServiceId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<Service>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
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
