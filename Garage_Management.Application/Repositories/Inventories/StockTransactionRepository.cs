using Garage_Management.Application.Interfaces.Repositories.Inventories;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Repositories.Inventories
{
    public class StockTransactionRepository : BaseRepository<StockTransaction>,IStockTransactionRepository
    {
        private readonly AppDbContext _context;

        public StockTransactionRepository(AppDbContext context) :base(context)
        {
            _context = context;
        }

        public IQueryable<StockTransaction> Query()
            => _context.StockTransactions.AsQueryable();

        public async Task<StockTransaction?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _context.StockTransactions
                .Include(x => x.Inventory)
                    .ThenInclude(i => i.SparePartBrand)
                .Include(x => x.Inventory)
                    .ThenInclude(i => i.SparePartCategory)
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(x => x.StockTransactionId == id, ct);
    }
}
