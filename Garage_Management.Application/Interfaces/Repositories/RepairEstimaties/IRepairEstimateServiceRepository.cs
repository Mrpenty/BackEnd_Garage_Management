using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.RepairEstimaties;

namespace Garage_Management.Application.Interfaces.Repositories.RepairEstimaties
{
    public interface IRepairEstimateServiceRepository
    {
        /// Author: KhanhDV
        /// Created Date: 20-2-2026
        /// <summary>
        /// Lấy danh sách công việc dự kiến của từng dịch vụ được phân trang
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>
        Task<PagedResult<RepairEstimateService>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<RepairEstimateService?> GetByIdAsync(int repairEstimateId, int serviceId, CancellationToken ct = default);
        Task<RepairEstimateService?> GetTrackedByIdAsync(int repairEstimateId, int serviceId, CancellationToken ct = default);
        Task AddAsync(RepairEstimateService entity, CancellationToken ct = default);
        Task UpdateAsync(RepairEstimateService entity, CancellationToken ct = default);
        Task DeleteAsync(RepairEstimateService entity, CancellationToken ct = default);
    }
}
