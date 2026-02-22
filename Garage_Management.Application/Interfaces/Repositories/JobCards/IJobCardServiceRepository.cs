using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Interface;

namespace Garage_Management.Application.Interfaces.Repositories.JobCards
{
    public interface IJobCardServiceRepository : IBaseRepository<JobCardService>
    {
        Task<PagedResult<JobCardService>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
    }
}
