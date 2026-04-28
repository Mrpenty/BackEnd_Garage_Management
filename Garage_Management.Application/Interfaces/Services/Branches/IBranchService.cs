using Garage_Management.Application.DTOs.Branches;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services.Branches
{
    public interface IBranchService
    {
        Task<PagedResult<BranchResponse>> GetPagedAsync(ParamQuery query, CancellationToken ct = default);
        Task<BranchResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<BranchResponse> CreateAsync(BranchCreateRequest request, CancellationToken ct = default);
        Task<BranchResponse?> UpdateAsync(int id, BranchUpdateRequest request, CancellationToken ct = default);
        Task<BranchResponse?> UpdateStatusAsync(int id, bool isActive, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
