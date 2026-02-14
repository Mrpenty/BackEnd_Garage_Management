using Garage_Management.Application.DTOs.Vehicles.VehicleBrand;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services
{
    /*
     * Author: KhanhDV
     * Created Date: 13-02-2026
    */
    public interface IVehicleBrandService
    {
        Task<VehicleBrandResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResult<VehicleBrandResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<VehicleBrandResponse> CreateAsync(VehicleBrandCreateRequest request, CancellationToken ct = default);
        Task<VehicleBrandResponse?> UpdateAsync(int id, VehicleBrandUpdate request, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
