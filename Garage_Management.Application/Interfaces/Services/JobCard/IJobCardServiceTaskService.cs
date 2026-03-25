using Garage_Management.Application.DTOs.JobCardServiceTasks;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services.JobCard
{
    public interface IJobCardServiceTaskService
    {
        Task<JobCardServiceTaskResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResult<JobCardServiceTaskResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<JobCardServiceTaskResponse> CreateAsync(JobCardServiceTaskCreateRequest request, CancellationToken ct = default);
        Task<JobCardServiceTaskResponse?> UpdateAsync(int id, JobCardServiceTaskUpdateRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
