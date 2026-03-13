using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Services;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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

            var query = _context.Services
                .Include(x => x.ServiceTasks)
                .AsNoTracking();
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
                .Include(x => x.ServiceTasks)
                .FirstOrDefaultAsync(x => x.ServiceId == id);

        public async Task<List<Service>> GetByVehicleTypeAsync(int vehicleTypeId, CancellationToken ct = default)
            => await _context.Services
                .Include(x => x.ServiceTasks)
                .Include(x => x.ServiceVehicleTypes)
                .Where(x => x.ServiceVehicleTypes.Any(svt => svt.VehicleTypeId == vehicleTypeId))
                .AsNoTracking()
                .OrderBy(x => x.ServiceName)
                .ToListAsync(ct);

        public async Task<bool> HasDependenciesAsync(int serviceId, CancellationToken ct = default)
        {
            return await _context.AppointmentServices.AsNoTracking().AnyAsync(x => x.ServiceId == serviceId, ct)
                || await _context.JobCardServices.AsNoTracking().AnyAsync(x => x.ServiceId == serviceId, ct)
                || await _context.RepairEstimateServices.AsNoTracking().AnyAsync(x => x.ServiceId == serviceId, ct)
                || await _context.WarrantyServices.AsNoTracking().AnyAsync(x => x.ServiceId == serviceId, ct);
        }

        public Task<bool> ExistsByNameAsync(string serviceName, CancellationToken ct = default)
        {
            var name = serviceName.Trim().ToLower();
            return _context.Services.AsNoTracking().AnyAsync(x => x.ServiceName.ToLower() == name, ct);
        }

        public async Task SaveAsync(CancellationToken cancellationToken)
            => await _context.SaveChangesAsync(cancellationToken);
    }
}
