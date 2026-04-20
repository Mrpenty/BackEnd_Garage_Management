using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Branches;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories.Branches
{
    public interface IBranchRepository : IBaseRepository<Branch>
    {
        Task<PagedResult<Branch>> GetPagedAsync(ParamQuery query, int? scopedBranchId, CancellationToken ct = default);
        Task<Branch?> GetDetailByIdAsync(int id, CancellationToken ct = default);
        Task<bool> CodeExistsAsync(string branchCode, int? excludeId = null, CancellationToken ct = default);
        Task<int> CountEmployeesAsync(int branchId, CancellationToken ct = default);
        Task<int> CountActiveJobCardsAsync(int branchId, CancellationToken ct = default);
        Task<bool> HasDependenciesAsync(int branchId, CancellationToken ct = default);
    }
}
