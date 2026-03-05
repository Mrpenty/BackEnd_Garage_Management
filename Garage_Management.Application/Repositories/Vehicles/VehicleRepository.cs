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

        public async Task<ApiResponse<PagedResult<Vehicle>>> GetPagedAsync(ParamQuery query, CancellationToken ct = default)
        {
            
            if (query.Page <= 0) query.Page = 1;
            if (query.PageSize <= 0) query.PageSize = 10;

            // Base query
            var q = _context.Set<Vehicle>()
                .Include(x => x.Customer)
                    .ThenInclude(c => c.User)  
                .Include(v => v.Brand)
                .Include(v => v.Model)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();

                q = q.Where(x =>
                    (x.LicensePlate ?? "").Contains(search) ||
                    (x.Customer != null && x.Customer.User != null &&
                     (x.Customer.User.PhoneNumber ?? "").ToLower().Contains(search))
                );

                var searchDigits = new string(search.Where(char.IsDigit).ToArray());
                if (!string.IsNullOrEmpty(searchDigits)) 
                {
                    q = q.Where(x =>
                        EF.Functions.Like(
                            (x.LicensePlate ?? "").Replace("-", "").Replace(".", "").Replace(" ", ""),
                            $"%{searchDigits}%"
                        ) ||
                        (x.Customer != null && x.Customer.User != null &&
                         EF.Functions.Like(
                             (x.Customer.User.PhoneNumber ?? "").Replace(" ", "").Replace("-", "").Replace(".", ""),
                             $"%{searchDigits}%"
                         ))
                    );
                }
            }

            var total = await q.CountAsync(ct);

            var data = await q
                .OrderByDescending(x => x.VehicleId)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(ct);

            var pagedResult = new PagedResult<Vehicle>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                Total = total,
                PageData = data
            };

            return ApiResponse<PagedResult<Vehicle>>.SuccessResponse(pagedResult, "Lấy danh sách xe thành công");
        }

        public async Task<PagedResult<Vehicle>> GetByCustomerIdAsync(int page, int pageSize, int customerId, CancellationToken ct = default)
        {
            if(page <= 0) page = 1;
            if(pageSize <= 0) pageSize = 10; 
            var query = GetAll().Where(x=>x.CustomerId==customerId).Include(v => v.Brand).Include(v => v.Model).AsNoTracking();
            var total = await query.CountAsync(ct);
            var data = await query
                .OrderByDescending(x => x.VehicleId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
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
