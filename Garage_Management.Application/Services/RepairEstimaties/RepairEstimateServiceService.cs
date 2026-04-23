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
                throw new InvalidOperationException("Khong tim thay bao gia sua chua");

            var service = await _serviceRepository.GetByIdAsync(request.ServiceId);
            if (service == null)
                throw new InvalidOperationException("Khong tim thay dich vu");

            if (!service.IsActive)
                throw new InvalidOperationException($"Dich vu {request.ServiceId} da ngung hoat dong");

            if (!service.BasePrice.HasValue)
                throw new InvalidOperationException($"Dich vu {request.ServiceId} chua co gia co ban");

            if (service.BasePrice.Value < 0)
                throw new InvalidOperationException($"Gia co ban cua dich vu {request.ServiceId} khong hop le");

            var existed = await _repo.GetByIdAsync(request.RepairEstimateId, request.ServiceId, ct);
            if (existed != null)
                throw new InvalidOperationException("Dich vu bao gia sua chua nay da ton tai");

            var unitPrice = service.BasePrice.Value;

            var entity = new Garage_Management.Base.Entities.RepairEstimaties.RepairEstimateService
            {
                RepairEstimateId = request.RepairEstimateId,
                ServiceId = request.ServiceId,
                Status = RepairEstimateApprovalStatus.WaitingApproval,
                UnitPrice = unitPrice,
                Quantity = request.Quantity,
                TotalAmount = unitPrice * request.Quantity
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

            ValidateUpdateRequest(request);

            if (request.Quantity.HasValue)
                entity.Quantity = request.Quantity.Value;

            entity.TotalAmount = entity.UnitPrice * entity.Quantity;

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
                throw new InvalidOperationException("Trang thai dich vu bao gia sua chua khong hop le");
        }

        private static void ValidatePaging(int page, int pageSize)
        {
            if (page <= 0)
                throw new InvalidOperationException("Trang phai lon hon 0");

            if (pageSize <= 0)
                throw new InvalidOperationException("Kich thuoc trang phai lon hon 0");
        }

        private static void ValidateRepairEstimateId(int repairEstimateId)
        {
            if (repairEstimateId <= 0)
                throw new InvalidOperationException("RepairEstimateId phai lon hon 0");
        }

        private static void ValidateServiceId(int serviceId)
        {
            if (serviceId <= 0)
                throw new InvalidOperationException("ServiceId phai lon hon 0");
        }

        private static void ValidateCreateRequest(RepairEstimateServiceCreateRequest request)
        {
            ValidateRepairEstimateId(request.RepairEstimateId);
            ValidateServiceId(request.ServiceId);

            if (request.Quantity <= 0)
                throw new InvalidOperationException("So luong phai lon hon 0");
        }

        private static void ValidateUpdateRequest(RepairEstimateServiceUpdateRequest request)
        {
            if (request.Quantity.HasValue && request.Quantity.Value <= 0)
                throw new InvalidOperationException("So luong phai lon hon 0");
        }
    }
}
