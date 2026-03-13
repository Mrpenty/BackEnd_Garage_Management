using Garage_Management.Application.DTOs.Services;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services
{
    public interface IServiceService
    {
        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy chi tiết 1 dịch vụ
        /// </summary>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param> 
        Task<ServiceResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách dịch vụ có phân trang
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>    
        Task<PagedResult<ServiceResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Tạo 1 lịch đặt mới
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>    
        Task<ServiceResponse> CreateAsync(ServiceCreateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Xóa 1 dịch vụ
        /// </summary>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Ngừng kích hoạt (deactivate) dịch vụ.
        /// </summary>
        Task<ServiceResponse?> DeactivateAsync(int id, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 6-3-2026
        /// <summary>
        /// Lấy danh sách dịch vụ theo loại xe (VehicleType)
        /// </summary>
        Task<List<ServiceResponse>> GetByVehicleTypeAsync(int vehicleTypeId, CancellationToken ct = default);
    }
}
