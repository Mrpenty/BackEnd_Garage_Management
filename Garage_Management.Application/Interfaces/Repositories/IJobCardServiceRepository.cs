using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Repositories
{
    using Garage_Management.Base.Entities.JobCards;

    public interface IJobCardServiceRepository
    {
        Task AddAsync(JobCardService entity, CancellationToken cancellationToken);

        IQueryable<JobCardService> Query();

        Task SaveAsync(CancellationToken cancellationToken);
    }

}
