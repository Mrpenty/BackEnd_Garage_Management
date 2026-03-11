using Garage_Management.Application.DTOs.ServiceWarrantyPolicies;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services
{
    /*
     * Author: KhanhDV
     * Created Date: 26-02-2026
    */
    public interface IServiceWarrantyPolicyService
    {
        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Lấy chi tiết chính sách bảo hành dịch vụ
        /// </summary>
        Task<ServiceWarrantyPolicyResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Lấy danh sách chính sách bảo hành dịch vụ có phân trang
        /// </summary>
        Task<PagedResult<ServiceWarrantyPolicyResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Tạo chính sách bảo hành dịch vụ
        /// </summary>
        Task<ServiceWarrantyPolicyResponse> CreateAsync(ServiceWarrantyPolicyCreateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Cập nhật chính sách bảo hành dịch vụ
        /// </summary>
        Task<ServiceWarrantyPolicyResponse?> UpdateAsync(int id, ServiceWarrantyPolicyUpdateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Xóa chính sách bảo hành dịch vụ
        /// </summary>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
