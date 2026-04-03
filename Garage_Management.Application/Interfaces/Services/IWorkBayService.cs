using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.DTOs.Workbays;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Services
{
    public interface IWorkBayService
    {
        Task<List<WorkBayDto>> GetListAsync(WorkBayStatus? status, CancellationToken cancellationToken);
        /// <summary>
        /// tạo khoang sửa chữa mới
        /// </summary>
        Task<ApiResponse<WorkBayDto>> CreateWorkBayAsync(CreateWorkBayRequest request, CancellationToken cancellationToken);
        /// <summary>
        /// Câp nhật thông tin khoang sửa chữa
        /// </summary>
        Task<ApiResponse<WorkBayDto>> UpdateWorkBayAsync(int id,UpdateWorkBayRequest request, CancellationToken cancellationToken);
        Task<WorkBayDto?> GetByIdAsync(int workBayId, CancellationToken cancellationToken);
    }
}
