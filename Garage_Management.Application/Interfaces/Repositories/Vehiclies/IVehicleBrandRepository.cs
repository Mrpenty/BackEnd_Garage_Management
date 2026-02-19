using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Vehiclies;
using Garage_Management.Base.Interface;
using Garage_Management.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Repositories.Vehiclies
{
    public interface IVehicleBrandRepository : IBaseRepository<VehicleBrand>
    {
        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách xe máy được phân trang
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>    
        Task<PagedResult<VehicleBrand>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Kiểm tra brand đã có model hay chưa
        /// </summary>
        Task<bool> HasModelsAsync(int brandId, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Kiểm tra brand đã có bản ghi nào trong appointment chưa 
        /// </summary>
        Task<bool> HasVehiclesAsync(int brandId, CancellationToken ct = default);
        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Kiểm tra brand đã tồn tại hay chưa
        /// </summary>
        Task<bool> HasExistAsync(string brandName, int? excludeId, CancellationToken ct = default);
    }
}
