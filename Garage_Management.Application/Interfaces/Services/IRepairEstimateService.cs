using Garage_Management.Application.DTOs.RepairEstimates;

namespace Garage_Management.Application.Interfaces.Services
{
    public interface IRepairEstimateService
    {
        Task<List<RepairEstimateResponse>> GetAllAsync(CancellationToken ct = default);
        Task<RepairEstimateDetailResponse?> GetByIdAsync(int repairEstimateId, CancellationToken ct = default);
        Task<List<RepairEstimateDetailResponse>?> GetByJobCardIdAsync(int jobCardId, CancellationToken ct = default);
        Task<RepairEstimateDetailResponse> CreateAsync(RepairEstimateCreateRequest request, CancellationToken ct = default);
    }
}
