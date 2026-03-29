using Garage_Management.Application.DTOs.JobCardMechanics;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Repositories.JobCards
{
    public interface IJobCardMechanicRepository : IBaseRepository<JobCardMechanic>
    {
        Task<PagedResult<JobCardMechanicDto>> GetJobCardsByEmployeeIdAsync(int employeeId, ParamQuery param);
    }
}
