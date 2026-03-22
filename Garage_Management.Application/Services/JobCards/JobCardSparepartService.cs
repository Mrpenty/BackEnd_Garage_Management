using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Services.JobCard;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.JobCards;

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

    public async Task<JobCardSparePart?> AddSparePartAsync(
        int jobCardId,
        AddSparePartToJobCardDto dto,
        CancellationToken ct)
    {
        var jobCard = await _jobCardRepository.GetByIdAsync(jobCardId);
        if (jobCard == null)
            return null;

        if (dto.Quantity <= 0)
            throw new InvalidOperationException("Quantity phải lớn hơn 0");

        var inventory = await _inventoryRepository.GetByIdAsync(dto.SparePartId);
        if (inventory == null)
            throw new InvalidOperationException("Phụ tùng không tồn tại");

        if (inventory.Quantity < dto.Quantity)
            throw new InvalidOperationException("Không đủ tồn kho");

        var unitPrice = inventory.SellingPrice ?? 0m;

        var entity = new JobCardSparePart
        {
            JobCardId = jobCardId,
            SparePartId = dto.SparePartId,
            Quantity = dto.Quantity,
            UnitPrice = unitPrice,
            TotalAmount = unitPrice * dto.Quantity,
            IsUnderWarranty = dto.IsUnderWarranty,
            Note = dto.Note,
            CreatedAt = DateTime.UtcNow
        };

        inventory.Quantity -= dto.Quantity;
        inventory.UpdatedAt = DateTime.UtcNow;

        await _jobCardSparePartRepository.AddAsync(entity, ct);
        await _jobCardSparePartRepository.SaveAsync(ct);

        return entity;
    }

    public async Task<bool> RemoveSparePartAsync(
     int jobCardSparePartId,
     CancellationToken ct)
    {
        // 1. Lấy phụ tùng trong jobcard
        var entity = await _jobCardSparePartRepository.GetByIdAsync(jobCardSparePartId);
        if (entity == null)
            return false;

        // 2. Kiểm tra trạng thái jobcard (chỉ cho xóa khi bị từ chối)
        var jobCard = await _jobCardRepository.GetByIdAsync(entity.JobCardId);
        if (jobCard == null)
            throw new InvalidOperationException("Không tìm thấy JobCard");

        if (jobCard.Status != JobCardStatus.Rejected)
            throw new InvalidOperationException("Chỉ được xóa phụ tùng khi khách từ chối sửa");

        // 3. Lấy tồn kho theo SparePartId (đúng mapping)
        var inventory = await _inventoryRepository.GetByIdAsync(entity.SparePartId);
        if (inventory == null)
            throw new InvalidOperationException("Không tìm thấy tồn kho");

        // 4. Hoàn lại tồn kho
        inventory.Quantity += entity.Quantity;
        inventory.UpdatedAt = DateTime.UtcNow;

        // 5. Xóa record phụ tùng khỏi jobcard
        _jobCardSparePartRepository.Delete(entity);

        // 6. Lưu tất cả thay đổi trong cùng transaction
        await _jobCardSparePartRepository.SaveAsync(ct);

        return true;
    }
}