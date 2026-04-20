using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Base.Entities.JobCards;

namespace Garage_Management.Application.Interfaces.Services.JobCard
{
    public interface IJobCardSparePartService
    {
        Task<List<JobCardSparePartResponse>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<JobCardSparePartResponse>> GetByJobCardIdAsync(int jobCardId, CancellationToken cancellationToken);
        /// <summary>
        /// Thêm nhiều phụ tùng vào phiếu sửa chữa
        /// </summary>
        Task<List<JobCardSparePartResponse>?> AddSparePartsAsync(int jobCardId, AddMultipleSparePartsToJobCardDto dto, CancellationToken cancellationToken);
        Task<JobCardSparePartResponse?> UpdateAsync(int jobCardId, int sparePartId, UpdateJobCardSparePartDto dto, CancellationToken ct);
        Task<bool> RemoveSparePartAsync(int jobCardId, int sparePartId, CancellationToken ct);
    }
}
