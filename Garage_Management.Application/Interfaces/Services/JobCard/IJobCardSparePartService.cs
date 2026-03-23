using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Base.Entities.JobCards;

namespace Garage_Management.Application.Interfaces.Services.JobCard
{
    public interface IJobCardSparePartService
    {
        /// <summary>
        /// Thêm phụ tùng vào phiếu sửa chữa
        /// </summary>
        Task<JobCardSparePart?> AddSparePartAsync(int jobCardId, AddSparePartToJobCardDto dto, CancellationToken cancellationToken);
        Task<bool> RemoveSparePartAsync(int jobCardSparePartId,CancellationToken ct);
    }
}