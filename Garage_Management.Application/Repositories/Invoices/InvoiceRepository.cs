using Garage_Management.Application.Interfaces.Repositories.Invoices;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Common.Models.Invoices;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.Invoices
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        private readonly AppDbContext _context;

        public InvoiceRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Invoice?> GetByIdWithDetailsAsync(int invoiceId, CancellationToken ct = default)
        {
            return await _context.Invoices
                .Include(i => i.JobCard)
                    .ThenInclude(j => j.Customer)
                .Include(i => i.JobCard)
                    .ThenInclude(j => j.Vehicle)
                .Include(i => i.JobCard)
                    .ThenInclude(j => j.Appointment)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId, ct);
        }

        public async Task<PagedResult<Invoice>> GetPagedAsync(InvoiceQuery query, CancellationToken ct = default)
        {
            if (query.Page <= 0) query.Page = 1;
            if (query.PageSize <= 0) query.PageSize = 10;

            var q = _context.Invoices
                .Include(i => i.JobCard)
                    .ThenInclude(j => j.Customer)
                .Include(i => i.JobCard)
                    .ThenInclude(j => j.Vehicle)
                .AsNoTracking()
                .AsQueryable();

            // Search theo InvoiceId hoac ten khach hang
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                q = q.Where(i =>
                    i.InvoiceId.ToString().Contains(search) ||
                    (i.JobCard.Customer.FirstName ?? "").ToLower().Contains(search) ||
                    (i.JobCard.Customer.LastName ?? "").ToLower().Contains(search));
            }

            // Loc theo PaymentStatus
            if (!string.IsNullOrWhiteSpace(query.PaymentStatus))
            {
                q = q.Where(i => i.PaymentStatus == query.PaymentStatus);
            }

            // Loc theo CustomerId
            if (query.CustomerId.HasValue)
            {
                q = q.Where(i => i.JobCard.CustomerId == query.CustomerId.Value);
            }

            // Loc theo JobCardId
            if (query.JobCardId.HasValue)
            {
                q = q.Where(i => i.JobCardId == query.JobCardId.Value);
            }

            // Sap xep
            var orderBy = (query.OrderBy ?? "").Trim().ToLower();
            var desc = string.Equals(query.SortOrder, "DESC", StringComparison.OrdinalIgnoreCase);

            q = orderBy switch
            {
                "invoicedate" => desc ? q.OrderByDescending(i => i.InvoiceDate) : q.OrderBy(i => i.InvoiceDate),
                "grandtotal" => desc ? q.OrderByDescending(i => i.GrandTotal) : q.OrderBy(i => i.GrandTotal),
                "paymentstatus" => desc ? q.OrderByDescending(i => i.PaymentStatus) : q.OrderBy(i => i.PaymentStatus),
                _ => q.OrderByDescending(i => i.InvoiceDate)
            };

            var total = await q.CountAsync(ct);
            var data = await q
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(ct);

            return new PagedResult<Invoice>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                Total = total,
                PageData = data
            };
        }

        public async Task<Invoice?> GetByJobCardIdAsync(int jobCardId, CancellationToken ct = default)
        {
            return await _context.Invoices
                .FirstOrDefaultAsync(i => i.JobCardId == jobCardId, ct);
        }

        public async Task SaveAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
