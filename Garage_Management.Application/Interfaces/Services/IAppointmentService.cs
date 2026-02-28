using Garage_Management.Application.DTOs.Appointments;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Common.Models.Appointments;

namespace Garage_Management.Application.Interfaces.Services
{
    /*
     * Author: KhanhDV
     * Created Date: 13-02-2026
    */
    public interface IAppointmentService
    {
        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy chi tiết 1 lịch đặt
        /// </summary> 
        Task<AppointmentResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách lịch đặt theo khách hàng
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="customerId">Id của khách hàng</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>    
        Task<PagedResult<AppointmentResponse>> GetByCustomerIdAsync(int page, int pageSize, int customerId, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách lịch đặt được phân trang
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param> 
        Task<PagedResult<AppointmentResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 26-2-2026
        /// <summary>
        /// Lấy danh sách lịch đặt có phân trang, lọc, tìm kiếm, sắp xếp
        /// </summary>
        /// <param name="query">
        /// - status: có thể chọn nhiều status
        /// - Filter: pending | confirmed | completed | canceled | {customerId}
        /// - Search: Description, FirstName, LastName, Phone
        /// - OrderBy: appointmentdatetime | status | createdat
        /// - SortOrder: ASC | DESC
        /// </param>
        Task<PagedResult<AppointmentResponse>> GetPagedAsync(AppointmentQuery query, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Tạo 1 lịch đặt mới
        /// </summary>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param> 
        Task<AppointmentResponse> CreateAsync(AppointmentCreateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Update 1 lịch đặt cũ
        /// </summary>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param> 
        Task<AppointmentResponse?> UpdateAsync(int id, AppointmentUpdateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Xóa 1 lịch đặt đã tồn tại
        /// </summary>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param> 
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
