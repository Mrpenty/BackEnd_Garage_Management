using Garage_Management.Application.DTOs.JobCardMechanics;
using Garage_Management.Application.DTOs.Notifications;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Services.JobCard;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.JobCards
{
    public class JobCardMechanicService : IJobCardMechanicService
    {
        private readonly IJobCardMechanicRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JobCardMechanicService(IJobCardMechanicRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResponse<PagedResult<JobCardMechanicDto>>> GetJobCardsByEmployeeAsync(ParamQuery param, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return ApiResponse<PagedResult<JobCardMechanicDto>>.ErrorResponse("Vui lòng đăng nhập");

            }
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employeeIdClaim = httpContext.User.FindFirst("EmployeeId")?.Value;
            var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
            {
                return ApiResponse<PagedResult<JobCardMechanicDto>>.ErrorResponse("Thông tin người dùng không hợp lệ");
            }
            var currentUserId = int.Parse(userIdClaim);
            var employeeId = int.Parse(employeeIdClaim);
     
            // Validate
            if (currentUserId <= 0)
                return ApiResponse<PagedResult<JobCardMechanicDto>>.ErrorResponse("EmployeeId không hợp lệ");

            var result = await _repository.GetJobCardsByEmployeeIdAsync(employeeId, param);
            if(result == null || result.Total == 0)
                return ApiResponse<PagedResult<JobCardMechanicDto>>.ErrorResponse("Không tìm thấy phiếu sửa chữa nào cho thợ máy này");

            return ApiResponse<PagedResult<JobCardMechanicDto>>.SuccessResponse(result, "Lấy danh sách phiếu sửa chữa thành công");
        }
        public async Task<ApiResponse<JobCardMechanicDto>> UpdateStatusAsync(int jobCardId, UpdateJobCardMechanicStatusDto dto)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return ApiResponse<JobCardMechanicDto>.ErrorResponse("Vui lòng đăng nhập");

            }
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employeeIdClaim = httpContext.User.FindFirst("EmployeeId")?.Value;
            var roleClaim = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
            {
                return ApiResponse<JobCardMechanicDto>.ErrorResponse("Thông tin người dùng không hợp lệ");
            }
            var currentUserId = int.Parse(userIdClaim);
            var employeeId = int.Parse(employeeIdClaim);

            // Kiểm tra tồn tại
            var jobCardMechanic = await _repository.GetByIdsAsync(jobCardId, employeeId);
            if (jobCardMechanic == null)
                return ApiResponse<JobCardMechanicDto>.ErrorResponse("Không tìm thấy phân công thợ này");

            var httpContextc = _httpContextAccessor.HttpContext;

            // Thực hiện update
            bool success = await _repository.UpdateStatusAsync(jobCardId, employeeId, dto.NewStatus);
            if (!success)
                return ApiResponse<JobCardMechanicDto>.ErrorResponse("Cập nhật trạng thái thất bại");

            // Lấy lại dữ liệu sau khi update để trả về
            var updated = await _repository.GetByIdsAsync(jobCardId, employeeId);

            var dtoResult = new JobCardMechanicDto
            {
                JobCardId = updated.JobCardId,
                EmployeeId = updated.EmployeeId,
                MechanicAssignmenStatus = updated.Status,
                StartedAt = updated.StartedAt,
                CompletedAt = updated.CompletedAt,
                Note = updated.Note,
                AssignedAt = updated.AssignedAt,
                JobCardDescription= updated.JobCard.Note,

            };

            return ApiResponse<JobCardMechanicDto>.SuccessResponse(dtoResult, "Cập nhật trạng thái thành công");
        }

    }
}
