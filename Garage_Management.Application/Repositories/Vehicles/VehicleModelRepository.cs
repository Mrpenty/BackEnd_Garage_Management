using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Vehiclies;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Repositories.Vehicles
{
    public class VehicleModelRepository : BaseRepository<VehicleModel>, IVehicleModelRepository
    {
        private readonly AppDbContext _context;
        public VehicleModelRepository(AppDbContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<PagedResult<VehicleModel>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 1;

            var query = GetAll().AsNoTracking();
            var total = await query.CountAsync(ct);
            var data = await query
                .OrderByDescending(x => x.ModelId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct)
                ;

            return new PagedResult<VehicleModel>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
        }

        public Task<bool> ExistsAsync(int brandId, string modelName, int? excludeId = null, CancellationToken ct = default)
        {
            var name = modelName.Trim().ToLower();
            var query = GetAll().AsNoTracking().Where(x => x.BrandId == brandId && x.ModelName.ToLower() == name);
            if (excludeId.HasValue)
                query = query.Where(x => x.ModelId != excludeId.Value);

            return query.AnyAsync(ct);
        }
        public Task<bool> HasVehiclesAsync(int modelId, CancellationToken ct = default)
        {
            return _context.Set<Vehicle>()
                .AsNoTracking()
                .AnyAsync(x => x.ModelId == modelId, ct);
        }
    }
}
