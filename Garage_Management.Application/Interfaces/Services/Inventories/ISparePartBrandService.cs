using Garage_Management.Application.DTOs.Inventories.SparePartBrands;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services.Inventories
{
    /*
     * Author: KhanhDV
     * Created Date: 26-02-2026
    */
    public interface ISparePartBrandService
    {
        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Lấy chi tiết hãng phụ tùng
        /// </summary>
        Task<SparePartBrandResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Lấy danh sách hãng phụ tùng có phân trang
        /// </summary>
        Task<PagedResult<SparePartBrandResponse>> GetPagedAsync(ParamQuery query, bool onlyActive = false, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Tạo hãng phụ tùng
        /// </summary>
        Task<SparePartBrandResponse> CreateAsync(SparePartBrandCreateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Cập nhật hãng phụ tùng
        /// </summary>
        Task<SparePartBrandResponse?> UpdateAsync(int id, SparePartBrandUpdateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Xóa hãng phụ tùng
        /// </summary>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
