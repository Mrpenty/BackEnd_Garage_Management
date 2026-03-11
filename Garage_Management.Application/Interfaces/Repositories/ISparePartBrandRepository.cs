using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories
{
    public interface ISparePartBrandRepository : IBaseRepository<SparePartBrand>
    {
        Task<PagedResult<SparePartBrand>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<bool> HasExistAsync(string brandName, int? excludeId = null, CancellationToken ct = default);
        Task<bool> HasSparePartsAsync(int brandId, CancellationToken ct = default);
    }
}
