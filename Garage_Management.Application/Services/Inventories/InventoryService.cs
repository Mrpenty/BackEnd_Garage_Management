using Garage_Management.Application.DTOs.Iventories;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services.Inventories;
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
