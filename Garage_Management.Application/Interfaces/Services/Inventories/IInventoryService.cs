using Garage_Management.Application.DTOs.Iventories;

namespace Garage_Management.Application.Interfaces.Services.Inventories
{
    public interface IInventoryService
    {
        Task<List<InventoryResponse>> GetByBrandIdAsync(int brandId, CancellationToken ct = default);
    }
}
