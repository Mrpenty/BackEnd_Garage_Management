using Garage_Management.Application.DTOs.RepairEstimateSpareParts;

namespace Garage_Management.Application.Interfaces.Services
{
    public interface IRepairEstimateSparePartService
    {
        Task<RepairEstimateSparePartResponse> CreateAsync(RepairEstimateSparePartCreateRequest request, CancellationToken ct = default);
        Task<RepairEstimateSparePartResponse?> UpdateStatusAsync(int repairEstimateId, int sparePartId, RepairEstimateSparePartStatusUpdateRequest request, CancellationToken ct = default);
    }
}
