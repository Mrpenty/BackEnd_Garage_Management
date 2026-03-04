using Garage_Management.Application.DTOs.WarrantyServices;
using Garage_Management.Application.Interfaces.Repositories.Warranties;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Warranties;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.Warranties
{
    public class WarrantyServiceService : IWarrantyServiceService
    {
        private readonly IWarrantyServiceRepository _repo;

        public WarrantyServiceService(IWarrantyServiceRepository repo)
        {
            _repo = repo;
        }

        public async Task<WarrantyServiceResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<PagedResult<WarrantyServiceResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(page, pageSize, ct);
            return new PagedResult<WarrantyServiceResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = paged.PageData.Select(Map).ToList()
            };
        }

        public async Task<WarrantyServiceResponse> CreateAsync(WarrantyServiceCreateRequest request, CancellationToken ct = default)
        {
            var entity = new WarrantyService
            {
                ServiceId = request.ServiceId,
                ServiceWarrantyPolicyId = request.ServiceWarrantyPolicyId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Description = request.Description
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<WarrantyServiceResponse?> UpdateAsync(int id, WarrantyServiceUpdateRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (request.ServiceId.HasValue) entity.ServiceId = request.ServiceId.Value;
            if (request.ServiceWarrantyPolicyId.HasValue) entity.ServiceWarrantyPolicyId = request.ServiceWarrantyPolicyId.Value;
            if (request.StartDate.HasValue) entity.StartDate = request.StartDate.Value;
            if (request.EndDate.HasValue) entity.EndDate = request.EndDate.Value;
            if (request.Description != null) entity.Description = request.Description;

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

        private static WarrantyServiceResponse Map(WarrantyService entity)
        {
            return new WarrantyServiceResponse
            {
                WarrantyServiceId = entity.WarrantyServiceId,
                ServiceId = entity.ServiceId,
                ServiceWarrantyPolicyId = entity.ServiceWarrantyPolicyId,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Description = entity.Description
            };
        }
    }
}
