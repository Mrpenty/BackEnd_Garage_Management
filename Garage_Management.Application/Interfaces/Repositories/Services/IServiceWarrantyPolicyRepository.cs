using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Services;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories.Services
{
    public interface IServiceWarrantyPolicyRepository : IBaseRepository<ServiceWarrantyPolicy>
    {
        Task<PagedResult<ServiceWarrantyPolicy>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    }
}
