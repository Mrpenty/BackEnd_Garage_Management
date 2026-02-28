using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Common.Models.Appointments;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories.Appointments
{
    public interface IAppointmentRepository : IBaseRepository<Appointment>
    {
        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách lịch đặt được phân trang
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param> 
        Task<PagedResult<Appointment>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách lịch đặt được phân trang
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param> 
        Task<PagedResult<Appointment>> GetByCustomerIdAsync(int page, int pageSize, int customerId, CancellationToken ct = default);

        /// <summary>
        /// Lấy chi tiết lịch hẹn kèm services/tasks.
        /// </summary>
        Task<Appointment?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Lấy danh sách lịch đặt có phân trang, lọc, tìm kiếm, sắp xếp
        /// </summary>
        Task<PagedResult<Appointment>> GetPagedAsync(AppointmentQuery query, CancellationToken ct = default);

    }
}
