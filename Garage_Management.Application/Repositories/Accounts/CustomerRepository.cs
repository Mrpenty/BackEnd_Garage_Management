using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.Application.Repositories.Accounts
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public Task<Customer?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return GetAll().AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId, ct);
        }
    }
}
