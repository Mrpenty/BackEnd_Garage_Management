using Garage_Management.Application.DTOs.Services;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services
{
    public interface IServiceService
    {
        Task<ServiceResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResult<ServiceResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<ServiceResponse> CreateAsync(ServiceCreateRequest request, CancellationToken ct = default);
        Task<ServiceResponse?> UpdateAsync(int id, ServiceUpdateRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
