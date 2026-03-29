using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Garage_Management.Application.Interfaces.Repositories
{
    public interface IWorkBayRepository
    {
        Task<WorkBay?> GetByIdAsync(int id);

            IQueryable<WorkBay> Query();

            Task AddAsync(WorkBay entity, CancellationToken cancellationToken);

            Task SaveAsync(CancellationToken cancellationToken);
            Task<List<WorkBay>> GetByStatusAsync( WorkBayStatus? status, CancellationToken cancellationToken);
        
    }
 }


