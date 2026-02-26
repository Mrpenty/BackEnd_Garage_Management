using Garage_Management.Application.DTOs.Inventories.SparePartBrands;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services.Inventories
{
    public interface ISparePartBrandService
    {
        Task<SparePartBrandResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResult<SparePartBrandResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<SparePartBrandResponse> CreateAsync(SparePartBrandCreateRequest request, CancellationToken ct = default);
        Task<SparePartBrandResponse?> UpdateAsync(int id, SparePartBrandUpdateRequest request, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
