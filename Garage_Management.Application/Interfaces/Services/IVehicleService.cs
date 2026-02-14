using Garage_Management.Application.DTOs.Vehicles;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services
{
    /*
     * Author: KhanhDV
     * Created Date: 13-02-2026
    */
    public interface IVehicleService
    {
        Task<VehicleResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResult<VehicleResponse>> GetByCustomerIdAsync(int page, int pageSize, int customerId, CancellationToken ct = default);
        Task<PagedResult<VehicleResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<VehicleResponse> CreateAsync(VehicleCreateRequest request, CancellationToken ct = default);
        Task<VehicleResponse?> UpdateAsync(int id, VehicleUpdateRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
