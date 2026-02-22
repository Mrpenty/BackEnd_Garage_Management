using Garage_Management.Application.DTOs.ServiceWarrantyPolicies;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services
{
    public interface IServiceWarrantyPolicyService
    {
        Task<ServiceWarrantyPolicyResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResult<ServiceWarrantyPolicyResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<ServiceWarrantyPolicyResponse> CreateAsync(ServiceWarrantyPolicyCreateRequest request, CancellationToken ct = default);
        Task<ServiceWarrantyPolicyResponse?> UpdateAsync(int id, ServiceWarrantyPolicyUpdateRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
