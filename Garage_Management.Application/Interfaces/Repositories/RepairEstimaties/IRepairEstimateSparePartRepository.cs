using Garage_Management.Base.Entities.RepairEstimaties;

namespace Garage_Management.Application.Interfaces.Repositories.RepairEstimaties
{
    public interface IRepairEstimateSparePartRepository
    {
        Task<bool> RepairEstimateExistsAsync(int repairEstimateId, CancellationToken ct = default);
        Task<RepairEstimateSparePart?> GetByIdAsync(int repairEstimateId, int sparePartId, CancellationToken ct = default);
        Task<RepairEstimateSparePart?> GetTrackedByIdAsync(int repairEstimateId, int sparePartId, CancellationToken ct = default);
        Task AddAsync(RepairEstimateSparePart entity, CancellationToken ct = default);
        Task UpdateAsync(RepairEstimateSparePart entity, CancellationToken ct = default);
    }
}
