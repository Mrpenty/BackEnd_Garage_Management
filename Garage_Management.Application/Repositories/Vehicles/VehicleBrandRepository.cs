using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Vehiclies;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.Vehicles
{
    public class VehicleBrandRepository : BaseRepository<VehicleBrand>, IVehicleBrandRepository
    {
        public VehicleBrandRepository(AppDbContext context) : base(context) { }
        public async Task<PagedResult<VehicleBrand>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if(page < 1) page = 1;
            if(pageSize < 1) pageSize = 1;

            var query = GetAll().AsNoTracking();
            var total = await query.CountAsync(ct);
            var data = await query
                .OrderByDescending(x => x.BrandId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct)
                ;

            return new PagedResult<VehicleBrand>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
        }
    }
}
