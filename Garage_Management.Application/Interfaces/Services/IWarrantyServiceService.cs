using Garage_Management.Application.DTOs.WarrantyServices;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services
{
    /*
     * Author: KhanhDV
     * Created Date: 26-02-2026
    */
    public interface IWarrantyServiceService
    {
        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Lấy chi tiết bảo hành dịch vụ
        /// </summary>
        Task<WarrantyServiceResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Lấy danh sách bảo hành dịch vụ có phân trang
        /// </summary>
        Task<PagedResult<WarrantyServiceResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Tạo bảo hành dịch vụ
        /// </summary>
        Task<WarrantyServiceResponse> CreateAsync(WarrantyServiceCreateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Cập nhật bảo hành dịch vụ
        /// </summary>
        Task<WarrantyServiceResponse?> UpdateAsync(int id, WarrantyServiceUpdateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Xóa bảo hành dịch vụ
        /// </summary>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
