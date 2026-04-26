using Garage_Management.Application.DTOs.StockChecks;
using Garage_Management.Application.Interfaces.Services.Auth;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Services.Inventories
{
    /// <summary>
    /// Tận dụng Inventory + StockTransaction (TransactionType.Adjustment) cho UC-42.
    /// Mỗi phiên kiểm kê được nhóm bởi ReceiptCode dạng "KK-yyyyMMddHHmmss".
    /// </summary>
    public class StockCheckService : IStockCheckService
    {
        private const string ReceiptPrefix = "KK-";
        private readonly AppDbContext _db;
        private readonly ICurrentUserService _currentUser;

        public StockCheckService(AppDbContext db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
        }

        public async Task<PagedResult<StockCheckItemSnapshotResponse>> GetSnapshotAsync(
            ParamQuery query,
            int? categoryId,
            int? brandId,
            List<int>? sparePartIds,
            CancellationToken ct = default)
        {
            var page = query.Page <= 0 ? 1 : query.Page;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            var q = _db.Inventories
                .AsNoTracking()
                .Include(x => x.SparePartCategory)
                .Include(x => x.SparePartBrand)
                .Where(x => x.IsActive && x.DeletedAt == null);

            // Branch scope cho non-admin
            if (!_currentUser.IsAdmin())
            {
                var branchId = _currentUser.GetCurrentBranchId();
                if (branchId.HasValue)
                    q = q.Where(x => x.BranchId == branchId.Value);
            }

            if (categoryId.HasValue) q = q.Where(x => x.CategoryId == categoryId.Value);
            if (brandId.HasValue) q = q.Where(x => x.SparePartBrandId == brandId.Value);
            if (sparePartIds != null && sparePartIds.Count > 0)
                q = q.Where(x => sparePartIds.Contains(x.SparePartId));

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                q = q.Where(x =>
                    (x.PartName ?? string.Empty).ToLower().Contains(search) ||
                    (x.PartCode ?? string.Empty).ToLower().Contains(search));
            }

            var orderBy = (query.OrderBy ?? string.Empty).Trim().ToLower();
            var desc = string.Equals(query.SortOrder, "DESC", StringComparison.OrdinalIgnoreCase);
            q = orderBy switch
            {
                "partname" => desc ? q.OrderByDescending(x => x.PartName) : q.OrderBy(x => x.PartName),
                "partcode" => desc ? q.OrderByDescending(x => x.PartCode) : q.OrderBy(x => x.PartCode),
                "quantity" => desc ? q.OrderByDescending(x => x.Quantity) : q.OrderBy(x => x.Quantity),
                _ => q.OrderBy(x => x.PartName)
            };

            var total = await q.CountAsync(ct);
            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new StockCheckItemSnapshotResponse
                {
                    SparePartId = x.SparePartId,
                    PartCode = x.PartCode,
                    PartName = x.PartName,
                    Unit = x.Unit,
                    CategoryId = x.CategoryId,
                    CategoryName = x.SparePartCategory != null ? x.SparePartCategory.CategoryName : null,
                    SparePartBrandId = x.SparePartBrandId,
                    SparePartBrandName = x.SparePartBrand != null ? x.SparePartBrand.BrandName : null,
                    StockSystem = x.Quantity,
                    BranchId = x.BranchId
                })
                .ToListAsync(ct);

            return new PagedResult<StockCheckItemSnapshotResponse>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = items
            };
        }

        public async Task<StockCheckResultResponse> SubmitAsync(StockCheckSubmitRequest request, CancellationToken ct = default)
        {
            if (request.Items == null || request.Items.Count == 0)
                throw new InvalidOperationException("Phiên kiểm kê phải có ít nhất 1 phụ tùng");

            var checkDate = request.CheckDate ?? DateTime.UtcNow;
            var receiptCode = $"{ReceiptPrefix}{checkDate:yyyyMMddHHmmss}";
            var currentUserId = _currentUser.GetCurrentUserId();

            // Load tất cả inventory liên quan
            var sparePartIds = request.Items.Select(i => i.SparePartId).Distinct().ToList();
            var inventories = await _db.Inventories
                .Where(x => sparePartIds.Contains(x.SparePartId) && x.DeletedAt == null)
                .ToListAsync(ct);

            // Validate: các SparePartId phải tồn tại
            var missing = sparePartIds.Except(inventories.Select(x => x.SparePartId)).ToList();
            if (missing.Count > 0)
                throw new InvalidOperationException($"SparePartId không tồn tại: {string.Join(", ", missing)}");

            // Validate branch scope
            var branchIds = inventories.Select(x => x.BranchId).Distinct().ToList();
            if (branchIds.Count > 1)
                throw new InvalidOperationException("Các phụ tùng được kiểm kê phải thuộc cùng 1 chi nhánh");

            var sessionBranchId = branchIds[0];
            if (!_currentUser.IsAdmin())
            {
                var userBranchId = _currentUser.GetCurrentBranchId();
                if (!userBranchId.HasValue || userBranchId.Value != sessionBranchId)
                    throw new UnauthorizedAccessException("Không có quyền kiểm kê chi nhánh khác");
            }

            var adjustments = new List<StockCheckAdjustmentResponse>();
            int shortage = 0, surplus = 0;

            foreach (var item in request.Items)
            {
                var inv = inventories.First(x => x.SparePartId == item.SparePartId);
                var delta = item.StockActual - inv.Quantity;

                if (delta == 0) continue;

                if (string.IsNullOrWhiteSpace(item.Reason))
                    throw new InvalidOperationException($"Phải nhập lý do cho phụ tùng {inv.PartName} (SparePartId={inv.SparePartId}) khi có chênh lệch");

                var transaction = new StockTransaction
                {
                    BranchId = inv.BranchId,
                    SparePartId = inv.SparePartId,
                    TransactionType = TransactionType.Adjustment,
                    QuantityChange = delta,
                    UnitPrice = 0,
                    ReceiptCode = receiptCode,
                    Note = $"[Kiểm kê] {item.Reason}",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = currentUserId
                };

                inv.Quantity = item.StockActual;
                inv.UpdatedAt = DateTime.UtcNow;
                inv.UpdatedBy = currentUserId;

                _db.StockTransactions.Add(transaction);

                if (delta < 0) shortage++;
                else surplus++;

                adjustments.Add(new StockCheckAdjustmentResponse
                {
                    SparePartId = inv.SparePartId,
                    PartCode = inv.PartCode,
                    PartName = inv.PartName,
                    StockSystem = inv.Quantity - delta,
                    StockActual = item.StockActual,
                    Delta = delta,
                    Reason = item.Reason
                });
            }

            await _db.SaveChangesAsync(ct);

            var savedTransactions = await _db.StockTransactions
                .AsNoTracking()
                .Where(x => x.ReceiptCode == receiptCode)
                .ToListAsync(ct);

            foreach (var adj in adjustments)
            {
                var tx = savedTransactions.FirstOrDefault(x => x.SparePartId == adj.SparePartId);
                if (tx != null) adj.StockTransactionId = tx.StockTransactionId;
            }

            return new StockCheckResultResponse
            {
                ReceiptCode = receiptCode,
                CheckDate = checkDate,
                Scope = request.Scope,
                BranchId = sessionBranchId,
                TotalItems = request.Items.Count,
                DiscrepanciesAdjusted = adjustments.Count,
                ShortageCount = shortage,
                SurplusCount = surplus,
                Adjustments = adjustments
            };
        }

        public async Task<StockCheckSessionResponse?> GetByReceiptCodeAsync(string receiptCode, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(receiptCode)) return null;

            var q = _db.StockTransactions
                .AsNoTracking()
                .Include(x => x.Inventory)
                .Where(x => x.ReceiptCode == receiptCode && x.TransactionType == TransactionType.Adjustment);

            if (!_currentUser.IsAdmin())
            {
                var branchId = _currentUser.GetCurrentBranchId();
                if (branchId.HasValue)
                    q = q.Where(x => x.BranchId == branchId.Value);
            }

            var transactions = await q.ToListAsync(ct);
            if (transactions.Count == 0) return null;

            var first = transactions[0];
            var adjustments = transactions.Select(x => new StockCheckAdjustmentResponse
            {
                StockTransactionId = x.StockTransactionId,
                SparePartId = x.SparePartId,
                PartCode = x.Inventory?.PartCode,
                PartName = x.Inventory?.PartName ?? string.Empty,
                StockSystem = (x.Inventory?.Quantity ?? 0) - x.QuantityChange,
                StockActual = x.Inventory?.Quantity ?? 0,
                Delta = x.QuantityChange,
                Reason = x.Note
            }).ToList();

            return new StockCheckSessionResponse
            {
                ReceiptCode = receiptCode,
                CheckDate = first.CreatedAt,
                BranchId = first.BranchId,
                CreatedBy = first.CreatedBy,
                DiscrepanciesAdjusted = adjustments.Count,
                ShortageCount = adjustments.Count(a => a.Delta < 0),
                SurplusCount = adjustments.Count(a => a.Delta > 0),
                Adjustments = adjustments
            };
        }

        public async Task<PagedResult<StockCheckSessionResponse>> GetPagedSessionsAsync(
            ParamQuery query,
            DateTime? from,
            DateTime? to,
            CancellationToken ct = default)
        {
            var page = query.Page <= 0 ? 1 : query.Page;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            var baseQ = _db.StockTransactions
                .AsNoTracking()
                .Where(x => x.TransactionType == TransactionType.Adjustment
                    && x.ReceiptCode != null
                    && x.ReceiptCode.StartsWith(ReceiptPrefix));

            if (!_currentUser.IsAdmin())
            {
                var branchId = _currentUser.GetCurrentBranchId();
                if (branchId.HasValue)
                    baseQ = baseQ.Where(x => x.BranchId == branchId.Value);
            }

            if (from.HasValue) baseQ = baseQ.Where(x => x.CreatedAt >= from.Value);
            if (to.HasValue) baseQ = baseQ.Where(x => x.CreatedAt <= to.Value);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                baseQ = baseQ.Where(x => (x.ReceiptCode ?? string.Empty).ToLower().Contains(search));
            }

            var sessionsQ = baseQ
                .GroupBy(x => x.ReceiptCode!)
                .Select(g => new
                {
                    ReceiptCode = g.Key,
                    CheckDate = g.Min(x => x.CreatedAt),
                    BranchId = g.Min(x => x.BranchId),
                    CreatedBy = g.Min(x => x.CreatedBy),
                    DiscrepanciesAdjusted = g.Count(),
                    ShortageCount = g.Count(x => x.QuantityChange < 0),
                    SurplusCount = g.Count(x => x.QuantityChange > 0)
                });

            var desc = !string.Equals(query.SortOrder, "ASC", StringComparison.OrdinalIgnoreCase);
            sessionsQ = desc
                ? sessionsQ.OrderByDescending(x => x.CheckDate)
                : sessionsQ.OrderBy(x => x.CheckDate);

            var total = await sessionsQ.CountAsync(ct);
            var items = await sessionsQ
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<StockCheckSessionResponse>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                PageData = items.Select(x => new StockCheckSessionResponse
                {
                    ReceiptCode = x.ReceiptCode,
                    CheckDate = x.CheckDate,
                    BranchId = x.BranchId,
                    CreatedBy = x.CreatedBy,
                    DiscrepanciesAdjusted = x.DiscrepanciesAdjusted,
                    ShortageCount = x.ShortageCount,
                    SurplusCount = x.SurplusCount,
                    Adjustments = new List<StockCheckAdjustmentResponse>() // không load chi tiết khi list
                }).ToList()
            };
        }
    }
}
