using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Vehiclies;
using Garage_Management.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Repositories.Vehiclies
{

    public interface IVehicleModelRepository: IBaseRepository<VehicleModel>
    {
        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách Model của xe máy được phân trang
        /// </summary>
        /// <param name="page">Số trang hiện tại (bắt đầu từ 1)</param>
        /// <param name="pageSize">Tổng số trang</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>         
        Task<PagedResult<VehicleModel>> GetPagedAsync (int page, int pageSize, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Kiểm tra model này đã tồn tại hay chưa
        /// </summary>   
        /// <param name="typeId">Loại xe cần kiểm tra</param>
        /// <param name="brandId">Hãng xe cần kiểm tra</param>
        /// <param name="modelName">Tên model cần kiểm tra khi trùng</param>
        /// <param name="excludeId">Bỏ qua 1 modelId khi update</param>
        /// <param name="ct">Để dừng các query khi tắt page hoặc tắt app.</param>
        Task<bool> ExistsAsync(int brandId, int typeId, string modelName, int? excludeId = null, CancellationToken ct = default);
        /// Author: KhanhDV
        /// Created Date: 13-2-2026
        /// <summary>
        /// Kiểm tra model này đã tồn tại ở bản ghi nào ở vehicle hay chưa
        /// </summary>  
        Task<bool> HasVehiclesAsync(int modelId, CancellationToken ct = default);
    }
}
