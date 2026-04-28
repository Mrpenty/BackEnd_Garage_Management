using Garage_Management.Application.DTOs.RepairEstimates;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.RepairEstimaties;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.RepairEstimaties;

namespace Garage_Management.Application.Services.RepairEstimaties
{
    public class RepairEstimateService : IRepairEstimateService
    {
        private readonly IRepairEstimateRepository _repo;
        private readonly IJobCardRepository _jobCardRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public RepairEstimateService(
            IRepairEstimateRepository repo,
            IJobCardRepository jobCardRepository,
            IServiceRepository serviceRepository,
            IInventoryRepository inventoryRepository)
        {
            _repo = repo;
            _jobCardRepository = jobCardRepository;
            _serviceRepository = serviceRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<List<RepairEstimateResponse>> GetAllAsync(CancellationToken ct = default)
        {
            var data = await _repo.GetAllAsync(ct);
            return data.Select(Map).ToList();
        }

        public async Task<RepairEstimateDetailResponse?> GetByIdAsync(int repairEstimateId, CancellationToken ct = default)
        {
            ValidateRepairEstimateId(repairEstimateId);

            var entity = await _repo.GetByIdAsync(repairEstimateId, ct);
            return entity == null ? null : MapDetail(entity);
        }

        public async Task<List<RepairEstimateDetailResponse>?> GetByJobCardIdAsync(int jobCardId, CancellationToken ct = default)
        {
            ValidateJobCardId(jobCardId);

            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);
            if (jobCard == null)
                return null;

            var entities = await _repo.GetByJobCardIdAsync(jobCardId, ct);
            return entities.Select(MapDetail).ToList();
        }

        public async Task<RepairEstimateDetailResponse> CreateAsync(RepairEstimateCreateRequest request, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            ValidateCreateRequest(request);

            var jobCard = await _jobCardRepository.GetByIdAsync(request.JobCardId);
            if (jobCard == null)
                throw new InvalidOperationException("Không tìm thấy JobCard");

            ValidateDuplicates(request);

            var entity = new RepairEstimate
            {
                JobCardId = request.JobCardId,
                Note = request.Note
            };

            foreach (var item in request.Services)
            {
                if (item.ServiceId <= 0)
                    throw new InvalidOperationException("ServiceId không hợp lệ");

                if (item.Quantity <= 0)
                    throw new InvalidOperationException("Số lượng dịch vụ phải lớn hơn 0");

                var service = await _serviceRepository.GetByIdAsync(item.ServiceId);
                if (service == null)
                    throw new InvalidOperationException($"Không tìm thấy dịch vụ {item.ServiceId}");

                if (!service.IsActive)
                    throw new InvalidOperationException($"Dịch vụ {item.ServiceId} đã ngừng hoạt động");

                if (!service.BasePrice.HasValue)
                    throw new InvalidOperationException($"Dịch vụ {item.ServiceId} chưa có giá cơ bản");

                if (service.BasePrice.Value < 0)
                    throw new InvalidOperationException($"Giá cơ bản của dịch vụ {item.ServiceId} không hợp lệ");

                entity.Services.Add(new Base.Entities.RepairEstimaties.RepairEstimateService
                {
                    ServiceId = item.ServiceId,
                    Service = service,
                    Quantity = item.Quantity,
                    UnitPrice = service.BasePrice.Value,
                    TotalAmount = service.BasePrice.Value * item.Quantity
                });
            }

            foreach (var item in request.SpareParts)
            {
                if (item.SparePartId <= 0)
                    throw new InvalidOperationException("SparePartId không hợp lệ");

                if (item.Quantity <= 0)
                    throw new InvalidOperationException("Số lượng phụ tùng phải lớn hơn 0");

                var inventory = await _inventoryRepository.GetByIdAsync(item.SparePartId);
                if (inventory == null)
                    throw new InvalidOperationException($"Không tìm thấy phụ tùng {item.SparePartId}");

                if (!inventory.IsActive)
                    throw new InvalidOperationException($"Phụ tùng {item.SparePartId} đã ngừng hoạt động");

                if (!inventory.SellingPrice.HasValue)
                    throw new InvalidOperationException($"Phụ tùng {item.SparePartId} chưa có giá bán");

                if (inventory.SellingPrice.Value < 0)
                    throw new InvalidOperationException($"Giá bán của phụ tùng {item.SparePartId} không hợp lệ");

                entity.SpareParts.Add(new RepairEstimateSparePart
                {
                    SparePartId = item.SparePartId,
                    Inventory = inventory,
                    Quantity = item.Quantity,
                    UnitPrice = inventory.SellingPrice.Value,
                    TotalAmount = inventory.SellingPrice.Value * item.Quantity
                });
            }

            entity.ServiceTotal = entity.Services.Sum(x => x.TotalAmount);
            entity.SparePartTotal = entity.SpareParts.Sum(x => x.TotalAmount);
            entity.GrandTotal = entity.ServiceTotal + entity.SparePartTotal;

            await _repo.AddAsync(entity, ct);

            return MapDetail(entity);
        }

