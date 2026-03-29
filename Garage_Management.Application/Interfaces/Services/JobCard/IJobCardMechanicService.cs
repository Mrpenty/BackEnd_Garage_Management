using Garage_Management.Application.DTOs.JobCardMechanics;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Services.JobCard
{
    public interface IJobCardMechanicService 
    {
        /// <summary>
        /// Lấy danh sách JobCard của một nhân viên
        /// </summary>
        Task<ApiResponse<PagedResult<JobCardMechanicDto>>> GetJobCardsByEmployeeAsync(ParamQuery param, CancellationToken ct);
        Task<ApiResponse<JobCardMechanicDto>> UpdateStatusAsync(int jobCardId, UpdateJobCardMechanicStatusDto dto);

    }
}
