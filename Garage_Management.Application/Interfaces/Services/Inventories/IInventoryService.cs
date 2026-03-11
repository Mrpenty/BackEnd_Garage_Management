using Garage_Management.Application.DTOs.Iventories;
using Garage_Management.Base.Common.Models;

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
        /// Lấy danh sách phụ tùng có phân trang, lọc, tìm kiếm, sắp xếp
        /// </summary>
        /// <param name="query">
        /// - Filter = "active" | "inactive" | "{brandId}"
        /// - Search: tìm theo PartName, VehicleBrand, ModelCompatible, BrandName
        /// - OrderBy: partname | brandname | sellingprice | lastpurchaseprice | createdat
        /// - SortOrder: ASC | DESC
        /// </param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>
        Task<ApiResponse<PagedResult<InventoryResponse>>> GetPagedAsync(ParamQuery query, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Lấy danh sách phụ tùng theo hãng phụ tùng
        /// </summary>
        Task<List<InventoryResponse>> GetByBrandIdAsync(int brandId, CancellationToken ct = default);
    }
}
