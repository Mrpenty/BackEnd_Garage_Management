using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories
{
    public interface ISparePartCategoryRepository : IBaseRepository<SparePartCategory>
    {
        Task<PagedResult<SparePartCategory>> GetPagedAsync(ParamQuery query, bool onlyActive = false, CancellationToken ct = default);
        Task<bool> HasExistAsync(string categoryName, int? excludeId = null, CancellationToken ct = default);
        Task<bool> HasSparePartsAsync(int categoryId, CancellationToken ct = default);
    }
}
