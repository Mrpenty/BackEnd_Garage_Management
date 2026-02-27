using Garage_Management.Application.DTOs.Iventories;

namespace Garage_Management.Application.Interfaces.Services.Inventories
{
    /*
     * Author: KhanhDV
     * Created Date: 26-02-2026
    */
    public interface IInventoryService
    {
        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Lấy danh sách phụ tùng theo hãng phụ tùng
        /// </summary>
        Task<List<InventoryResponse>> GetByBrandIdAsync(int brandId, CancellationToken ct = default);
    }
}
