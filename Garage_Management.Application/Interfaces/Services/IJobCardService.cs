using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data.Configurations.JobCards;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.JobCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Garage_Management.Application.Interfaces.Services
{   
    public interface IJobCardService
    {
        /// <summary>
        /// Tạo phiếu sửa chữa
        /// </summary>
        Task<JobCardDto> CreateAsync(CreateJobCardDto dto, int currentUserId, CancellationToken cancellationToken);

        /// <summary>
        /// Lấy chi tiết phiếu sửa chữa
        /// </summary>
        Task<JobCardDto?> GetByIdAsync(int id);
        /// <summary>
        /// Cập nhật trạng thái phiếu sửa chữa
        /// </summary>
        Task<bool> UpdateStatusAsync(int id, JobCardStatus status, CancellationToken cancellationToken);

        /// <summary>
        /// Cập nhật thông tin phiếu sửa chữa
        /// </summary>
        Task<bool> UpdateAsync(int id, UpdateJobCardDto dto, CancellationToken cancellationToken);

        /// <summary>
        /// Phân công kỹ thuật viên
        /// </summary>
        Task<bool> AssignMechanicAsync(int id, AssignMechanicDto dto, CancellationToken cancellationToken);
        Task<PagedResult<JobCardListDto>> GetActiveAsync(string? search,string? sortBy,string? sortDirection,int page, int pageSize);
        Task<bool> AddServiceAsync(int jobCardId, AddServiceToJobCardDto dto,CancellationToken cancellationToken);

        /// <summary>
        /// Thêm phụ tùng vào phiếu sửa chữa
        /// </summary>
        Task<bool> AddSparePartAsync(int jobCardId, AddSparePartToJobCardDto dto, CancellationToken cancellationToken);
        
        /// <summary>
        /// Phân công khoang sửa chữa
        /// </summary>
        Task<bool> AssignWorkBayAsync(AssignWorkBayRequestDto dto, CancellationToken cancellationToken);

        /// <summary>
        /// Giải phóng khoang sửa chữa
        /// </summary>
        Task<bool> ReleaseWorkBayAsync(ReleaseWorkBayDto dto, CancellationToken cancellationToken);

        Task<List<JobcardListBySupervisor>> GetJobCardsBySupervisorIdAsync(int supervisorId);
    }

}


