using Garage_Management.Application.DTOs.JobCard;
using Garage_Management.Base.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Garage_Management.Application.Services.JobCards
{
    public interface IJobCardService
    {
        Task<JobCardDto> CreateAsync(CreateJobCardDto dto, CancellationToken cancellationToken);
        Task<IEnumerable<JobCardDto>> GetAllAsync();
        Task<JobCardDto?> GetByIdAsync(int id);
        Task<bool> UpdateStatusAsync(int id, ServiceStatus status, CancellationToken cancellationToken);

    }

}
