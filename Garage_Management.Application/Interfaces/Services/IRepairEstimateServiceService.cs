using Garage_Management.Application.DTOs.RepairEstimateServices;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services
{
    /*
     * Author: KhanhDV
     * Created Date: 26-02-2026
    */
    public interface IRepairEstimateServiceService
    {
        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Lấy chi tiết dịch vụ trong báo giá
        /// </summary>
        Task<RepairEstimateServiceResponse?> GetByIdAsync(int repairEstimateId, int serviceId, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Lấy danh sách dịch vụ trong báo giá có phân trang
        /// </summary>
        Task<PagedResult<RepairEstimateServiceResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Thêm dịch vụ vào báo giá
        /// </summary>
        Task<RepairEstimateServiceResponse> CreateAsync(RepairEstimateServiceCreateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Cập nhật dịch vụ trong báo giá
        /// </summary>
        Task<RepairEstimateServiceResponse?> UpdateAsync(int repairEstimateId, int serviceId, RepairEstimateServiceUpdateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Xóa dịch vụ trong báo giá
        /// </summary>
        Task<bool> DeleteAsync(int repairEstimateId, int serviceId, CancellationToken ct = default);
        Task<RepairEstimateServiceResponse?> UpdateStatusAsync(int repairEstimateId, int serviceId, RepairEstimateServiceStatusUpdateRequest request, CancellationToken ct = default);
    }
}
