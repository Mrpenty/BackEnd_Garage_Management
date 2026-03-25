using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.Inventories
{
    public class SupplierRepository : BaseRepository<Supplier>, ISupplierRepository
    {
        private readonly AppDbContext _context;

        public SupplierRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedResult<Supplier>> GetPagedAsync(ParamQuery query, CancellationToken ct = default)
        {
            var page = query.Page <= 0 ? 1 : query.Page;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            var q = GetAll().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                q = q.Where(x =>
                    (x.SupplierName ?? string.Empty).ToLower().Contains(search) ||
                    (x.Phone ?? string.Empty).ToLower().Contains(search) ||
                    (x.Address ?? string.Empty).ToLower().Contains(search) ||
                    (x.TaxCode ?? string.Empty).ToLower().Contains(search));
            }
            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var filter = query.Filter.Trim().ToLower();
                if (bool.TryParse(filter, out var isActive))
                    q = q.Where(x => x.IsActive == isActive);
                else if (filter == "individual")
                    q = q.Where(x => x.SupplierType == Base.Common.Enums.SupplierType.Individual);
                else if (filter == "business")
                    q = q.Where(x => x.SupplierType == Base.Common.Enums.SupplierType.Business);
            }

            var orderBy = (query.OrderBy ?? string.Empty).Trim().ToLower();
            var desc = string.Equals(query.SortOrder, "DESC", StringComparison.OrdinalIgnoreCase);
            q = orderBy switch
            {
                "suppliername" => desc ? q.OrderByDescending(x => x.SupplierName) : q.OrderBy(x => x.SupplierName),
                "suppliertype" => desc ? q.OrderByDescending(x => x.SupplierType) : q.OrderBy(x => x.SupplierType),
                "isactive" => desc ? q.OrderByDescending(x => x.IsActive) : q.OrderBy(x => x.IsActive),
                _ => desc ? q.OrderByDescending(x => x.SupplierId) : q.OrderBy(x => x.SupplierId)
            };

            var total = await q.CountAsync(ct);
            var data = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

            return new PagedResult<Supplier>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
        }

        public Task<bool> HasExistAsync(string supplierName, int? excludeId = null, CancellationToken ct = default)
        {
            var name = supplierName.Trim().ToLower();
            var q = _context.Set<Supplier>().AsNoTracking().Where(x => x.SupplierName.ToLower() == name);

            if (excludeId.HasValue)
                q = q.Where(x => x.SupplierId != excludeId.Value);

            return q.AnyAsync(ct);
        }

        public Task<bool> HasStockTransactionsAsync(int supplierId, CancellationToken ct = default)
        {
            return _context.Set<StockTransaction>()
                .AsNoTracking()
                .AnyAsync(x => x.SupplierId == supplierId, ct);
        }
    }
}
