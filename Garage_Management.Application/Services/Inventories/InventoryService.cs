using Garage_Management.Application.DTOs.Iventories;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Base.Common.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Garage_Management.Application.Services.Inventories
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repo;

        public InventoryService(IInventoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<PagedResult<InventoryResponse>>> GetPagedAsync(ParamQuery query, CancellationToken ct = default)
        {
            try
            {
                var q = _repo.Query()
                    .Include(x => x.SparePartBrand)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(query.Search))
                {
                    var search = query.Search.Trim().ToLower();
                    q = q.Where(x =>
                        (x.PartName ?? "").ToLower().Contains(search) ||
                        (x.VehicleBrand ?? "").ToLower().Contains(search) ||
                        (x.ModelCompatible ?? "").ToLower().Contains(search) ||
                        (x.SparePartBrand != null && (x.SparePartBrand.BrandName ?? "").ToLower().Contains(search))
                    );
                }

                if (!string.IsNullOrWhiteSpace(query.Filter))
                {
                    var filter = query.Filter.Trim().ToLower();
                    if (filter == "active")
                        q = q.Where(x => x.IsActive);
                    else if (filter == "inactive")
                        q = q.Where(x => !x.IsActive);
                    else if (int.TryParse(filter, out var brandId))
                        q = q.Where(x => x.SparePartBrandId == brandId);
                }

                var orderBy = (query.OrderBy ?? "").Trim().ToLower();
                var desc = string.Equals(query.SortOrder, "DESC", StringComparison.OrdinalIgnoreCase);

                q = orderBy switch
                {
                    "partname" => desc ? q.OrderByDescending(x => x.PartName) : q.OrderBy(x => x.PartName),
                    "brandname" => desc ? q.OrderByDescending(x => x.SparePartBrand != null ? x.SparePartBrand.BrandName : "") : q.OrderBy(x => x.SparePartBrand != null ? x.SparePartBrand.BrandName : ""),
                    "sellingprice" => desc ? q.OrderByDescending(x => x.SellingPrice ?? 0) : q.OrderBy(x => x.SellingPrice ?? 0),
                    "lastpurchaseprice" => desc ? q.OrderByDescending(x => x.LastPurchasePrice ?? 0) : q.OrderBy(x => x.LastPurchasePrice ?? 0),
                    "createdat" => desc ? q.OrderByDescending(x => x.CreatedAt) : q.OrderBy(x => x.CreatedAt),
                    _ => q.OrderByDescending(x => x.CreatedAt)
                };

                var total = await q.CountAsync(ct);

                var items = await q
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .Select(x => new InventoryResponse
                    {
                        SparePartId = x.SparePartId,
                        PartName = x.PartName,
                        Unit = x.Unit,
                        SparePartBrandId = x.SparePartBrandId,
                        SparePartBrandName = x.SparePartBrand != null ? x.SparePartBrand.BrandName : null,
                        LastPurchasePrice = x.LastPurchasePrice,
                        ModelCompatible = x.ModelCompatible,
                        VehicleBrand = x.VehicleBrand,
                        SellingPrice = x.SellingPrice,
                        IsActive = x.IsActive
                    })
                    .ToListAsync(ct);

                var paged = new PagedResult<InventoryResponse>
                {
                    Page = query.Page,
                    PageSize = query.PageSize,
                    Total = total,
                    PageData = items
                };

                return ApiResponse<PagedResult<InventoryResponse>>.SuccessResponse(paged, "OK");
            }
            catch
            {
                return ApiResponse<PagedResult<InventoryResponse>>.ErrorResponse("Có lỗi xảy ra khi lấy danh sách phụ tùng");
            }
        }

        public async Task<List<InventoryResponse>> GetByBrandIdAsync(int brandId, CancellationToken ct = default)
        {
            var data = await _repo.GetByBrandIdAsync(brandId, ct);
            return data.Select(Map).ToList();
        }

        private static InventoryResponse Map(Base.Entities.Inventories.Inventory entity)
        {
            return new InventoryResponse
            {
                SparePartId = entity.SparePartId,
                PartName = entity.PartName,
                Unit = entity.Unit,
                SparePartBrandId = entity.SparePartBrandId,
                SparePartBrandName = entity.SparePartBrand?.BrandName,
                LastPurchasePrice = entity.LastPurchasePrice,
                ModelCompatible = entity.ModelCompatible,
                VehicleBrand = entity.VehicleBrand,
                SellingPrice = entity.SellingPrice,
                IsActive = entity.IsActive
            };
        }
    }
}
