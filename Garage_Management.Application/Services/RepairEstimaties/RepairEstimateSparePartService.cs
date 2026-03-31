using Garage_Management.Application.DTOs.RepairEstimateSpareParts;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.RepairEstimaties;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.RepairEstimaties;

namespace Garage_Management.Application.Services.RepairEstimaties
{
    public class RepairEstimateSparePartService : IRepairEstimateSparePartService
    {
        private readonly IRepairEstimateSparePartRepository _repo;
        private readonly IInventoryRepository _inventoryRepository;

        public RepairEstimateSparePartService(
            IRepairEstimateSparePartRepository repo,
            IInventoryRepository inventoryRepository)
        {
            _repo = repo;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<RepairEstimateSparePartResponse> CreateAsync(RepairEstimateSparePartCreateRequest request, CancellationToken ct = default)
        {
            if (!await _repo.RepairEstimateExistsAsync(request.RepairEstimateId, ct))
                throw new InvalidOperationException("RepairEstimate not found");

            var inventory = await _inventoryRepository.GetByIdAsync(request.SparePartId);
            if (inventory == null)
                throw new InvalidOperationException("SparePart not found");

            if (request.Quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than 0");

            var existed = await _repo.GetByIdAsync(request.RepairEstimateId, request.SparePartId, ct);
            if (existed != null)
                throw new InvalidOperationException("RepairEstimateSparePart already exists");

            if (!inventory.SellingPrice.HasValue)
                throw new InvalidOperationException("SparePart does not have a selling price in inventory");

            var unitPrice = inventory.SellingPrice.Value;

            var entity = new RepairEstimateSparePart
            {
                RepairEstimateId = request.RepairEstimateId,
                SparePartId = request.SparePartId,
                Status = RepairEstimateApprovalStatus.WaitingApproval,
                Quantity = request.Quantity,
                UnitPrice = unitPrice,
                TotalAmount = unitPrice * request.Quantity
            };

            await _repo.AddAsync(entity, ct);
            return Map(entity);
        }

        public async Task<RepairEstimateSparePartResponse?> UpdateStatusAsync(int repairEstimateId, int sparePartId, RepairEstimateSparePartStatusUpdateRequest request, CancellationToken ct = default)
        {
            ValidateStatus(request.Status);

            var entity = await _repo.GetTrackedByIdAsync(repairEstimateId, sparePartId, ct);
            if (entity == null)
                return null;

            entity.Status = request.Status;
            await _repo.UpdateAsync(entity, ct);
            return Map(entity);
        }

        private static RepairEstimateSparePartResponse Map(RepairEstimateSparePart entity)
        {
            return new RepairEstimateSparePartResponse
            {
                RepairEstimateId = entity.RepairEstimateId,
                SparePartId = entity.SparePartId,
                Status = entity.Status,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                TotalAmount = entity.TotalAmount
            };
        }

        private static void ValidateStatus(RepairEstimateApprovalStatus status)
        {
            if (!Enum.IsDefined(typeof(RepairEstimateApprovalStatus), status))
                throw new InvalidOperationException("Invalid repair estimate spare part status");
        }
    }
}
