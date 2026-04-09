using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.RepairEstimaties;
using Garage_Management.Base.Entities.Warranties;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
namespace Garage_Management.Application.Repositories.Inventories
{



    public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
    {
            private readonly AppDbContext _context;

            public InventoryRepository(AppDbContext context) :base(context)
            {
                _context = context;
            }

            public IQueryable<Inventory> Query()
                => _context.Inventories.AsQueryable();

            public async Task<Inventory?> GetByIdAsync(int id)
                => await _context.Inventories
                    .FirstOrDefaultAsync(x => x.SparePartId == id);

            public async Task<Inventory?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default)
                => await _context.Inventories
                    .Include(x => x.SparePartBrand)
                    .Include(x => x.SparePartCategory)
                    .FirstOrDefaultAsync(x => x.SparePartId == id, ct);

            public async Task<List<Inventory>> GetByBrandIdAsync(int brandId, CancellationToken ct = default)
                => await _context.Inventories
                    .Include(x => x.SparePartBrand)
                    .Where(x => x.SparePartBrandId == brandId)
                    .ToListAsync(ct);

            public async Task<bool> HasDependenciesAsync(int sparePartId, CancellationToken ct = default)
            {
                var hasJobCard = _context.Set<JobCardSparePart>()
                    .AsNoTracking()
                    .AnyAsync(x => x.SparePartId == sparePartId, ct);

                var hasAppointment = _context.Set<AppointmentSparePart>()
                    .AsNoTracking()
                    .AnyAsync(x => x.SparePartId == sparePartId, ct);

                var hasEstimate = _context.Set<RepairEstimateSparePart>()
                    .AsNoTracking()
                    .AnyAsync(x => x.SparePartId == sparePartId, ct);

                var hasWarranty = _context.Set<WarrantySparePart>()
                    .AsNoTracking()
                    .AnyAsync(x => x.SparePartId == sparePartId, ct);

                var hasTransaction = _context.Set<StockTransaction>()
                    .AsNoTracking()
                    .AnyAsync(x => x.SparePartId == sparePartId, ct);

                await Task.WhenAll(hasJobCard, hasAppointment, hasEstimate, hasWarranty, hasTransaction);
                return hasJobCard.Result || hasAppointment.Result || hasEstimate.Result || hasWarranty.Result || hasTransaction.Result;
            }
        }
    }


