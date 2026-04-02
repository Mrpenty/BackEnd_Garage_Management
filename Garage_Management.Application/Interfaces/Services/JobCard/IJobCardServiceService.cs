using Garage_Management.Application.DTOs.JobCardServices;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services.JobCard
{
    public interface IJobCardServiceService
    {
        Task<JobCardServiceResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResult<JobCardServiceResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<JobCardServiceResponse> CreateAsync(JobCardServiceCreateRequest request, CancellationToken ct = default);
        Task<JobCardServiceResponse?> UpdateAsync(int id, JobCardServiceUpdateRequest request, CancellationToken ct = default);
        Task<JobCardServiceResponse?> UpdateStatusByServiceIdAsync(int serviceId, int? jobCardId, JobCardServiceStatusUpdateRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
