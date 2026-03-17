using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Services;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories.Services
{
    public interface IServiceRepository : IBaseRepository<Service>
    {
        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách dịch vụ được phân trang
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>
        Task<PagedResult<Service>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 6-3-2026
        /// <summary>
        /// Lấy danh sách dịch vụ theo loại xe (VehicleType)
        /// </summary>
        Task<List<Service>> GetByVehicleTypeAsync(int vehicleTypeId, CancellationToken ct = default);

        /// <summary>
        /// Kiểm tra service đã phát sinh dữ liệu liên quan ở các bảng con hay chưa.
        /// </summary>
        Task<bool> HasDependenciesAsync(int serviceId, CancellationToken ct = default);

        /// <summary>
        /// Kiểm tra tên dịch vụ đã tồn tại hay chưa.
        /// </summary>
        Task<bool> ExistsByNameAsync(string serviceName, CancellationToken ct = default);
        Task<PagedResult<ServiceVehicleType>> GetServiceVehicleTypePairsPagedAsync(int page, int pageSize, CancellationToken ct = default);
    }
}
