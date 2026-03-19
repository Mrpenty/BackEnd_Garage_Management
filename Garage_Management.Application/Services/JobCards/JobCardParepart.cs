using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Services.JobCard;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.JobCards
{
    public class JobCardSparepartService : IJobCardSparePartService
    {

        private readonly IJobCardRepository _repository;
        private readonly IInventoryRepository _inventoryRepository;
            private readonly IJobCardSparePartRepository _jobCardSparePartRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;


            public JobCardSparepartService(
                 IJobCardRepository repository,
                IInventoryRepository inventoryRepository,
                IJobCardSparePartRepository jobCardSparePartRepository,
                IHttpContextAccessor httpContext)
            {
                 _repository = repository;
                 _inventoryRepository = inventoryRepository;
                _jobCardSparePartRepository = jobCardSparePartRepository;
                _httpContextAccessor = httpContext;
            }

            public async Task<bool> AddSparePartAsync(int jobCardId, AddSparePartToJobCardDto dto, CancellationToken cancellationToken)
        {
            // 1️⃣ Kiểm tra JobCard
            var jobCard = await _repository.GetByIdAsync(jobCardId);
            if (jobCard == null) return false;

            // 2️⃣ Lấy Inventory theo SparePartId
            var inventory = await _inventoryRepository.GetByIdAsync(dto.SparePartId);

            if (inventory == null) return false;

            if (dto.Quantity <= 0) return false;

            // 3️⃣ Lấy giá bán (nullable → decimal)
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

            await _jobCardSparePartRepository.AddAsync(entity, cancellationToken);
            await _jobCardSparePartRepository.SaveAsync(cancellationToken);

            return true;
        }
    }
}
