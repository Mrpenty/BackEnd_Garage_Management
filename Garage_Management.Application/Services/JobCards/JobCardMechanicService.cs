using Garage_Management.Application.DTOs.JobCardMechanics;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Services.JobCard;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Garage_Management.Application.Services.JobCards
{
    public class JobCardMechanicService : IJobCardMechanicService
    {
        private readonly IJobCardMechanicRepository _repository;
        private readonly IJobCardRepository _jobCardRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JobCardMechanicService(
            IJobCardMechanicRepository repository,
            IJobCardRepository jobCardRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _jobCardRepository = jobCardRepository;
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

            if (currentUserId <= 0)
                return ApiResponse<PagedResult<JobCardMechanicDto>>.ErrorResponse("EmployeeId không hợp lệ");

            var result = await _repository.GetJobCardsByEmployeeIdAsync(employeeId, param);
            if (result == null || result.Total == 0)
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

            var employeeId = int.Parse(employeeIdClaim);

            var jobCardMechanic = await _repository.GetByIdsAsync(jobCardId, employeeId);
            if (jobCardMechanic == null)
                return ApiResponse<JobCardMechanicDto>.ErrorResponse("Không tìm thấy phân công thợ này");

            var success = await _repository.UpdateStatusAsync(jobCardId, employeeId, dto.NewStatus);
            if (!success)
                return ApiResponse<JobCardMechanicDto>.ErrorResponse("Cập nhật trạng thái thất bại");

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
                JobCardDescription = updated.JobCard.Note,
            };

            return ApiResponse<JobCardMechanicDto>.SuccessResponse(dtoResult, "Cập nhật trạng thái thành công");
        }

        public async Task<ApiResponse<JobCardMechanicDto>> StartInspectionAsync(StartInspectionDto dto, CancellationToken ct)
        {
            if (dto.JobCardId <= 0)
                return ApiResponse<JobCardMechanicDto>.ErrorResponse("JobCardId không hợp lệ");

            if (dto.MechanicId <= 0)
                return ApiResponse<JobCardMechanicDto>.ErrorResponse("MechanicId không hợp lệ");

            var mechanicBusy = _repository.GetAll()
                .Any(x => x.EmployeeId == dto.MechanicId
                    && x.JobCardId != dto.JobCardId
                    && (x.Status == MechanicAssignmentStatus.InProgress || x.Status == MechanicAssignmentStatus.OnHold));

            if (mechanicBusy)
                return ApiResponse<JobCardMechanicDto>.ErrorResponse("Thợ đang bận sửa xe khác, không thể yêu cầu sửa jobCard này");

            var jobCard = await _jobCardRepository.GetByIdAsync(dto.JobCardId);
            if (jobCard == null)
                return ApiResponse<JobCardMechanicDto>.ErrorResponse("Không tìm thấy phiếu sửa chữa");

            var assignment = await _repository.GetByIdsAsync(dto.JobCardId, dto.MechanicId);
            if (assignment == null)
                return ApiResponse<JobCardMechanicDto>.ErrorResponse("Không tìm thấy phân công thợ này");

            var now = DateTime.UtcNow;
            assignment.Status = MechanicAssignmentStatus.InProgress;
            assignment.StartedAt ??= now;
            jobCard.Status = JobCardStatus.Inspection;

            _repository.Update(assignment);
            _jobCardRepository.Update(jobCard);
            await _jobCardRepository.SaveAsync(ct);

            var result = new JobCardMechanicDto
            {
                JobCardId = assignment.JobCardId,
                EmployeeId = assignment.EmployeeId,
                MechanicAssignmenStatus = assignment.Status,
                AssignedAt = assignment.AssignedAt,
                StartedAt = assignment.StartedAt,
                CompletedAt = assignment.CompletedAt,
                Note = assignment.Note,
                JobCardDescription = jobCard.Note
            };

            return ApiResponse<JobCardMechanicDto>.SuccessResponse(result, "Yêu cầu bắt đầu kiểm tra thành công");
        }
    }
}
