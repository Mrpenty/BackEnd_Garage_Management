using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Garage_Management.Base.Entities.JobCards;
namespace Garage_Management.Application.Interfaces.Repositories.JobCards
{
    

    public interface IJobCardSparePartRepository
    {
        Task AddAsync(JobCardSparePart entity, CancellationToken cancellationToken);
        //Task AddAsync(Base.Entities.JobCards.JobCardSparePart entity, CancellationToken cancellationToken);
        IQueryable<JobCardSparePart> Query();

        Task SaveAsync(CancellationToken cancellationToken);
    }

}
