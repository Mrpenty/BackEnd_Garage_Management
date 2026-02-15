using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Repositories
{
    using Garage_Management.Base.Entities.Services;

    public interface IServiceRepository
    {
        Task<Service?> GetByIdAsync(int id);

        IQueryable<Service> Query();

        Task SaveAsync(CancellationToken cancellationToken);
    }

}
