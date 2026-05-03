using System;
using System.Collections.Generic;
using System.Linq;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Interface;
namespace Garage_Management.Application.Interfaces.Repositories
{
    public interface IInventoryRepository : IBaseRepository<Inventory>
    {
        Task<Inventory?> GetByIdAsync(int id);
        Task<Inventory?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default);

        IQueryable<Inventory> Query();

        Task<List<Inventory>> GetByBrandIdAsync(int brandId, int? branchId = null, CancellationToken ct = default);
        Task<bool> HasDependenciesAsync(int sparePartId, CancellationToken ct = default);

    }

}
