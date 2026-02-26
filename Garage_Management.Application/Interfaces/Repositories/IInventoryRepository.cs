using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Garage_Management.Base.Entities.Inventories;
namespace Garage_Management.Application.Interfaces.Repositories
{
    

    public interface IInventoryRepository
    {
        Task<Inventory?> GetByIdAsync(int id);

        IQueryable<Inventory> Query();

        Task<List<Inventory>> GetByBrandIdAsync(int brandId, CancellationToken ct = default);

        Task SaveAsync(CancellationToken cancellationToken);
    }

}
