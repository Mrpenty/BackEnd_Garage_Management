using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Services;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories.Services
{
    public interface IServiceRepository : IBaseRepository<Service>
    {
        Task<PagedResult<Service>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    }
}
