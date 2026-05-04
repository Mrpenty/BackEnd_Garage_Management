using Garage_Management.Application.DTOs.Reports;
using Garage_Management.Application.Interfaces.Services.Auth;
using Garage_Management.Application.Interfaces.Services.Reports;
using Garage_Management.Base.Common.Enums;
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

        public async Task<ReceptionistReportResponse?> GetReceptionistReportAsync(int branchId, DateTime? from, DateTime? to, CancellationToken ct = default)
        {
            EnsureCanAccess(branchId);

            var branch = await _db.Branches.AsNoTracking()
                .FirstOrDefaultAsync(b => b.BranchId == branchId && b.DeletedAt == null, ct);
            if (branch == null) return null;

            var apptQuery = _db.Appointments.AsNoTracking()
                .Where(a => a.BranchId == branchId);
            if (from.HasValue) apptQuery = apptQuery.Where(a => a.AppointmentDateTime >= from.Value);
            if (to.HasValue) apptQuery = apptQuery.Where(a => a.AppointmentDateTime <= to.Value);

            var statusGroups = await apptQuery
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync(ct);

            int CountFor(AppointmentStatus s) =>
                statusGroups.FirstOrDefault(x => x.Status == s)?.Count ?? 0;

            var breakdown = new AppointmentStatusBreakdown
            {
                Pending = CountFor(AppointmentStatus.Pending),
                Confirmed = CountFor(AppointmentStatus.Confirmed),
                ConvertedToJobCard = CountFor(AppointmentStatus.ConvertedToJobCard),
                Completed = CountFor(AppointmentStatus.Completed),
                Cancelled = CountFor(AppointmentStatus.Cancelled),
                NoShow = CountFor(AppointmentStatus.NoShow)
            };

            var total = statusGroups.Sum(x => x.Count);
            double Rate(int part) => total == 0 ? 0d : (double)part / total;

            // Walk-in vs theo lịch (đếm trên JobCards trong khoảng)
            var jcQuery = _db.JobCards.AsNoTracking()
                .Where(j => j.BranchId == branchId && j.DeletedAt == null);
            if (from.HasValue) jcQuery = jcQuery.Where(j => j.StartDate >= from.Value);
            if (to.HasValue) jcQuery = jcQuery.Where(j => j.StartDate <= to.Value);

            var walkIn = await jcQuery.CountAsync(j => j.AppointmentId == null, ct);
            var apptBased = await jcQuery.CountAsync(j => j.AppointmentId != null, ct);

            // Phiếu sửa do user hiện tại (lễ tân) tạo
            var currentUserId = _currentUser.GetCurrentUserId();
            var createdByMe = currentUserId.HasValue
                ? await jcQuery.CountAsync(j => j.CreatedBy == currentUserId.Value, ct)
                : 0;

            return new ReceptionistReportResponse
            {
                BranchId = branch.BranchId,
                BranchName = branch.Name,
                FromDate = from,
                ToDate = to,
                TotalAppointments = total,
                AppointmentsByStatus = breakdown,
                NoShowRate = Rate(breakdown.NoShow),
                CancelRate = Rate(breakdown.Cancelled),
                ConversionRate = Rate(breakdown.ConvertedToJobCard + breakdown.Completed),
                WalkInCount = walkIn,
                AppointmentBasedCount = apptBased,
                JobCardsCreatedByMe = createdByMe
            };
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
