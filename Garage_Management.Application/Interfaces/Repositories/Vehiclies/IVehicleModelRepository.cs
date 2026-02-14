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
    /*
     * Author: KhanhDV
     * Created Date: 13-02-2026
    */
    public interface IVehicleModelRepository: IBaseRepository<VehicleModel>
    {
        Task<PagedResult<VehicleModel>> GetPagedAsync (int page, int pageSize, CancellationToken ct = default);
        Task<bool> ExistsAsync(int brandId, string modelName, int? excludeId = null, CancellationToken ct = default);
    }
}
