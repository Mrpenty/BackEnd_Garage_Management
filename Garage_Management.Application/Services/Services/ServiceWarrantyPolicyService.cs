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
            var name = request.PolicyName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("PolicyName không được để trống");
            if (request.DurationMonths.HasValue && request.DurationMonths.Value < 0)
                throw new InvalidOperationException("DurationMonths không hợp lệ");
            if (request.MileageLimit.HasValue && request.MileageLimit.Value < 0)
                throw new InvalidOperationException("MileageLimit không hợp lệ");
            if (await _repo.ExistsByNameAsync(name, null, ct))
                throw new InvalidOperationException("PolicyName đã tồn tại");

            var entity = new ServiceWarrantyPolicy
            {
                PolicyName = name,
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

            if (!string.IsNullOrWhiteSpace(request.PolicyName))
            {
                var inputName = request.PolicyName.Trim();
                if (await _repo.ExistsByNameAsync(inputName, id, ct))
                    throw new InvalidOperationException("PolicyName đã tồn tại");

                entity.PolicyName = inputName;
            }

            if (request.DurationMonths.HasValue && request.DurationMonths.Value < 0)
                throw new InvalidOperationException("DurationMonths không hợp lệ");
            if (request.MileageLimit.HasValue && request.MileageLimit.Value < 0)
                throw new InvalidOperationException("MileageLimit không hợp lệ");

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
            if (await _repo.HasDependenciesAsync(id, ct))
                throw new InvalidOperationException("Không thể xóa policy vì đã có dữ liệu bảo hành liên quan");

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
