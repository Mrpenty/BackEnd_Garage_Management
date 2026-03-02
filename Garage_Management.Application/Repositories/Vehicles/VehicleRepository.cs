using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Vehiclies;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.Vehicles
{
    public class VehicleRepository : BaseRepository<Vehicle>, IVehicleRepository
    {
        private readonly DbContext _context;
        public VehicleRepository(AppDbContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<PagedResult<Vehicle>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = GetAll().AsNoTracking();

            var total = await query.CountAsync(ct);
            var data = await query
                .OrderByDescending(x => x.VehicleId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct)
                ;

            return new PagedResult<Vehicle>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
        }

        public Task<bool> HasAppointmentsAsync(int vehicleId, CancellationToken ct = default)
        {
            return _context.Set<Vehicle>().AsNoTracking().AnyAsync(x=>x.VehicleId == vehicleId);
        }
    }
}
