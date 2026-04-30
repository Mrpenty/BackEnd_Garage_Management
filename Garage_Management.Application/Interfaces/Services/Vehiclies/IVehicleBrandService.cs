using Garage_Management.Application.DTOs.Vehicles.VehicleBrand;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services.Vehiclies
{
    /*
     * Author: KhanhDV
     * Created Date: 13-02-2026
    */
    public interface IVehicleBrandService
    {
        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy chi tiết 1 vehicle brand
        /// </summary>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>    
        Task<VehicleBrandResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách vehicle brand có phân trạng
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>    
        Task<PagedResult<VehicleBrandResponse>> GetPagedAsync(int page, int pageSize, string? keyword = null, CancellationToken ct = default);
        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Tạo 1 vehicle brand mới
        /// </summary>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>    
        Task<VehicleBrandResponse> CreateAsync(VehicleBrandCreateRequest request, CancellationToken ct = default);

        /// <summary>
        /// Cập nhật thông tin vehicle brand. Chỉ cho phép khi chưa có model/vehicle liên kết.
        /// </summary>
        Task<VehicleBrandResponse?> UpdateAsync(int id, VehicleBrandUpdate request, CancellationToken ct = default);

        /// <summary>
        /// Cập nhật trạng thái isActive của vehicle brand.
        /// </summary>
        Task<bool> UpdateStatusAsync(int id, bool isActive, CancellationToken ct = default);

        /// <summary>
        /// Toggle trạng thái isActive (active ↔ deactive).
        /// </summary>
        Task<bool> ToggleStatusAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Xóa cứng vehicle brand. Chỉ cho phép khi chưa có model/vehicle liên kết.
        /// </summary>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Xóa 1 lịch đặt
        /// </summary>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>
        Task<bool> DeActiveAsync(int id, CancellationToken ct = default);
    }
}
