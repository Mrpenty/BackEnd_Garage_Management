using Garage_Management.Application.DTOs.JobCardServiceTasks;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.JobCards;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.JobCards
{
    public class JobCardServiceTaskService : IJobCardServiceTaskService
    {
        private readonly IJobCardServiceTaskRepository _repo;

        public JobCardServiceTaskService(IJobCardServiceTaskRepository repo)
        {
            _repo = repo;
        }

        public async Task<JobCardServiceTaskResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<PagedResult<JobCardServiceTaskResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(page, pageSize, ct);
            return new PagedResult<JobCardServiceTaskResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = paged.PageData.Select(Map).ToList()
            };
        }

        public async Task<JobCardServiceTaskResponse> CreateAsync(JobCardServiceTaskCreateRequest request, CancellationToken ct = default)
        {
            var entity = new JobCardServiceTask
            {
                JobCardId = request.JobCardId,
                ServiceTaskId = request.ServiceTaskId,
                TaskOrder = request.TaskOrder,
                Status = request.Status,
                IsOptional = request.IsOptional,
                StartedAt = request.StartedAt,
                CompletedAt = request.CompletedAt,
                Note = request.Note,
                PerformedById = request.PerformedById
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<JobCardServiceTaskResponse?> UpdateAsync(int id, JobCardServiceTaskUpdateRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (request.JobCardId.HasValue) entity.JobCardId = request.JobCardId.Value;
            if (request.ServiceTaskId.HasValue) entity.ServiceTaskId = request.ServiceTaskId.Value;
            if (request.TaskOrder.HasValue) entity.TaskOrder = request.TaskOrder.Value;
            if (request.Status.HasValue) entity.Status = request.Status.Value;
            if (request.IsOptional.HasValue) entity.IsOptional = request.IsOptional.Value;
            if (request.StartedAt.HasValue) entity.StartedAt = request.StartedAt.Value;
            if (request.CompletedAt.HasValue) entity.CompletedAt = request.CompletedAt.Value;
            if (request.Note != null) entity.Note = request.Note;
            if (request.PerformedById.HasValue) entity.PerformedById = request.PerformedById.Value;

            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            _repo.Delete(entity);
            await _repo.SaveAsync(ct);
            return true;
        }

        private static JobCardServiceTaskResponse Map(JobCardServiceTask entity)
        {
            return new JobCardServiceTaskResponse
            {
                JobCardServiceTaskId = entity.JobCardServiceTaskId,
                JobCardId = entity.JobCardId,
                ServiceTaskId = entity.ServiceTaskId,
                TaskOrder = entity.TaskOrder,
                Status = entity.Status,
                IsOptional = entity.IsOptional,
                StartedAt = entity.StartedAt,
                CompletedAt = entity.CompletedAt,
                Note = entity.Note,
                PerformedById = entity.PerformedById
            };
        }
    }
}
