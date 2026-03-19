using Garage_Management.Application.DTOs.JobCards;

namespace Garage_Management.Application.Interfaces.Services.JobCard
{
    public interface IJobCardSparePartService
    {
        /// <summary>
        /// Thêm phụ tùng vào phiếu sửa chữa
        /// </summary>
        Task<bool> AddSparePartAsync(int jobCardId, AddSparePartToJobCardDto dto, CancellationToken cancellationToken);

    }
}