using Garage_Management.Application.DTOs.Reports;
using Garage_Management.Application.Interfaces.Services.Auth;
using Garage_Management.Application.Interfaces.Services.Reports;
using Garage_Management.Base.Data;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _db;
        private readonly ICurrentUserService _currentUser;

        public ReportService(AppDbContext db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<BranchRevenueResponse?> GetBranchRevenueAsync(int branchId, DateTime? from, DateTime? to, CancellationToken ct = default)
        {
            EnsureCanAccess(branchId);

            var branch = await _db.Branches.AsNoTracking()
                .FirstOrDefaultAsync(b => b.BranchId == branchId && b.DeletedAt == null, ct);
            if (branch == null) return null;

            var q = _db.Invoices.AsNoTracking()
                .Where(i => i.BranchId == branchId && i.DeletedAt == null);

            if (from.HasValue) q = q.Where(i => i.InvoiceDate >= from.Value);
            if (to.HasValue) q = q.Where(i => i.InvoiceDate <= to.Value);

            var aggregate = await q
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Count = g.Count(),
                    ServiceTotal = g.Sum(x => x.ServiceTotal),
                    SparePartTotal = g.Sum(x => x.SparePartTotal),
                    GrandTotal = g.Sum(x => x.GrandTotal)
                })
                .FirstOrDefaultAsync(ct);

            return new BranchRevenueResponse
            {
                BranchId = branch.BranchId,
                BranchCode = branch.BranchCode,
                BranchName = branch.Name,
                FromDate = from,
                ToDate = to,
                InvoiceCount = aggregate?.Count ?? 0,
                ServiceTotal = aggregate?.ServiceTotal ?? 0m,
                SparePartTotal = aggregate?.SparePartTotal ?? 0m,
                GrandTotal = aggregate?.GrandTotal ?? 0m
            };
        }

        public async Task<BranchJobCardSummaryResponse?> GetBranchJobCardSummaryAsync(int branchId, DateTime? from, DateTime? to, CancellationToken ct = default)
        {
            EnsureCanAccess(branchId);

            var branch = await _db.Branches.AsNoTracking()
                .FirstOrDefaultAsync(b => b.BranchId == branchId && b.DeletedAt == null, ct);
            if (branch == null) return null;

            var q = _db.JobCards.AsNoTracking()
                .Where(j => j.BranchId == branchId && j.DeletedAt == null);

            if (from.HasValue) q = q.Where(j => j.StartDate >= from.Value);
            if (to.HasValue) q = q.Where(j => j.StartDate <= to.Value);

            var breakdown = await q
                .GroupBy(j => j.Status)
                .Select(g => new JobCardStatusCount
                {
                    Status = g.Key,
                    StatusName = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToListAsync(ct);

            return new BranchJobCardSummaryResponse
            {
                BranchId = branch.BranchId,
                BranchCode = branch.BranchCode,
                BranchName = branch.Name,
                FromDate = from,
                ToDate = to,
                TotalCount = breakdown.Sum(x => x.Count),
                StatusBreakdown = breakdown
            };
        }

        public async Task<List<BranchRevenueResponse>> GetRevenueByBranchAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
        {
            if (!_currentUser.IsAdmin())
                throw new UnauthorizedAccessException("Chỉ Admin được xem báo cáo tất cả chi nhánh");

            var branches = await _db.Branches.AsNoTracking()
                .Where(b => b.DeletedAt == null)
                .OrderBy(b => b.BranchId)
                .Select(b => new { b.BranchId, b.BranchCode, Name = b.Name })
                .ToListAsync(ct);

            var invoiceQuery = _db.Invoices.AsNoTracking()
                .Where(i => i.DeletedAt == null);
            if (from.HasValue) invoiceQuery = invoiceQuery.Where(i => i.InvoiceDate >= from.Value);
            if (to.HasValue) invoiceQuery = invoiceQuery.Where(i => i.InvoiceDate <= to.Value);

            var aggregates = await invoiceQuery
                .GroupBy(i => i.BranchId)
                .Select(g => new
                {
                    BranchId = g.Key,
                    Count = g.Count(),
                    ServiceTotal = g.Sum(x => x.ServiceTotal),
                    SparePartTotal = g.Sum(x => x.SparePartTotal),
                    GrandTotal = g.Sum(x => x.GrandTotal)
                })
                .ToListAsync(ct);

            return branches.Select(b =>
            {
                var agg = aggregates.FirstOrDefault(a => a.BranchId == b.BranchId);
                return new BranchRevenueResponse
                {
                    BranchId = b.BranchId,
                    BranchCode = b.BranchCode,
                    BranchName = b.Name,
                    FromDate = from,
                    ToDate = to,
                    InvoiceCount = agg?.Count ?? 0,
                    ServiceTotal = agg?.ServiceTotal ?? 0m,
                    SparePartTotal = agg?.SparePartTotal ?? 0m,
                    GrandTotal = agg?.GrandTotal ?? 0m
                };
            }).ToList();
        }

        private void EnsureCanAccess(int branchId)
        {
            if (_currentUser.IsAdmin()) return;
            var scoped = _currentUser.GetCurrentBranchId();
            if (scoped.HasValue && scoped.Value == branchId) return;
            throw new UnauthorizedAccessException("Không có quyền xem báo cáo của chi nhánh khác");
        }
    }
}
