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
        

        public async Task<PagedResult<Service>> GetPagedAsync(
            int page,
            int pageSize,
            string? keyword = null,
            bool? isActive = null,
            bool? hasPrice = null,
            int? vehicleTypeId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            string? sortBy = null,
            bool sortDesc = true,
            CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Services
                .Include(x => x.ServiceTasks)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var k = keyword.Trim().ToLower();
                query = query.Where(x =>
                    x.ServiceName.ToLower().Contains(k)
                    || (x.Description != null && x.Description.ToLower().Contains(k)));
            }

            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive.Value);

            if (hasPrice.HasValue)
            {
                query = hasPrice.Value
                    ? query.Where(x => x.BasePrice != null && x.BasePrice > 0)
                    : query.Where(x => x.BasePrice == null || x.BasePrice <= 0);
            }

            if (vehicleTypeId.HasValue && vehicleTypeId.Value > 0)
                query = query.Where(x => x.ServiceVehicleTypes.Any(svt => svt.VehicleTypeId == vehicleTypeId.Value));

            if (minPrice.HasValue)
                query = query.Where(x => x.BasePrice != null && x.BasePrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(x => x.BasePrice != null && x.BasePrice <= maxPrice.Value);

            query = (sortBy?.Trim().ToLower()) switch
            {
                "name" => sortDesc ? query.OrderByDescending(x => x.ServiceName) : query.OrderBy(x => x.ServiceName),
                "price" => sortDesc ? query.OrderByDescending(x => x.BasePrice) : query.OrderBy(x => x.BasePrice),
                "createdat" => sortDesc ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
                _ => sortDesc ? query.OrderByDescending(x => x.ServiceId) : query.OrderBy(x => x.ServiceId),
            };

            var total = await query.CountAsync(ct);
            var data = await query
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

        public async Task<PagedResult<ServiceVehicleType>> GetServiceVehicleTypePairsPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.ServiceVehicleTypes
                .Include(x => x.Service)
                .Include(x => x.VehicleType)
                .Where(x => x.Service.IsActive && x.VehicleType.IsActive)
                .AsNoTracking();

            var total = await query.CountAsync(ct);
            var data = await query
                .OrderByDescending(x => x.VehicleTypeId)
                .ThenBy(x => x.ServiceId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<ServiceVehicleType>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
        }

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
        public async Task<Service?> GetByIdWithTasksAsync(int id, CancellationToken ct)
        {
            return await _context.Services
                .Include(s => s.ServiceTasks) 
                .FirstOrDefaultAsync(s => s.ServiceId == id, ct);
        }
    }
}
