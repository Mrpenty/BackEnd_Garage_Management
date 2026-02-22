using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Warranties;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories.Warranties
{
    public interface IWarrantyServiceRepository : IBaseRepository<WarrantyService>
    {
        ///Author: KhanhDV
        ///Created Date: 20-2-2026
        /// <summary>
        /// Lấy danh sách chính sách bảo hành của từng dịch vụ được phân trang
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>
        Task<PagedResult<WarrantyService>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    }
}
