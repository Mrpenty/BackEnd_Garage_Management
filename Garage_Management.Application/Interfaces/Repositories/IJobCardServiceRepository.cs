using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Garage_Management.Base.Entities.JobCards;
namespace Garage_Management.Application.Interfaces.Repositories
{
    

    public interface IJobCardServiceRepository
    {
        Task AddAsync(JobCardService entity, CancellationToken cancellationToken);
        //Task AddAsync(Base.Entities.JobCards.JobCardService jobCardService, CancellationToken cancellationToken);
        IQueryable<JobCardService> Query();

        Task SaveAsync(CancellationToken cancellationToken);
    }

}
