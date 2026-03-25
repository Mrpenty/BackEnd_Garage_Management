using Garage_Management.Application.DTOs.Iventories.StockTransactions;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Inventories;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Services.Inventories
{
    public class StockTransactionService : IStockTransactionService
    {
        private readonly IStockTransactionRepository _repo;
        private readonly IInventoryRepository _inventoryRepo;

        public StockTransactionService(IStockTransactionRepository repo, IInventoryRepository inventoryRepo)
        {
            _repo = repo;
            _inventoryRepo = inventoryRepo;
        }

        public async Task<PagedResult<StockTransactionResponse>> GetPagedAsync(ParamQuery query, CancellationToken ct = default)
        {
            var page = query.Page <= 0 ? 1 : query.Page;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            var q = _repo.Query()
                .AsNoTracking()
                .Include(x => x.Inventory)
                .Include(x => x.Supplier)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                q = q.Where(x =>
                    (x.Inventory.PartName ?? string.Empty).ToLower().Contains(search) ||
                    (x.ReceiptCode ?? string.Empty).ToLower().Contains(search) ||
                    (x.Note ?? string.Empty).ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var filter = query.Filter.Trim().ToLower();
                if (filter == "import") q = q.Where(x => x.TransactionType == TransactionType.Import);
                else if (filter == "export") q = q.Where(x => x.TransactionType == TransactionType.ExportToJobCard);
                else if (filter == "return") q = q.Where(x => x.TransactionType == TransactionType.ReturnFromJobCard);
                else if (filter == "adjust") q = q.Where(x => x.TransactionType == TransactionType.Adjustment);
                else if (int.TryParse(filter, out var sparePartId)) q = q.Where(x => x.SparePartId == sparePartId);
            }

            var desc = string.Equals(query.SortOrder, "DESC", StringComparison.OrdinalIgnoreCase);
            var orderBy = (query.OrderBy ?? string.Empty).Trim().ToLower();
            q = orderBy switch
            {
                "unitprice" => desc ? q.OrderByDescending(x => x.UnitPrice) : q.OrderBy(x => x.UnitPrice),
                "quantitychange" => desc ? q.OrderByDescending(x => x.QuantityChange) : q.OrderBy(x => x.QuantityChange),
                _ => desc ? q.OrderByDescending(x => x.CreatedAt) : q.OrderBy(x => x.CreatedAt)
            };

            var total = await q.CountAsync(ct);
            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

            return new PagedResult<StockTransactionResponse>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = items.Select(Map).ToList()
            };
        }

        public async Task<StockTransactionResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            return entity == null ? null : Map(entity);
        }

        public async Task<StockTransactionResponse> CreateAsync(StockTransactionCreateRequest request, CancellationToken ct = default)
        {
            var inventory = await _inventoryRepo.GetByIdAsync(request.SparePartId);
            if (inventory == null)
                throw new InvalidOperationException("SparePartId không tồn tại");

            if (request.TransactionType != TransactionType.Adjustment && request.QuantityChange <= 0)
                throw new InvalidOperationException("QuantityChange phải lớn hơn 0");

            if (request.TransactionType == TransactionType.Adjustment)
            {
                if (!request.ActualQuantity.HasValue)
                    throw new InvalidOperationException("Adjustment bắt buộc nhập ActualQuantity");
                if (request.ActualQuantity.Value < 0)
                    throw new InvalidOperationException("ActualQuantity không hợp lệ");
            }

            // Normalize quantity by transaction type:
            // - Import/Return: positive
            // - Export: negative
            // - Adjustment: convert actual quantity to delta
            var effectiveQuantityChange = request.TransactionType switch
            {
                TransactionType.Import => request.QuantityChange,
                TransactionType.ReturnFromJobCard => request.QuantityChange,
                TransactionType.ExportToJobCard => -request.QuantityChange,
                _ => request.ActualQuantity!.Value - inventory.Quantity
            };

            if (effectiveQuantityChange == 0)
                throw new InvalidOperationException("Không có thay đổi tồn kho");

            var nextQuantity = inventory.Quantity + effectiveQuantityChange;
            if (nextQuantity < 0)
                throw new InvalidOperationException("Số lượng tồn kho không đủ");

            var entity = new StockTransaction
            {
                SparePartId = request.SparePartId,
                TransactionType = request.TransactionType,
                QuantityChange = effectiveQuantityChange,
                UnitPrice = request.UnitPrice,
                SupplierId = request.SupplierId,
                JobCardId = request.JobCardId,
                ReceiptCode = request.ReceiptCode,
                LotNumber = request.LotNumber,
                SerialNumber = request.SerialNumber,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow
            };

            inventory.Quantity = nextQuantity;
            if (request.TransactionType == TransactionType.Import && request.UnitPrice > 0)
                inventory.LastPurchasePrice = request.UnitPrice;
            inventory.UpdatedAt = DateTime.UtcNow;

            _inventoryRepo.Update(inventory);
            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);

            var detail = await _repo.GetByIdAsync(entity.StockTransactionId, ct);
            return detail == null ? Map(entity) : Map(detail);
        }

        private static StockTransactionResponse Map(StockTransaction entity)
        {
            return new StockTransactionResponse
            {
                StockTransactionId = entity.StockTransactionId,
                SparePartId = entity.SparePartId,
                PartCode = entity.Inventory?.PartCode,
                PartName = entity.Inventory?.PartName ?? string.Empty,
                TransactionType = entity.TransactionType,
                QuantityChange = entity.QuantityChange,
                UnitPrice = entity.UnitPrice,
                SupplierId = entity.SupplierId,
                SupplierName = entity.Supplier?.SupplierName,
                JobCardId = entity.JobCardId,
                ReceiptCode = entity.ReceiptCode,
                LotNumber = entity.LotNumber,
                SerialNumber = entity.SerialNumber,
                Note = entity.Note,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
