using Garage_Management.Application.DTOs.ServiceTasks;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Application.Interfaces.Services.Auth;
using Garage_Management.Application.Services.Auth;
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
        private readonly IServiceRepository _serviceRepo;
        private readonly ICurrentUserService _currentUser;

        public ServiceTaskService(
            IServiceTaskRepository repo,
            IServiceRepository serviceRepo,
            ICurrentUserService currentUser)
        {
            _repo = repo;
            _serviceRepo = serviceRepo;
            _currentUser = currentUser;
        }

        public ServiceTaskService(IServiceTaskRepository repo, IServiceRepository serviceRepo)
        {
            _repo = repo;
            _serviceRepo = serviceRepo;
            _currentUser = new NullCurrentUserService();
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
            if (!(_currentUser.IsInRole("Supervisor") || _currentUser.IsInRole("Accountant")))
                throw new UnauthorizedAccessException("Chỉ Supervisor hoặc Accountant được tạo công việc dịch vụ");

            if (request.ServiceId <= 0)
                throw new InvalidOperationException("ServiceId không hợp lệ");

            var service = await _serviceRepo.GetByIdAsync(request.ServiceId);
            if (service == null)
                throw new InvalidOperationException("ServiceId không tồn tại");

            if (string.IsNullOrWhiteSpace(request.TaskName))
                throw new InvalidOperationException("TaskName không hợp lệ");

            if (request.TaskOrder <= 0)
                throw new InvalidOperationException("TaskOrder phải lớn hơn 0");

            if (request.EstimateMinute < 0)
                throw new InvalidOperationException("EstimateMinute không hợp lệ");

            if (await _repo.HasExistAsync(request.ServiceId, request.TaskOrder, null, ct))
                throw new InvalidOperationException("TaskOrder đã tồn tại trong service này");

            var entity = new ServiceTask
            {
                ServiceId = request.ServiceId,
                TaskName = request.TaskName.Trim(),
                TaskOrder = request.TaskOrder,
                EstimateMinute = request.EstimateMinute,
                Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUser.GetCurrentUserId()
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<ServiceTaskResponse?> UpdateAsync(int id, ServiceTaskUpdateRequest request, CancellationToken ct = default)
        {
            if (!(_currentUser.IsInRole("Supervisor") || _currentUser.IsInRole("Accountant")))
                throw new UnauthorizedAccessException("Chỉ Supervisor hoặc Accountant được cập nhật công việc dịch vụ");

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            var serviceId = request.ServiceId ?? entity.ServiceId;
            var taskOrder = request.TaskOrder ?? entity.TaskOrder;

            if (request.ServiceId.HasValue)
            {
                if (request.ServiceId.Value <= 0)
                    throw new InvalidOperationException("ServiceId không hợp lệ");
                if (request.ServiceId.Value != entity.ServiceId)
                {
                    var service = await _serviceRepo.GetByIdAsync(request.ServiceId.Value);
                    if (service == null)
                        throw new InvalidOperationException("ServiceId không tồn tại");
                }
            }

            if (request.TaskOrder.HasValue && request.TaskOrder.Value <= 0)
                throw new InvalidOperationException("TaskOrder phải lớn hơn 0");

            if (request.EstimateMinute.HasValue && request.EstimateMinute.Value < 0)
                throw new InvalidOperationException("EstimateMinute không hợp lệ");

            if (request.TaskName != null && string.IsNullOrWhiteSpace(request.TaskName))
                throw new InvalidOperationException("TaskName không hợp lệ");

            if (await _repo.HasExistAsync(serviceId, taskOrder, id, ct))
                throw new InvalidOperationException("TaskOrder đã tồn tại trong service này");

            if (request.ServiceId.HasValue) entity.ServiceId = request.ServiceId.Value;
            if (request.TaskName != null) entity.TaskName = request.TaskName.Trim();
            if (request.TaskOrder.HasValue) entity.TaskOrder = request.TaskOrder.Value;
            if (request.EstimateMinute.HasValue) entity.EstimateMinute = request.EstimateMinute.Value;
            if (request.Note != null) entity.Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim();
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _currentUser.GetCurrentUserId();

            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
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
