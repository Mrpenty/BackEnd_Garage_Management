using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Services.JobCard;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Inventories;
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
            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);
            if (jobCard == null)
                throw new KeyNotFoundException($"JobCard {jobCardId} khong ton tai");

            var entities = await _jobCardSparePartRepository.GetByJobCardIdAsync(jobCardId, cancellationToken);
            return entities.Select(Map).ToList();
        }

        public async Task<List<JobCardSparePartResponse>?> AddSparePartsAsync(
            int jobCardId,
            AddMultipleSparePartsToJobCardDto dto,
            CancellationToken ct)
        {
            ValidateJobCardId(jobCardId);
            ArgumentNullException.ThrowIfNull(dto);
            ValidateSparePartItems(dto.SpareParts);

            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);
            if (jobCard == null)
                return null;

            EnsureJobCardApprovedForSparePartCreation(jobCard);

            var entities = new List<JobCardSparePart>();

            foreach (var item in dto.SpareParts)
            {
                var existed = await _jobCardSparePartRepository.GetByIdAsync(jobCardId, item.SparePartId, ct);
                if (existed != null)
                    throw new InvalidOperationException($"Phu tung {item.SparePartId} da ton tai trong JobCard");

                var inventory = await _inventoryRepository.GetByIdAsync(item.SparePartId);
                if (inventory == null)
                    throw new InvalidOperationException($"Khong tim thay phu tung {item.SparePartId}");

                if (inventory.BranchId != jobCard.BranchId)
                    throw new InvalidOperationException($"Phu tung {item.SparePartId} khong thuoc chi nhanh cua JobCard");

                ValidateInventoryForSparePart(item.SparePartId, inventory);

                if (inventory.Quantity < item.Quantity)
                    throw new InvalidOperationException($"Phu tung {item.SparePartId} khong du so luong ton kho");

                var unitPrice = inventory.SellingPrice!.Value;
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

                await _jobCardSparePartRepository.AddAsync(entity, ct);
                entities.Add(entity);
            }

            await _jobCardSparePartRepository.SaveAsync(ct);
            return entities.Select(Map).ToList();
        }

        public async Task<JobCardSparePartResponse?> UpdateAsync(
            int jobCardId,
            int sparePartId,
            UpdateJobCardSparePartDto dto,
            CancellationToken ct)
        {
            ValidateJobCardId(jobCardId);
            ValidateSparePartId(sparePartId);
            ArgumentNullException.ThrowIfNull(dto);
            ValidateUpdateRequest(dto);

            var entity = await _jobCardSparePartRepository.GetByIdAsync(jobCardId, sparePartId, ct);
            if (entity == null)
                return null;

            var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);
            if (jobCard == null)
                throw new InvalidOperationException("Khong tim thay JobCard");

            var hasChanges = false;

            if (dto.Quantity.HasValue && dto.Quantity.Value != entity.Quantity)
            {
                var inventory = await _inventoryRepository.GetByIdAsync(sparePartId);
                if (inventory == null)
                    throw new InvalidOperationException($"Khong tim thay phu tung {sparePartId}");

                if (inventory.BranchId != jobCard.BranchId)
                    throw new InvalidOperationException($"Phu tung {sparePartId} khong thuoc chi nhanh cua JobCard");

                ValidateInventoryForSparePart(sparePartId, inventory);

                if (inventory.Quantity < dto.Quantity.Value)
                    throw new InvalidOperationException($"Phu tung {sparePartId} khong du so luong ton kho");

                entity.Quantity = dto.Quantity.Value;
                entity.TotalAmount = entity.Quantity * entity.UnitPrice;
                hasChanges = true;
            }

            if (dto.IsUnderWarranty.HasValue)
            {
                entity.IsUnderWarranty = dto.IsUnderWarranty.Value;
                hasChanges = true;
            }

            if (dto.Note != null)
            {
                entity.Note = dto.Note;
                hasChanges = true;
            }

            if (!hasChanges)
                throw new InvalidOperationException("Khong co du lieu de cap nhat");

            await _jobCardSparePartRepository.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<bool> RemoveSparePartAsync(
            int jobCardId,
            int sparePartId,
            CancellationToken ct)
        {
            ValidateJobCardId(jobCardId);
            ValidateSparePartId(sparePartId);

            var entity = await _jobCardSparePartRepository.GetByIdAsync(jobCardId, sparePartId, ct);
            if (entity == null)
                return false;

            _jobCardSparePartRepository.Delete(entity);
            await _jobCardSparePartRepository.SaveAsync(ct);

            return true;
        }

        private static void ValidateJobCardId(int jobCardId)
        {
            if (jobCardId <= 0)
                throw new InvalidOperationException("JobCardId phai lon hon 0");
        }

        private static void ValidateSparePartId(int sparePartId)
        {
            if (sparePartId <= 0)
                throw new InvalidOperationException("SparePartId phai lon hon 0");
        }

        private static void ValidateUpdateRequest(UpdateJobCardSparePartDto dto)
        {
            if (!dto.Quantity.HasValue && !dto.IsUnderWarranty.HasValue && dto.Note == null)
                throw new InvalidOperationException("Khong co du lieu de cap nhat");

            if (dto.Quantity.HasValue && dto.Quantity.Value <= 0)
                throw new InvalidOperationException("Quantity phai lon hon 0");

            if (dto.Note != null && string.IsNullOrWhiteSpace(dto.Note))
                throw new InvalidOperationException("Note khong duoc chi chua khoang trang");

            if (dto.Note?.Length > 500)
                throw new InvalidOperationException("Note khong duoc vuot qua 500 ky tu");
        }

        private static void ValidateSparePartItems(List<AddSparePartToJobCardDto>? spareParts)
        {
            if (spareParts == null || spareParts.Count == 0)
                throw new InvalidOperationException("Danh sach phu tung khong duoc rong");

            var duplicateIds = spareParts
                .Where(x => x != null)
                .GroupBy(x => x.SparePartId)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicateIds.Count > 0)
                throw new InvalidOperationException($"SparePartId bi trung trong yeu cau: {string.Join(", ", duplicateIds)}");

            foreach (var item in spareParts)
            {
                if (item == null)
                    throw new InvalidOperationException("Thong tin phu tung khong hop le");

                ValidateSparePartId(item.SparePartId);

                if (item.Quantity <= 0)
                    throw new InvalidOperationException($"Quantity cua phu tung {item.SparePartId} phai lon hon 0");

                if (item.Note != null && string.IsNullOrWhiteSpace(item.Note))
                    throw new InvalidOperationException($"Note cua phu tung {item.SparePartId} khong duoc chi chua khoang trang");

                if (item.Note?.Length > 500)
                    throw new InvalidOperationException($"Note cua phu tung {item.SparePartId} khong duoc vuot qua 500 ky tu");
            }
        }

        private static void ValidateInventoryForSparePart(int sparePartId, Inventory inventory)
        {
            if (!inventory.IsActive)
                throw new InvalidOperationException($"Phu tung {sparePartId} da ngung kinh doanh");

            if (!inventory.SellingPrice.HasValue)
                throw new InvalidOperationException($"Phu tung {sparePartId} chua co gia ban");

            if (inventory.SellingPrice.Value < 0)
                throw new InvalidOperationException($"Gia ban cua phu tung {sparePartId} khong hop le");
        }

        private static void EnsureJobCardApprovedForSparePartCreation(JobCard jobCard)
        {
            if (jobCard.Status == JobCardStatus.WaitingSupervisorApproval)
                throw new InvalidOperationException("JobCard dang cho supervisor phe duyet. Hay phe duyet supervisor truoc, sau do chuyen sang WaitingCustomerApproval");

            if (jobCard.Status != JobCardStatus.WaitingCustomerApproval)
                throw new InvalidOperationException($"Chi duoc tao phu tung khi JobCard o trang thai WaitingCustomerApproval. Trang thai hien tai: {jobCard.Status}.");
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
