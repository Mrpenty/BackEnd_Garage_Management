using Garage_Management.Application.DTOs.RepairEstimateServices;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services
{
    public interface IRepairEstimateServiceService
    {
        Task<RepairEstimateServiceResponse?> GetByIdAsync(int repairEstimateId, int serviceId, CancellationToken ct = default);
        Task<PagedResult<RepairEstimateServiceResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<RepairEstimateServiceResponse> CreateAsync(RepairEstimateServiceCreateRequest request, CancellationToken ct = default);
        Task<RepairEstimateServiceResponse?> UpdateAsync(int repairEstimateId, int serviceId, RepairEstimateServiceUpdateRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(int repairEstimateId, int serviceId, CancellationToken ct = default);
    }
}
