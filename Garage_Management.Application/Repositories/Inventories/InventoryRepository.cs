using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Base.Data;
 using Garage_Management.Base.Entities.Inventories;
using Microsoft.EntityFrameworkCore;
namespace Garage_Management.Application.Repositories.Inventories
{
    

  
        public class InventoryRepository : IInventoryRepository
        {
            private readonly AppDbContext _context;

            public InventoryRepository(AppDbContext context)
            {
                _context = context;
            }

            public IQueryable<Inventory> Query()
                => _context.Inventories.AsQueryable();

            public async Task<Inventory?> GetByIdAsync(int id)
                => await _context.Inventories
                    .FirstOrDefaultAsync(x => x.SparePartId == id);

            public async Task SaveAsync(CancellationToken cancellationToken)
                => await _context.SaveChangesAsync(cancellationToken);
        }
    }


