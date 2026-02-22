using Garage_Management.Application.DTOs.WarrantyServices;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services
{
    public interface IWarrantyServiceService
    {
        Task<WarrantyServiceResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResult<WarrantyServiceResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<WarrantyServiceResponse> CreateAsync(WarrantyServiceCreateRequest request, CancellationToken ct = default);
        Task<WarrantyServiceResponse?> UpdateAsync(int id, WarrantyServiceUpdateRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
