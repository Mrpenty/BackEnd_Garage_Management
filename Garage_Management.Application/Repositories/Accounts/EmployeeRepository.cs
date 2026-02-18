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
        public EmployeeRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<Employee?> GetByUserIdAsync(int userId)
        {
            return await dbSet
            .FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
