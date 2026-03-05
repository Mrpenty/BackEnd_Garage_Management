using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Vehiclies;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories.Vehiclies
{
    public interface IVehicleRepository : IBaseRepository<Vehicle>
    {
        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách Model của xe máy được phân trang
        /// </summary>
        Task<ApiResponse<PagedResult<Vehicle>>> GetPagedAsync(ParamQuery query, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách xe máy theo từng khách hàng được phân trang
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="customerId">Id của khách hàng</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>  
        Task<PagedResult<Vehicle>> GetByCustomerIdAsync(int page, int pageSize, int customerId, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Kiểm tra xe máy đã từng có trong bản ghi của lịch đặt nào chưa
        /// </summary>
        Task<bool> HasAppointmentsAsync(int vehicleId, CancellationToken ct = default);
    }
}
