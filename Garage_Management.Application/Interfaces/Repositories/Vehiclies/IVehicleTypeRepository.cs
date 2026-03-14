using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Vehiclies;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories.Vehiclies
{
    public interface IVehicleTypeRepository : IBaseRepository<VehicleType>
    {
        Task<PagedResult<VehicleType>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<bool> ExistsByTypeNameAsync(string typeName, int? excludeId = null, CancellationToken ct = default);
        Task<bool> HasModelsAsync(int vehicleTypeId, CancellationToken ct = default);
        Task<bool> HasServiceMappingsAsync(int vehicleTypeId, CancellationToken ct = default);
    }
}

