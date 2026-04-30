using Garage_Management.Application.DTOs.Vehicles.VehicleModel;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services.Vehiclies
{
    /*
     * Author: KhanhDV
     * Created Date: 13-02-2026
    */
    public interface IVehicleModelService
    {
        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy Chi tiết 1 model xe máy
        /// </summary>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>    
        Task<VehicleModelResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách model xe máy có phân trạng
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>    
        Task<PagedResult<VehicleModelResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Tạo 1 model xe máy mới
        /// </summary>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>    
        Task<VehicleModelResponse> CreateAsync(VehicleModelCreateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Cập nhật 1 model xe máy đã tồn tại
        /// </summary>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>    
        Task<VehicleModelResponse?> UpdateAsync(int id, VehicleModelUpdate request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Toggle trạng thái IsActive của 1 model xe máy (active ↔ deactive)
        /// </summary>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>
        Task<bool> DeActiveAsync(int id, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Xóa cứng 1 model xe máy. Chỉ cho phép khi chưa có vehicle liên kết.
        /// </summary>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
