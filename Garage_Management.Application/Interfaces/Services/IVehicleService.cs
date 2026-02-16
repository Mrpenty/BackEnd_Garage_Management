using Garage_Management.Application.DTOs.Vehicles;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services
{
    /*
     * Author: KhanhDV
     * Created Date: 13-02-2026
    */
    public interface IVehicleService
    {
        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy chi tiết 1 xe máy
        /// </summary>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>   
        Task<VehicleResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách xe máy theo khách hàng có phân trạng
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="customerId">Id của khách hàng</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>   
        Task<PagedResult<VehicleResponse>> GetByCustomerIdAsync(int page, int pageSize, int customerId, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách xe máy có phân trạng
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>   
        Task<PagedResult<VehicleResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Tạo 1 thông tin xe máy mới
        /// </summary>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>   
        Task<VehicleResponse> CreateAsync(VehicleCreateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Cập nhật thông tin xe máy
        /// </summary>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>   
        Task<VehicleResponse?> UpdateAsync(int id, VehicleUpdateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Xóa 1 thông tin xe máy đã tồn tại
        /// </summary>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>   
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
