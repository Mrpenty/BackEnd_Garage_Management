using Garage_Management.Application.DTOs.ServiceTasks;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services
{
    public interface IServiceTaskService
    {
        Task<ServiceTaskResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<ServiceTaskResponse>> GetByServiceIdAsync(int serviceId, CancellationToken ct = default);
        Task<PagedResult<ServiceTaskResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<ServiceTaskResponse> CreateAsync(ServiceTaskCreateRequest request, CancellationToken ct = default);
        Task<ServiceTaskResponse?> UpdateAsync(int id, ServiceTaskUpdateRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
