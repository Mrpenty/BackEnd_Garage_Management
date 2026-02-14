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
    /*
     * Author: KhanhDV
     * Created Date: 13-02-2026
    */
    public interface IVehicleBrandRepository : IBaseRepository<VehicleBrand>
    {
        Task<PagedResult<VehicleBrand>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    }
}
