using Garage_Management.Application.DTOs.ServiceTasks;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.Services
{
    public class ServiceTaskService : IServiceTaskService
    {
        private readonly IServiceTaskRepository _repo;

        public ServiceTaskService(IServiceTaskRepository repo)
        {
            _repo = repo;
        }

        public async Task<ServiceTaskResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<List<ServiceTaskResponse>> GetByServiceIdAsync(int serviceId, CancellationToken ct = default)
        {
            var data = await _repo.GetByServiceIdAsync(serviceId, ct);
            return data.Select(Map).ToList();
        }

        public async Task<PagedResult<ServiceTaskResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(page, pageSize, ct);
            return new PagedResult<ServiceTaskResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = paged.PageData.Select(Map).ToList()
            };
        }

        public async Task<ServiceTaskResponse> CreateAsync(ServiceTaskCreateRequest request, CancellationToken ct = default)
        {
            if (await _repo.HasExistAsync(request.ServiceId, request.TaskOrder, null, ct))
                throw new InvalidOperationException("TaskOrder đã tồn tại trong service này");
            var entity = new ServiceTask
            {
                ServiceId = request.ServiceId,
                TaskName = request.TaskName,
                TaskOrder = request.TaskOrder,
                EstimateMinute = request.EstimateMinute,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow
            };
            
            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<ServiceTaskResponse?> UpdateAsync(int id, ServiceTaskUpdateRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            var serviceId = request.ServiceId ?? entity.ServiceId;
            var taskOrder = request.TaskOrder ?? entity.TaskOrder;

            if (await _repo.HasExistAsync(serviceId, taskOrder, id, ct))
                throw new InvalidOperationException("TaskOrder đã tồn tại trong service này");

            if (request.ServiceId.HasValue) entity.ServiceId = request.ServiceId.Value;
            if (!string.IsNullOrWhiteSpace(request.TaskName)) entity.TaskName = request.TaskName;
            if (request.TaskOrder.HasValue) entity.TaskOrder = request.TaskOrder.Value;
            if (request.EstimateMinute.HasValue) entity.EstimateMinute = request.EstimateMinute.Value;
            if (request.Note != null) entity.Note = request.Note;
            entity.UpdatedAt = DateTime.UtcNow;

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

        private static ServiceTaskResponse Map(ServiceTask entity)
        {
            return new ServiceTaskResponse
            {
                ServiceTaskId = entity.ServiceTaskId,
                ServiceId = entity.ServiceId,
                TaskName = entity.TaskName,
                TaskOrder = entity.TaskOrder,
                EstimateMinute = entity.EstimateMinute,
                Note = entity.Note,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