        public async Task<RepairEstimateDetailResponse?> UpdateStatusAsync(int repairEstimateId, RepairEstimateStatusUpdateRequest request, CancellationToken ct = default)
        {
            ValidateRepairEstimateId(repairEstimateId);
            ArgumentNullException.ThrowIfNull(request);
            ValidateStatus(request.Status);

            var entity = await _repo.GetTrackedByIdAsync(repairEstimateId, ct);
            if (entity == null)
                return null;

            entity.Status = request.Status;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(entity, ct);
            return MapDetail(entity);
        }

        private static RepairEstimateResponse Map(RepairEstimate entity)
        {
            return new RepairEstimateResponse
            {
                RepairEstimateId = entity.RepairEstimateId,
                JobCardId = entity.JobCardId,
                Status = entity.Status,
                ServiceTotal = entity.ServiceTotal,
                SparePartTotal = entity.SparePartTotal,
                GrandTotal = entity.GrandTotal,
                Note = entity.Note,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        private static RepairEstimateDetailResponse MapDetail(RepairEstimate entity)
        {
            return new RepairEstimateDetailResponse
            {
                RepairEstimateId = entity.RepairEstimateId,
                JobCardId = entity.JobCardId,
                Status = entity.Status,
                ServiceTotal = entity.ServiceTotal,
                SparePartTotal = entity.SparePartTotal,
                GrandTotal = entity.GrandTotal,
                Note = entity.Note,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Services = entity.Services.Select(x => new RepairEstimateDetailServiceItemResponse
                {
                    ServiceId = x.ServiceId,
                    ServiceName = x.Service?.ServiceName ?? string.Empty,
                    Status = x.Status,
                    UnitPrice = x.UnitPrice,
                    Quantity = x.Quantity,
                    TotalAmount = x.TotalAmount
                }).ToList(),
                SpareParts = entity.SpareParts.Select(x => new RepairEstimateDetailSparePartItemResponse
                {
                    SparePartId = x.SparePartId,
                    SparePartName = x.Inventory?.PartName ?? string.Empty,
                    Status = x.Status,
                    UnitPrice = x.UnitPrice,
                    Quantity = x.Quantity,
                    TotalAmount = x.TotalAmount
                }).ToList()
            };
        }

        private static void ValidateStatus(RepairEstimateApprovalStatus status)
        {
            if (!Enum.IsDefined(typeof(RepairEstimateApprovalStatus), status))
                throw new InvalidOperationException("Trạng thái báo giá sửa chữa không hợp lệ");
        }

        private static void ValidateRepairEstimateId(int repairEstimateId)
        {
            if (repairEstimateId <= 0)
                throw new InvalidOperationException("RepairEstimateId phải lớn hơn 0");
        }

        private static void ValidateJobCardId(int jobCardId)
        {
            if (jobCardId <= 0)
                throw new InvalidOperationException("JobCardId phải lớn hơn 0");
        }

        private static void ValidateCreateRequest(RepairEstimateCreateRequest request)
        {
            ValidateJobCardId(request.JobCardId);

            if (request.Note != null && string.IsNullOrWhiteSpace(request.Note))
                throw new InvalidOperationException("Ghi chú không được chỉ chứa khoảng trắng");

            if (request.Note?.Length > 1000)
                throw new InvalidOperationException("Ghi chú không được vượt quá 1000 ký tự");

            if (request.Services == null)
                throw new InvalidOperationException("Danh sách dịch vụ là bắt buộc");

            if (request.SpareParts == null)
                throw new InvalidOperationException("Danh sách phụ tùng là bắt buộc");

            if (request.Services.Count == 0 && request.SpareParts.Count == 0)
                throw new InvalidOperationException("Báo giá sửa chữa phải có ít nhất một dịch vụ hoặc một phụ tùng");

            foreach (var item in request.Services)
            {
                if (item == null)
                    throw new InvalidOperationException("Mục dịch vụ không hợp lệ");

                if (item.ServiceId <= 0)
                    throw new InvalidOperationException("ServiceId phải lớn hơn 0");

                if (item.Quantity <= 0)
                    throw new InvalidOperationException($"Số lượng dịch vụ của dịch vụ {item.ServiceId} phải lớn hơn 0");
            }

            foreach (var item in request.SpareParts)
            {
                if (item == null)
                    throw new InvalidOperationException("Mục phụ tùng không hợp lệ");

                if (item.SparePartId <= 0)
                    throw new InvalidOperationException("SparePartId phải lớn hơn 0");

                if (item.Quantity <= 0)
                    throw new InvalidOperationException($"Số lượng phụ tùng của phụ tùng {item.SparePartId} phải lớn hơn 0");
            }
        }

        private static void ValidateDuplicates(RepairEstimateCreateRequest request)
        {
            var duplicatedServiceId = request.Services
                .GroupBy(x => x.ServiceId)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .FirstOrDefault();

            if (duplicatedServiceId > 0)
                throw new InvalidOperationException($"Dịch vụ {duplicatedServiceId} bị trùng trong yêu cầu");

            var duplicatedSparePartId = request.SpareParts
                .GroupBy(x => x.SparePartId)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .FirstOrDefault();

            if (duplicatedSparePartId > 0)
                throw new InvalidOperationException($"Phụ tùng {duplicatedSparePartId} bị trùng trong yêu cầu");
        }
    }
}
