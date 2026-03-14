using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Vehiclies;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.Vehicles
{
    public class VehicleTypeRepository : BaseRepository<VehicleType>, IVehicleTypeRepository
    {
        private readonly AppDbContext _context;

        public VehicleTypeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedResult<VehicleType>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.VehicleTypes.AsNoTracking();
            var total = await query.CountAsync(ct);
            var data = await query
                .OrderByDescending(x => x.VehicleTypeId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<VehicleType>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
        }

        public Task<bool> ExistsByTypeNameAsync(string typeName, int? excludeId = null, CancellationToken ct = default)
        {
            var normalized = typeName.Trim().ToLower();
            var query = _context.VehicleTypes
                .AsNoTracking()
                .Where(x => x.TypeName.ToLower() == normalized);

            if (excludeId.HasValue)
                query = query.Where(x => x.VehicleTypeId != excludeId.Value);

            return query.AnyAsync(ct);
        }

        public Task<bool> HasModelsAsync(int vehicleTypeId, CancellationToken ct = default)
            => _context.VehicleModels
                .AsNoTracking()
                .AnyAsync(x => x.VehicleTypeId == vehicleTypeId, ct);

        public Task<bool> HasServiceMappingsAsync(int vehicleTypeId, CancellationToken ct = default)
            => _context.ServiceVehicleTypes
                .AsNoTracking()
                .AnyAsync(x => x.VehicleTypeId == vehicleTypeId, ct);
    }
}

