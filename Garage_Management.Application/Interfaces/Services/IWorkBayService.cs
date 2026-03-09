using Garage_Management.Application.DTOs.Workbays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Services
{
    public interface IWorkBayService
    {
        Task<List<WorkBayDto>> GetListAsync(CancellationToken cancellationToken);
    }
}
