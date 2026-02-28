using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Common.Models.Appointments;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.Appointments
{
    public class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedResult<Appointment>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Appointments
                .Include(x => x.Services)
                    .ThenInclude(x => x.Service)
                        .ThenInclude(x => x.ServiceTasks)
                .Include(x => x.SpareParts)
                    .ThenInclude(x => x.Inventory)
                .AsNoTracking();

            var total = await query.CountAsync(ct);
            var data = await query
                .OrderByDescending(x => x.AppointmentDateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct)
                ;

            return new PagedResult<Appointment>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
        }

        public async Task<PagedResult<Appointment>> GetByCustomerIdAsync(int page, int pageSize, int customerId, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Appointments
                .Include(x => x.Services)
                    .ThenInclude(x => x.Service)
                        .ThenInclude(x => x.ServiceTasks)
                .Include(x => x.SpareParts)
                    .ThenInclude(x => x.Inventory)
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId);

            var total = await query.CountAsync(ct);
            var data = await query
                .OrderByDescending(x => x.AppointmentDateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct)
                ;

            return new PagedResult<Appointment>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = data
            };
        }

        public async Task<PagedResult<Appointment>> GetPagedAsync(AppointmentQuery query, CancellationToken ct = default)
        {
            if (query.Page <= 0) query.Page = 1;
            if (query.PageSize <= 0) query.PageSize = 10;

            var q = _context.Appointments
                .Include(x => x.Services)
                    .ThenInclude(x => x.Service)
                        .ThenInclude(x => x.ServiceTasks)
                .Include(x => x.SpareParts)
                    .ThenInclude(x => x.Inventory)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                q = q.Where(x =>
                    (x.Description ?? "").ToLower().Contains(search) ||
                    (x.FirstName ?? "").ToLower().Contains(search) ||
                    (x.LastName ?? "").ToLower().Contains(search) ||
                    (x.Phone ?? "").ToLower().Contains(search)
                );
            }
            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                var statuses = query.Status
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(s => s.ToLower())
                    .ToList();

                q = q.Where(a =>
                    (statuses.Contains("pending") && a.Status == AppointmentStatus.Pending) ||
                    (statuses.Contains("confirmed") && a.Status == AppointmentStatus.Confirmed) ||
                    (statuses.Contains("inprogress") && a.Status == AppointmentStatus.InProgress) ||
                    (statuses.Contains("completed") && a.Status == AppointmentStatus.Completed) ||
                    ((statuses.Contains("canceled") || statuses.Contains("cancelled")) && a.Status == AppointmentStatus.Cancelled) ||
                    (statuses.Contains("noshow") && a.Status == AppointmentStatus.NoShow) ||
                    (statuses.Contains("convertedtojobcard") && a.Status == AppointmentStatus.ConvertedToJobCard)
                );
            }
            if (query.CustomerId.HasValue)
                q = q.Where(a => a.CustomerId == query.CustomerId);
            var orderBy = (query.OrderBy ?? "").Trim().ToLower();
            var desc = string.Equals(query.SortOrder, "DESC", StringComparison.OrdinalIgnoreCase);

            q = orderBy switch
            {
                "appointmentdatetime" => desc ? q.OrderByDescending(x => x.AppointmentDateTime) : q.OrderBy(x => x.AppointmentDateTime),
                "status" => desc ? q.OrderByDescending(x => x.Status) : q.OrderBy(x => x.Status),
                "createdat" => desc ? q.OrderByDescending(x => x.CreatedAt) : q.OrderBy(x => x.CreatedAt),
                _ => q.OrderByDescending(x => x.AppointmentDateTime)
            };

            var total = await q.CountAsync(ct);
            var data = await q
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(ct);

            return new PagedResult<Appointment>
            {
                Page = query.Page,
                PageSize = query.PageSize,
                Total = total,
                PageData = data
            };
        }

        public async Task<Appointment?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default)
        {
            return await _context.Appointments
                .Include(x => x.Services)
                    .ThenInclude(x => x.Service)
                        .ThenInclude(x => x.ServiceTasks)
                .Include(x => x.SpareParts)
                    .ThenInclude(x => x.Inventory)
                .FirstOrDefaultAsync(x => x.AppointmentId == id, ct);
        }
    }
}
