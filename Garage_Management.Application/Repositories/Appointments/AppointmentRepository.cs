using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.Appointments
{
    public class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(AppDbContext context) : base(context) { }

        public async Task<PagedResult<Appointment>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = GetAll().AsNoTracking();

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

            var query = GetAll()
                .AsNoTracking()
                .Where(x=>x.CustomerId == customerId);

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
    }
}
