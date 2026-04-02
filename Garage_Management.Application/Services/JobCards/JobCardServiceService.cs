using Garage_Management.Application.DTOs.JobCardServices;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.JobCards;
using JobCardServiceEntity = Garage_Management.Base.Entities.JobCards.JobCardService;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Garage_Management.Application.Interfaces.Services.JobCard;
using Microsoft.EntityFrameworkCore;

namespace Garage_Management.Application.Services.JobCards
{
    public class JobCardServiceService : IJobCardServiceService
    {
        private readonly IJobCardServiceRepository _repo;

        public JobCardServiceService(IJobCardServiceRepository repo)
        {
            _repo = repo;
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

        public async Task<JobCardServiceResponse> CreateAsync(JobCardServiceCreateRequest request, CancellationToken ct = default)
        {
            var entity = new JobCardServiceEntity
            {
                JobCardId = request.JobCardId,
                ServiceId = request.ServiceId,
                Description = request.Description,
                Price = request.Price,
                Status = request.Status,
                SourceInspectionItemId = request.SourceInspectionItemId,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
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
            entity.UpdatedAt = DateTime.UtcNow;

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
