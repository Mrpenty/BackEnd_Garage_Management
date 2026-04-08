using Garage_Management.Application.DTOs.RepairEstimateServices;
using Garage_Management.Application.Interfaces.Repositories.RepairEstimaties;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;

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
            ValidateRepairEstimateId(repairEstimateId);
            ValidateServiceId(serviceId);

            var entity = await _repo.GetByIdAsync(repairEstimateId, serviceId, ct);
            return entity == null ? null : Map(entity);
        }

        public async Task<PagedResult<RepairEstimateServiceResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            ValidatePaging(page, pageSize);

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
            ArgumentNullException.ThrowIfNull(request);
            ValidateCreateRequest(request);

            var repairEstimate = await _repairEstimateRepository.GetByIdAsync(request.RepairEstimateId, ct);
            if (repairEstimate == null)
                throw new InvalidOperationException("RepairEstimate not found");

            var service = await _serviceRepository.GetByIdAsync(request.ServiceId);
            if (service == null)
                throw new InvalidOperationException("Service not found");

            if (!service.IsActive)
                throw new InvalidOperationException($"Service {request.ServiceId} is inactive");

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
            ValidateRepairEstimateId(repairEstimateId);
            ValidateServiceId(serviceId);
            ArgumentNullException.ThrowIfNull(request);

            var entity = await _repo.GetByIdAsync(repairEstimateId, serviceId, ct);
            if (entity == null)
                return null;

            ValidateUpdateRequest(request, entity.UnitPrice, entity.Quantity);

            if (request.UnitPrice.HasValue)
                entity.UnitPrice = request.UnitPrice.Value;

            if (request.Quantity.HasValue)
                entity.Quantity = request.Quantity.Value;

            var expectedTotalAmount = entity.UnitPrice * entity.Quantity;
            if (request.TotalAmount.HasValue && request.TotalAmount.Value != expectedTotalAmount)
                throw new InvalidOperationException("TotalAmount must equal UnitPrice multiplied by Quantity");

            entity.TotalAmount = request.TotalAmount ?? expectedTotalAmount;

            await _repo.UpdateAsync(entity, ct);
            return Map(entity);
        }

        public async Task<bool> DeleteAsync(int repairEstimateId, int serviceId, CancellationToken ct = default)
        {
            ValidateRepairEstimateId(repairEstimateId);
            ValidateServiceId(serviceId);

            var entity = await _repo.GetByIdAsync(repairEstimateId, serviceId, ct);
            if (entity == null)
                return false;

            await _repo.DeleteAsync(entity, ct);
            return true;
        }

        public async Task<RepairEstimateServiceResponse?> UpdateStatusAsync(int repairEstimateId, int serviceId, RepairEstimateServiceStatusUpdateRequest request, CancellationToken ct = default)
        {
            ValidateRepairEstimateId(repairEstimateId);
            ValidateServiceId(serviceId);
            ArgumentNullException.ThrowIfNull(request);
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

        private static void ValidatePaging(int page, int pageSize)
        {
            if (page <= 0)
                throw new InvalidOperationException("Page must be greater than 0");

            if (pageSize <= 0)
                throw new InvalidOperationException("PageSize must be greater than 0");
        }

        private static void ValidateRepairEstimateId(int repairEstimateId)
        {
            if (repairEstimateId <= 0)
                throw new InvalidOperationException("RepairEstimateId must be greater than 0");
        }

        private static void ValidateServiceId(int serviceId)
        {
            if (serviceId <= 0)
                throw new InvalidOperationException("ServiceId must be greater than 0");
        }

        private static void ValidateCreateRequest(RepairEstimateServiceCreateRequest request)
        {
            ValidateRepairEstimateId(request.RepairEstimateId);
            ValidateServiceId(request.ServiceId);

            if (request.UnitPrice <= 0)
                throw new InvalidOperationException("UnitPrice must be greater than 0");

            if (request.Quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than 0");

            if (request.TotalAmount <= 0)
                throw new InvalidOperationException("TotalAmount must be greater than 0");

            var expectedTotalAmount = request.UnitPrice * request.Quantity;
            if (request.TotalAmount != expectedTotalAmount)
                throw new InvalidOperationException("TotalAmount must equal UnitPrice multiplied by Quantity");
        }

        private static void ValidateUpdateRequest(RepairEstimateServiceUpdateRequest request, decimal currentUnitPrice, int currentQuantity)
        {
            if (request.UnitPrice.HasValue && request.UnitPrice.Value <= 0)
                throw new InvalidOperationException("UnitPrice must be greater than 0");

            if (request.Quantity.HasValue && request.Quantity.Value <= 0)
                throw new InvalidOperationException("Quantity must be greater than 0");

            if (request.TotalAmount.HasValue && request.TotalAmount.Value < 0)
                throw new InvalidOperationException("TotalAmount must not be negative");

            var finalUnitPrice = request.UnitPrice ?? currentUnitPrice;
            var finalQuantity = request.Quantity ?? currentQuantity;
            var expectedTotalAmount = finalUnitPrice * finalQuantity;

            if (request.TotalAmount.HasValue && request.TotalAmount.Value != expectedTotalAmount)
                throw new InvalidOperationException("TotalAmount must equal UnitPrice multiplied by Quantity");
        }
    }
}
