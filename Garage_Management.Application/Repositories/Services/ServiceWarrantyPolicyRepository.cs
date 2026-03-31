using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Services;
using Garage_Management.Base.Entities.Warranties;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.Services
{
    public class ServiceWarrantyPolicyRepository : BaseRepository<ServiceWarrantyPolicy>, IServiceWarrantyPolicyRepository
    {
        private readonly AppDbContext _context;

        public ServiceWarrantyPolicyRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedResult<ServiceWarrantyPolicy>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = GetAll().AsNoTracking();
            var total = await query.CountAsync(ct);
            var data = await query
                .OrderByDescending(x => x.PolicyId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<ServiceWarrantyPolicy>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
        }

        public Task<bool> ExistsByNameAsync(string policyName, int? excludeId = null, CancellationToken ct = default)
        {
            var name = policyName.Trim().ToLower();
            var query = GetAll().AsNoTracking().Where(x => x.PolicyName.ToLower() == name);
            if (excludeId.HasValue)
                query = query.Where(x => x.PolicyId != excludeId.Value);

            return query.AnyAsync(ct);
        }

        public async Task<bool> HasDependenciesAsync(int policyId, CancellationToken ct = default)
        {
            return await _context.Set<WarrantyService>()
                .AsNoTracking()
                .AnyAsync(x => x.ServiceWarrantyPolicyId == policyId, ct);
        }
    }
}
