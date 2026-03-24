using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories.Inventories
{
    public interface IStockTransactionRepository : IBaseRepository<StockTransaction>
    {
        IQueryable<StockTransaction> Query();
        Task<StockTransaction?> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
