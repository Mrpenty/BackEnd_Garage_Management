using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Repositories
{
    using Garage_Management.Base.Entities.Inventories;

    public interface IInventoryRepository
    {
        Task<Inventory?> GetByIdAsync(int id);

        IQueryable<Inventory> Query();

        Task SaveAsync(CancellationToken cancellationToken);
    }

}
