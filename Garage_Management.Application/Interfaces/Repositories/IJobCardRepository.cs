using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Repositories
{
    public interface IJobCardRepository : IBaseRepository<JobCard>
    {
        Task<List<JobCard>> GetActiveAsync();
        Task SaveChangesAsync();
    }
}
