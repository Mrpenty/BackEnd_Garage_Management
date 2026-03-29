using Garage_Management.Base.Entities.RepairEstimaties;

namespace Garage_Management.Application.Interfaces.Repositories.RepairEstimaties
{
    public interface IRepairEstimateRepository
    {
        Task<List<RepairEstimate>> GetAllAsync(CancellationToken ct = default);
        Task<RepairEstimate?> GetByIdAsync(int repairEstimateId, CancellationToken ct = default);
        Task<List<RepairEstimate>> GetByJobCardIdAsync(int jobCardId, CancellationToken ct = default);
        Task AddAsync(RepairEstimate entity, CancellationToken ct = default);
    }
}
