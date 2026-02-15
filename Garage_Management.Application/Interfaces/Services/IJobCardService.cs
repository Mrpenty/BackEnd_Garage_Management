using Garage_Management.Application.DTOs.JobCard;
using Garage_Management.Base.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Garage_Management.Application.Interfaces.Services
{
    public interface IJobCardService
    {
        Task<JobCardDto> CreateAsync(CreateJobCardDto dto, CancellationToken cancellationToken);
        
        Task<JobCardDto?> GetByIdAsync(int id);
        Task<bool> UpdateStatusAsync(int id, ServiceStatus status, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(int id, UpdateJobCardDto dto, CancellationToken cancellationToken);

        Task<bool> AssignMechanicAsync(int id, AssignMechanicDto dto, CancellationToken cancellationToken);
        Task<IEnumerable<JobCardListDto>> GetActiveAsync();
        Task<bool> AddServiceAsync(int jobCardId, AddServiceToJobCardDto dto,CancellationToken cancellationToken);
 Task<bool> AddSparePartAsync( int jobCardId, AddSparePartToJobCardDto dto, CancellationToken cancellationToken);
        }

    }


