using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Services;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories.Services
{
    public interface IServiceTaskRepository : IBaseRepository<ServiceTask>
    {
        ///Author: KhanhDV
        ///Created Date: 20-2-2026
        /// <summary>
        /// Lấy danh sách công việc của từng dịch vụ được phân trang
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>
        Task<PagedResult<ServiceTask>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        ///Author: KhanhDV
        ///Created Date: 20-2-2026
        /// <summary>
        /// Kiểm tra trong cùng một Service có task nào trùng thứ tự (TaskOrder) hay chưa.
        /// </summary>
        /// <param name="serviceId">ID dịch vụ cần kiểm tra.</param>
        /// <param name="taskOrder">Thứ tự task trong service.</param>
        /// <param name="excludeId">Bỏ qua một ServiceTaskId (khi update).</param>
        /// <param name="ct">Token để hủy truy vấn.</param>
        /// <returns>true nếu đã tồn tại task trùng thứ tự trong service.</returns>
        Task<bool> HasExistAsync(int serviceId, int taskOrder, int? excludeId = null, CancellationToken ct = default);

        ///Author: KhanhDV
        ///Created Date: 20-2-2026
        /// <summary>
        /// Lấy danh sách các ServiceTask theo từng dịch vụ.
        /// </summary>
        /// <param name="serviceId">ID dịch vụ cần kiểm tra.</param>
        /// <param name="ct">Token để hủy truy vấn.</param>
        Task<List<ServiceTask>> GetByServiceIdAsync(int serviceId, CancellationToken ct = default);
    }
}
