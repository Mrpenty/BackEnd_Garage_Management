using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Vehiclies;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories.Vehiclies
{
    /*
     * Author: KhanhDV
     * Created Date: 13-02-2026
    */
    public interface IVehicleRepository : IBaseRepository<Vehicle>
    {
        Task<PagedResult<Vehicle>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<PagedResult<Vehicle>> GetByCustomerIdAsync(int page, int pageSize, int customerId, CancellationToken ct = default);
    }
}
