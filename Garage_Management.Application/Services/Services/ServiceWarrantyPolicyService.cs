using Garage_Management.Application.DTOs.ServiceWarrantyPolicies;
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
    public class ServiceWarrantyPolicyService : IServiceWarrantyPolicyService
    {
        private readonly IServiceWarrantyPolicyRepository _repo;

        public ServiceWarrantyPolicyService(IServiceWarrantyPolicyRepository repo)
        {
            _repo = repo;
        }

        public async Task<ServiceWarrantyPolicyResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<PagedResult<ServiceWarrantyPolicyResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(page, pageSize, ct);
            return new PagedResult<ServiceWarrantyPolicyResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = paged.PageData.Select(Map).ToList()
            };
        }

        public async Task<ServiceWarrantyPolicyResponse> CreateAsync(ServiceWarrantyPolicyCreateRequest request, CancellationToken ct = default)
        {
            var entity = new ServiceWarrantyPolicy
            {
                PolicyName = request.PolicyName,
                DurationMonths = request.DurationMonths,
                MileageLimit = request.MileageLimit,
                TermsAndConditions = request.TermsAndConditions,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<ServiceWarrantyPolicyResponse?> UpdateAsync(int id, ServiceWarrantyPolicyUpdateRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (!string.IsNullOrWhiteSpace(request.PolicyName)) entity.PolicyName = request.PolicyName;
            if (request.DurationMonths.HasValue) entity.DurationMonths = request.DurationMonths.Value;
            if (request.MileageLimit.HasValue) entity.MileageLimit = request.MileageLimit.Value;
            if (request.TermsAndConditions != null) entity.TermsAndConditions = request.TermsAndConditions;
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

        private static ServiceWarrantyPolicyResponse Map(ServiceWarrantyPolicy entity)
        {
            return new ServiceWarrantyPolicyResponse
            {
                PolicyId = entity.PolicyId,
                PolicyName = entity.PolicyName,
                DurationMonths = entity.DurationMonths,
                MileageLimit = entity.MileageLimit,
                TermsAndConditions = entity.TermsAndConditions,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
