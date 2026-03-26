using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories
{
    public interface ISupplierRepository : IBaseRepository<Supplier>
    {
        Task<PagedResult<Supplier>> GetPagedAsync(ParamQuery query, CancellationToken ct = default);
        Task<bool> HasExistAsync(string supplierName, int? excludeId = null, CancellationToken ct = default);
        Task<bool> HasStockTransactionsAsync(int supplierId, CancellationToken ct = default);
    }
}
