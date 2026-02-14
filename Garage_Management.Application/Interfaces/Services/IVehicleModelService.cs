using Garage_Management.Application.DTOs.Vehicles.VehicleModel;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services
{
    /*
     * Author: KhanhDV
     * Created Date: 13-02-2026
    */
    public interface IVehicleModelService
    {
        Task<VehicleModelResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResult<VehicleModelResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<VehicleModelResponse> CreateAsync(VehicleModelCreateRequest request, CancellationToken ct = default);
        Task<VehicleModelResponse?> UpdateAsync(int id, VehicleModelUpdate request, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
