using Garage_Management.Application.DTOs.JobCardServices;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Services.JobCard;
using Garage_Management.Application.Repositories.JobCards;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JobCardServiceEntity = Garage_Management.Base.Entities.JobCards.JobCardService;

namespace Garage_Management.Application.Services.JobCards
{
    public class JobCardServiceService : IJobCardServiceService
    {
        private readonly IJobCardServiceRepository _repo;
        private readonly IJobCardServiceTaskRepository _jobCardServiceTaskRepository;
        private readonly IJobCardRepository _jobCardRepository;
        private readonly IServiceRepository _serviceRepository;

        public JobCardServiceService(IJobCardServiceRepository repo, IJobCardServiceTaskRepository jobCardServiceTaskRepository, IJobCardRepository jobCardRepository, IServiceRepository serviceRepository)
        {
            _repo = repo;
            _jobCardServiceTaskRepository = jobCardServiceTaskRepository;
            _jobCardRepository = jobCardRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<JobCardServiceResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<PagedResult<JobCardServiceResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(page, pageSize, ct);
            return new PagedResult<JobCardServiceResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = paged.PageData.Select(Map).ToList()
            };
        }

        public async Task<ApiResponse<JobCardServiceResponse>> CreateAsync(
    JobCardServiceCreateRequest request,
    CancellationToken ct = default)
        {
            try
            {
               
                // 1. Check JobCard
                var jobCard = await _jobCardRepository.GetByIdAsync(request.JobCardId);
                if (jobCard == null)
                    return ApiResponse<JobCardServiceResponse>.ErrorResponse($"Không tìm thấy JobCardId = {request.JobCardId}");

                // 2. Check Service + Tasks
                var service = await _serviceRepository.GetByIdWithTasksAsync(request.ServiceId, ct);
                if (service == null)
                    return ApiResponse<JobCardServiceResponse>.ErrorResponse($"Không tìm thấy ServiceId = {request.ServiceId}");

                if (service.ServiceTasks == null)
                    return ApiResponse<JobCardServiceResponse>.ErrorResponse("ServiceTasks bị thiếu ");

                // 3. Create JobCardService
                var entity = new JobCardServiceEntity
                {
                    JobCardId = request.JobCardId,
                    ServiceId = request.ServiceId,
                    Description = request.Description ?? service.Description,
                    Price = request.Price > 0 ? request.Price : (service.BasePrice ?? 0),
                    Status = request.Status,
                    SourceInspectionItemId = request.SourceInspectionItemId,
                    CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
                };

                _repo.Add(entity);
                await _repo.SaveAsync(ct);

                if (entity.JobCardServiceId <= 0)
                    return ApiResponse<JobCardServiceResponse>.ErrorResponse("Không tạo được JobCardService");

                // 4. Create Tasks
                var tasksToAdd = service.ServiceTasks
                    .OrderBy(x => x.TaskOrder)
                    .Select(st => new JobCardServiceTask
                    {
                        JobCardId = request.JobCardId,
                        JobCardServiceId = entity.JobCardServiceId,
                        ServiceTaskId = st.ServiceTaskId,
                        TaskOrder = st.TaskOrder,
                        Status = ServiceStatus.Pending,
                        IsOptional = false,

                       
                    })
                    .ToList();

                if (!tasksToAdd.Any())
                    return ApiResponse<JobCardServiceResponse>.ErrorResponse("Không có task nào để thêm");

                _jobCardServiceTaskRepository.AddRange(tasksToAdd);
                await _jobCardServiceTaskRepository.SaveAsync(ct);

                // 5. Verify insert
                var createdEntity = await _repo.GetByIdWithTasksAsync(entity.JobCardServiceId, ct);
                if (createdEntity == null)
                    return ApiResponse<JobCardServiceResponse>.ErrorResponse("Tạo JobCardService nhưng không load lại được");

                

                return ApiResponse<JobCardServiceResponse>
                    .SuccessResponse(Map(createdEntity), "Gắn dịch vụ thành công");
            }
            catch (Exception ex)
            {
                return ApiResponse<JobCardServiceResponse>
                    .ErrorResponse($"Lỗi hệ thống: {ex.Message}");
            }
        }

        public async Task<JobCardServiceResponse?> UpdateAsync(int id, JobCardServiceUpdateRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (request.JobCardId.HasValue) entity.JobCardId = request.JobCardId.Value;
            if (request.ServiceId.HasValue) entity.ServiceId = request.ServiceId.Value;
            if (request.Description != null) entity.Description = request.Description;
            if (request.Price.HasValue) entity.Price = request.Price.Value;
            if (request.Status.HasValue) entity.Status = request.Status.Value;
            if (request.SourceInspectionItemId.HasValue) entity.SourceInspectionItemId = request.SourceInspectionItemId.Value;
            entity.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<JobCardServiceResponse?> UpdateStatusByServiceIdAsync(int serviceId, int? jobCardId, JobCardServiceStatusUpdateRequest request, CancellationToken ct = default)
        {
            if (serviceId <= 0)
                throw new InvalidOperationException("ServiceId không hợp lệ");

            var query = _repo.GetAll().Where(x => x.ServiceId == serviceId);
            if (jobCardId.HasValue)
                query = query.Where(x => x.JobCardId == jobCardId.Value);

            var matches = await query.ToListAsync(ct);
            if (matches.Count == 0) return null;

            if (!jobCardId.HasValue && matches.Count > 1)
                throw new InvalidOperationException("Có nhiều JobCardService cùng ServiceId, vui lòng truyền thêm jobCardId");

            var entity = matches[0];
            entity.Status = request.Status;
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

        private static JobCardServiceResponse Map(JobCardServiceEntity entity)
        {
            return new JobCardServiceResponse
            {
                JobCardServiceId = entity.JobCardServiceId,
                JobCardId = entity.JobCardId,
                ServiceId = entity.ServiceId,
                Description = entity.Description,
                Price = entity.Price,
                Status = entity.Status,
                SourceInspectionItemId = entity.SourceInspectionItemId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
