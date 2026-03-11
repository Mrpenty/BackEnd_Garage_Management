using Garage_Management.Application.DTOs.ServiceTasks;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services
{
    /*
     * Author: KhanhDV
     * Created Date: 26-02-2026
    */
    public interface IServiceTaskService
    {
        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Lấy chi tiết công việc của dịch vụ
        /// </summary>
        Task<ServiceTaskResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Lấy danh sách công việc theo dịch vụ
        /// </summary>
        Task<List<ServiceTaskResponse>> GetByServiceIdAsync(int serviceId, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Lấy danh sách công việc có phân trang
        /// </summary>
        Task<PagedResult<ServiceTaskResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Tạo công việc cho dịch vụ
        /// </summary>
        Task<ServiceTaskResponse> CreateAsync(ServiceTaskCreateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Cập nhật công việc của dịch vụ
        /// </summary>
        Task<ServiceTaskResponse?> UpdateAsync(int id, ServiceTaskUpdateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Xóa công việc của dịch vụ
        /// </summary>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
