using Garage_Management.Application.DTOs.Iventories.StockTransactions;
using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Application.Interfaces.Services.JobCard;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.JobCards;

namespace Garage_Management.Application.Services.JobCards
{
    public class JobCardSparePartService : IJobCardSparePartService
    {
        private readonly IJobCardRepository _jobCardRepository;
        private readonly IJobCardSparePartRepository _jobCardSparePartRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IStockTransactionService _stockTransactionService;

        public JobCardSparePartService(
            IJobCardRepository jobCardRepository,
            IJobCardSparePartRepository jobCardSparePartRepository,
            IInventoryRepository inventoryRepository,
            IStockTransactionService stockTransactionService)
        {
            _jobCardRepository = jobCardRepository;
            _jobCardSparePartRepository = jobCardSparePartRepository;
            _inventoryRepository = inventoryRepository;
            _stockTransactionService = stockTransactionService;
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

            EnsureJobCardApprovedForSparePartCreation(jobCard);

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
                    throw new InvalidOperationException($"Phụ tùng {item.SparePartId} đã tồn tại trong jobcard");

                var inventory = await _inventoryRepository.GetByIdAsync(item.SparePartId);
                if (inventory == null)
                    throw new InvalidOperationException($"Phụ tùng {item.SparePartId} không tồn tại");

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

                // Tạo giao dịch xuất kho thay vì trừ tồn kho trực tiếp
                await _stockTransactionService.CreateAsync(new StockTransactionCreateRequest
                {
                    SparePartId = item.SparePartId,
                    TransactionType = TransactionType.ExportToJobCard,
                    QuantityChange = item.Quantity,
                    UnitPrice = unitPrice,
                    JobCardId = jobCardId,
                    Note = $"Xuất kho cho JobCard #{jobCardId}"
                }, ct);

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
                throw new InvalidOperationException("Không tìm thấy JobCard");

            // Tạo giao dịch nhập kho lại (trả phụ tùng về kho)
            var inventory = await _inventoryRepository.GetByIdAsync(entity.SparePartId);
            if (inventory == null)
                throw new InvalidOperationException("Không tìm thấy tồn kho");

            await _stockTransactionService.CreateAsync(new StockTransactionCreateRequest
            {
                SparePartId = entity.SparePartId,
                TransactionType = TransactionType.ReturnFromJobCard,
                QuantityChange = entity.Quantity,
                UnitPrice = entity.UnitPrice,
                JobCardId = entity.JobCardId,
                Note = $"Hoàn kho từ JobCard #{entity.JobCardId}"
            }, ct);

            _jobCardSparePartRepository.Delete(entity);
            await _jobCardSparePartRepository.SaveAsync(ct);

            return true;
        }

        private static void EnsureJobCardApprovedForSparePartCreation(JobCard jobCard)
        {
            if (jobCard.Status == JobCardStatus.WaitingSupervisorApproval)
                throw new InvalidOperationException(
                    "JobCard đang chờ supervisor phê duyệt. Hãy phê duyệt supervisor trước, sau đó chuyển sang WaitingCustomerApproval");

            if (jobCard.Status != JobCardStatus.WaitingCustomerApproval)
                throw new InvalidOperationException(
                    $"Chỉ được tạo phụ tùng khi JobCard ở trạng thái WaitingCustomerApproval. Trạng thái hiện tại: {jobCard.Status}.");
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
