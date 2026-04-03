using Garage_Management.Application.DTOs.RepairEstimateServices;
using Garage_Management.Application.Interfaces.Repositories.RepairEstimaties;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.RepairEstimaties;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.RepairEstimaties
{
    public class RepairEstimateServiceService : IRepairEstimateServiceService
    {
        private readonly IRepairEstimateServiceRepository _repo;
        private readonly IRepairEstimateRepository _repairEstimateRepository;
        private readonly IServiceRepository _serviceRepository;

        public RepairEstimateServiceService(
            IRepairEstimateServiceRepository repo,
            IRepairEstimateRepository repairEstimateRepository,
            IServiceRepository serviceRepository)
        {
            _repo = repo;
            _repairEstimateRepository = repairEstimateRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<RepairEstimateServiceResponse?> GetByIdAsync(int repairEstimateId, int serviceId, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(repairEstimateId, serviceId, ct);
            return entity == null ? null : Map(entity);
        }

        public async Task<PagedResult<RepairEstimateServiceResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(page, pageSize, ct);
            return new PagedResult<RepairEstimateServiceResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = paged.PageData.Select(Map).ToList()
            };
        }

        public async Task<RepairEstimateServiceResponse> CreateAsync(RepairEstimateServiceCreateRequest request, CancellationToken ct = default)
        {
            if (request.RepairEstimateId <= 0)
                throw new InvalidOperationException("RepairEstimateId không hợp lệ");
            if (request.ServiceId <= 0)
                throw new InvalidOperationException("ServiceId không hợp lệ");
            if (request.Quantity <= 0)
                throw new InvalidOperationException("Quantity phải lớn hơn 0");
            if (request.UnitPrice <= 0)
                throw new InvalidOperationException("UnitPrice phải lớn hơn 0");

            var estimate = await _repairEstimateRepository.GetByIdAsync(request.RepairEstimateId, ct);
            if (estimate == null)
                throw new InvalidOperationException("RepairEstimate not found");

            var service = await _serviceRepository.GetByIdAsync(request.ServiceId);
            if (service == null)
                throw new InvalidOperationException("Service not found");

            var existed = await _repo.GetByIdAsync(request.RepairEstimateId, request.ServiceId, ct);
            if (existed != null)
                throw new InvalidOperationException("RepairEstimateService already exists");

            var entity = new Garage_Management.Base.Entities.RepairEstimaties.RepairEstimateService
            {
                RepairEstimateId = request.RepairEstimateId,
                ServiceId = request.ServiceId,
                Status = RepairEstimateApprovalStatus.WaitingApproval,
                UnitPrice = request.UnitPrice,
                Quantity = request.Quantity,
                TotalAmount = request.UnitPrice * request.Quantity
            };

            await _repo.AddAsync(entity, ct);
            return Map(entity);
        }

        public async Task<RepairEstimateServiceResponse?> UpdateAsync(int repairEstimateId, int serviceId, RepairEstimateServiceUpdateRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(repairEstimateId, serviceId, ct);
            if (entity == null) return null;

            if (request.UnitPrice.HasValue)
            {
                if (request.UnitPrice.Value <= 0)
                    throw new InvalidOperationException("UnitPrice phải lớn hơn 0");
                entity.UnitPrice = request.UnitPrice.Value;
            }

            if (request.Quantity.HasValue)
            {
                if (request.Quantity.Value <= 0)
                    throw new InvalidOperationException("Quantity phải lớn hơn 0");
                entity.Quantity = request.Quantity.Value;
            }

            entity.TotalAmount = entity.UnitPrice * entity.Quantity;

            await _repo.UpdateAsync(entity, ct);
            return Map(entity);
        }

        public async Task<bool> DeleteAsync(int repairEstimateId, int serviceId, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(repairEstimateId, serviceId, ct);
            if (entity == null) return false;

            await _repo.DeleteAsync(entity, ct);
            return true;
        }

        public async Task<RepairEstimateServiceResponse?> UpdateStatusAsync(int repairEstimateId, int serviceId, RepairEstimateServiceStatusUpdateRequest request, CancellationToken ct = default)
        {
            ValidateStatus(request.Status);

            var entity = await _repo.GetTrackedByIdAsync(repairEstimateId, serviceId, ct);
            if (entity == null)
                return null;

            entity.Status = request.Status;
            await _repo.UpdateAsync(entity, ct);
            return Map(entity);
        }

        private static RepairEstimateServiceResponse Map(Garage_Management.Base.Entities.RepairEstimaties.RepairEstimateService entity)
        {
            return new RepairEstimateServiceResponse
            {
                RepairEstimateId = entity.RepairEstimateId,
                ServiceId = entity.ServiceId,
                Status = entity.Status,
                UnitPrice = entity.UnitPrice,
                Quantity = entity.Quantity,
                TotalAmount = entity.TotalAmount
            };
        }

        private static void ValidateStatus(RepairEstimateApprovalStatus status)
        {
            if (!Enum.IsDefined(typeof(RepairEstimateApprovalStatus), status))
                throw new InvalidOperationException("Invalid repair estimate service status");
        }
    }
}
