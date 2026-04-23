using Garage_Management.Application.Interfaces.Repositories.Branches;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Branches;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.Branches
{
    public class BranchRepository : BaseRepository<Branch>, IBranchRepository
    {
        private readonly AppDbContext _context;

        public BranchRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedResult<Branch>> GetPagedAsync(ParamQuery query, int? scopedBranchId, CancellationToken ct = default)
        {
            var page = query.Page <= 0 ? 1 : query.Page;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            var q = GetAll()
                .AsNoTracking()
                .Where(x => x.DeletedAt == null)
                .Include(x => x.ManagerEmployee)
                .AsQueryable();

            if (scopedBranchId.HasValue)
            {
                q = q.Where(x => x.BranchId == scopedBranchId.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                q = q.Where(x =>
                    x.Name.ToLower().Contains(search) ||
                    x.BranchCode.ToLower().Contains(search) ||
                    x.Address.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(query.Filter) && bool.TryParse(query.Filter, out var isActive))
            {
                q = q.Where(x => x.IsActive == isActive);
            }

            var orderBy = (query.OrderBy ?? string.Empty).Trim().ToLower();
            var desc = string.Equals(query.SortOrder, "DESC", StringComparison.OrdinalIgnoreCase);
            q = orderBy switch
            {
                "name" => desc ? q.OrderByDescending(x => x.Name) : q.OrderBy(x => x.Name),
                "branchcode" => desc ? q.OrderByDescending(x => x.BranchCode) : q.OrderBy(x => x.BranchCode),
                "isactive" => desc ? q.OrderByDescending(x => x.IsActive) : q.OrderBy(x => x.IsActive),
                _ => desc ? q.OrderByDescending(x => x.BranchId) : q.OrderBy(x => x.BranchId)
            };

            var total = await q.CountAsync(ct);
            var data = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

            return new PagedResult<Branch>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
        }

        public async Task<Branch?> GetDetailByIdAsync(int id, CancellationToken ct = default)
        {
            return await GetAll()
                .AsNoTracking()
                .Include(x => x.ManagerEmployee)
                .FirstOrDefaultAsync(x => x.BranchId == id && x.DeletedAt == null, ct);
        }

        public Task<bool> CodeExistsAsync(string branchCode, int? excludeId = null, CancellationToken ct = default)
        {
            var code = branchCode.Trim().ToLower();
            var q = GetAll().AsNoTracking().Where(x => x.BranchCode.ToLower() == code && x.DeletedAt == null);
            if (excludeId.HasValue)
                q = q.Where(x => x.BranchId != excludeId.Value);
            return q.AnyAsync(ct);
        }

        public Task<int> CountEmployeesAsync(int branchId, CancellationToken ct = default)
        {
            return _context.Employees
                .AsNoTracking()
                .CountAsync(e => e.BranchId == branchId && e.DeletedAt == null, ct);
        }

        public Task<int> CountActiveJobCardsAsync(int branchId, CancellationToken ct = default)
        {
            return _context.JobCards
                .AsNoTracking()
                .CountAsync(j => j.BranchId == branchId
                    && j.DeletedAt == null
                    && j.Status != JobCardStatus.Completed
                    && j.Status != JobCardStatus.Rejected, ct);
        }

        public async Task<bool> HasDependenciesAsync(int branchId, CancellationToken ct = default)
        {
            var hasEmployees = await _context.Employees.AsNoTracking()
                .AnyAsync(e => e.BranchId == branchId && e.DeletedAt == null, ct);
            if (hasEmployees) return true;

            var hasJobCards = await _context.JobCards.AsNoTracking()
                .AnyAsync(j => j.BranchId == branchId && j.DeletedAt == null, ct);
            if (hasJobCards) return true;

            var hasInventory = await _context.Inventories.AsNoTracking()
                .AnyAsync(i => i.BranchId == branchId && i.DeletedAt == null, ct);
            return hasInventory;
        }
    }
}
