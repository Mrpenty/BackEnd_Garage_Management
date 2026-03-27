using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Services.JobCard;
using Garage_Management.Base.Entities.JobCards;

namespace Garage_Management.Application.Services.JobCards
{
    public class JobCardSparePartService : IJobCardSparePartService
    {
        private readonly IJobCardRepository _jobCardRepository;
        private readonly IJobCardSparePartRepository _jobCardSparePartRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public JobCardSparePartService(
            IJobCardRepository jobCardRepository,
            IJobCardSparePartRepository jobCardSparePartRepository,
            IInventoryRepository inventoryRepository)
        {
            _jobCardRepository = jobCardRepository;
            _jobCardSparePartRepository = jobCardSparePartRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task<List<JobCardSparePartResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            var entities = await _jobCardSparePartRepository.GetAllWithDetailsAsync(cancellationToken);
            return entities.Select(Map).ToList();
        }

        public async Task<List<JobCardSparePartResponse>> GetByJobCardIdAsync(int jobCardId, CancellationToken cancellationToken)
        {
            var entities = await _jobCardSparePartRepository.GetByJobCardIdAsync(jobCardId, cancellationToken);
            return entities.Select(Map).ToList();
        }

        public async Task<List<JobCardSparePartResponse>?> AddSparePartsAsync(
            int jobCardId,
            AddMultipleSparePartsToJobCardDto dto,
            CancellationToken ct)
        {
            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);
            if (jobCard == null)
                return null;

            if (dto.SpareParts == null || dto.SpareParts.Count == 0)
                throw new InvalidOperationException("Danh sach phu tung khong duoc rong");

            var duplicateIds = dto.SpareParts
                .GroupBy(x => x.SparePartId)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicateIds.Count > 0)
                throw new InvalidOperationException($"SparePartId bi trung trong request: {string.Join(", ", duplicateIds)}");

            var entities = new List<JobCardSparePart>();

            foreach (var item in dto.SpareParts)
            {
                if (item.Quantity <= 0)
                    throw new InvalidOperationException($"Quantity cua phu tung {item.SparePartId} phai lon hon 0");

                var existed = await _jobCardSparePartRepository.GetByIdAsync(jobCardId, item.SparePartId, ct);
                if (existed != null)
                    throw new InvalidOperationException($"Phu tung {item.SparePartId} da ton tai trong job card");

                var inventory = await _inventoryRepository.GetByIdAsync(item.SparePartId);
                if (inventory == null)
                    throw new InvalidOperationException($"Phu tung {item.SparePartId} khong ton tai");

                if (inventory.Quantity < item.Quantity)
                    throw new InvalidOperationException($"Phu tung {item.SparePartId} khong du ton kho");

                var unitPrice = inventory.SellingPrice ?? 0m;
                var entity = new JobCardSparePart
                {
                    JobCardId = jobCardId,
                    SparePartId = item.SparePartId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    TotalAmount = unitPrice * item.Quantity,
                    IsUnderWarranty = item.IsUnderWarranty,
                    Note = item.Note,
                    CreatedAt = DateTime.UtcNow
                };

                inventory.Quantity -= item.Quantity;
                inventory.UpdatedAt = DateTime.UtcNow;

                await _jobCardSparePartRepository.AddAsync(entity, ct);
                entities.Add(entity);
            }

            await _jobCardSparePartRepository.SaveAsync(ct);
            return entities.Select(Map).ToList();
        }

        public async Task<bool> RemoveSparePartAsync(
            int jobCardId,
            int sparePartId,
            CancellationToken ct)
        {
            var entity = await _jobCardSparePartRepository.GetByIdAsync(jobCardId, sparePartId, ct);
            if (entity == null)
                return false;

            var jobCard = await _jobCardRepository.GetByIdAsync(entity.JobCardId);
            if (jobCard == null)
                throw new InvalidOperationException("Khong tim thay JobCard");

            var inventory = await _inventoryRepository.GetByIdAsync(entity.SparePartId);
            if (inventory == null)
                throw new InvalidOperationException("Khong tim thay ton kho");

            inventory.Quantity += entity.Quantity;
            inventory.UpdatedAt = DateTime.UtcNow;

            _jobCardSparePartRepository.Delete(entity);
            await _jobCardSparePartRepository.SaveAsync(ct);

            return true;
        }

        private static JobCardSparePartResponse Map(JobCardSparePart entity)
        {
            return new JobCardSparePartResponse
            {
                JobCardId = entity.JobCardId,
                SparePartId = entity.SparePartId,
                Quantity = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                TotalAmount = entity.TotalAmount,
                IsUnderWarranty = entity.IsUnderWarranty,
                Note = entity.Note,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
