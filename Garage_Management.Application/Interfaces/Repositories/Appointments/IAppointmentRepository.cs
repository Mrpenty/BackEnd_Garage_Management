using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories.Appointments
{
    /*
     * Module: Appointment Repository
     * Author: KhanhDV
     * Created Date: 13-02-2026
    */
    public interface IAppointmentRepository : IBaseRepository<Appointment>
    {
        /// <summary>
        /// Lấy danh sách lịch đặt được phân trang
        /// </summary>
        /// <param name="page">Current page number (starting from 1).</param>
        /// <param name="pageSize">Number of records per page.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// </returns>
        /// <remarks>
        Task<PagedResult<Appointment>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        /// <summary>
        /// Lấy danh sách lịch đặt được phân trang
        /// </summary>
        /// <param name="page">Current page number (starting from 1).</param>
        /// <param name="pageSize">Number of records per page.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// </returns>
        /// <remarks>
        /// Used for UC-11, UC14
        /// </remarks>
        Task<PagedResult<Appointment>> GetByCustomerIdAsync(int page, int pageSize, int customerId, CancellationToken ct = default);

    }
}
