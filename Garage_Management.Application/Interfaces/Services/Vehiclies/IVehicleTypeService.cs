using Garage_Management.Application.DTOs.Vehicles.VehicleType;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services.Vehiclies
{
    public interface IVehicleTypeService
    {
        Task<VehicleTypeResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResult<VehicleTypeResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<VehicleTypeResponse> CreateAsync(VehicleTypeCreateRequest request, CancellationToken ct = default);
        Task<VehicleTypeResponse?> ActivateAsync(int id, CancellationToken ct = default);
        Task<VehicleTypeResponse?> DeactivateAsync(int id, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
