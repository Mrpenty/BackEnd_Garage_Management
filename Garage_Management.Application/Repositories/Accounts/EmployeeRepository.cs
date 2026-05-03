using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Repositories.Accounts
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
        public async Task<Employee?> GetByUserIdAsync(int userId)
        {
            return await dbSet
            .FirstOrDefaultAsync(c => c.UserId == userId);
        }
        public async Task<List<Employee>> GetAllMechanicsAsync(int? branchId = null, CancellationToken ct = default)
        {
            var mechanicUserIds =
                from ur in _context.UserRoles
                join r in _context.Roles on ur.RoleId equals r.Id
                where r.Name == "Mechanic"
                select ur.UserId;

            var q = dbSet
                .AsNoTracking()
                .Where(e => e.IsActive && mechanicUserIds.Contains(e.UserId));

            if (branchId.HasValue)
                q = q.Where(e => e.BranchId == branchId.Value);

            return await q.ToListAsync(ct);
        }
    }

}
