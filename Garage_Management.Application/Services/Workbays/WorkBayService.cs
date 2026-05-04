using Garage_Management.Application.DTOs.JobCardMechanics;
using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.DTOs.Workbays;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Application.Interfaces.Services.Auth;
using Garage_Management.Application.Services.Auth;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.Workbays
{
    public class WorkBayService : IWorkBayService
    {
        private readonly IWorkBayRepository _workBayRepository;
        private readonly IJobCardRepository _jobCardRepository;
        private readonly ICurrentUserService _currentUser;

        public WorkBayService(
            IWorkBayRepository workBayRepository,
            IJobCardRepository jobCardRepository,
            ICurrentUserService currentUser)
        {
            _workBayRepository = workBayRepository;
            _jobCardRepository = jobCardRepository;
            _currentUser = currentUser;
        }

        public WorkBayService(
            IWorkBayRepository workBayRepository,
            IJobCardRepository jobCardRepository)
            : this(workBayRepository, jobCardRepository, new NullCurrentUserService())
        {
        }

        private int ResolveBranchIdForCreate(int? requestedBranchId)
        {
            if (_currentUser.IsAdmin())
            {
                if (requestedBranchId is not { } b || b <= 0)
                    throw new InvalidOperationException("Admin phải chỉ định BranchId khi tạo khoang sửa chữa");
                return b;
            }
            var scoped = _currentUser.GetCurrentBranchId();
            if (!scoped.HasValue)
                throw new UnauthorizedAccessException("Không xác định được chi nhánh từ tài khoản hiện tại");
            return scoped.Value;
        }

        private void EnsureCanAccess(int branchId)
        {
            if (_currentUser.IsAdmin()) return;
            var scoped = _currentUser.GetCurrentBranchId();
            if (scoped.HasValue && scoped.Value == branchId) return;
            throw new UnauthorizedAccessException("Không có quyền truy cập khoang của chi nhánh khác");
        }

        public async Task<List<WorkBayDto>> GetListAsync(
            WorkBayStatus? status,
            CancellationToken cancellationToken)
        {
            var bays = await _workBayRepository.GetByStatusAsync(status, cancellationToken);

            var bayIds = bays.Select(x => x.Id).ToList();

            var allJobs = await _jobCardRepository
                .GetByWorkBayIdsAsync(bayIds, cancellationToken);

            var result = bays
                .Select(bay => MapWorkBay(
                    bay,
                    allJobs.Where(j => j.WorkBayId == bay.Id)))
                .ToList();

            return result;
        }

        public async Task<WorkBayDto?> GetByIdAsync(
            int workBayId,
            CancellationToken cancellationToken)
        {
            var bay = await _workBayRepository.GetByIdAsync(workBayId);
            if (bay == null)
                return null;

            var jobs = await _jobCardRepository
                .GetByWorkBayIdAsync(workBayId, cancellationToken);

            return MapWorkBay(bay, jobs);
        }

        private static WorkBayDto MapWorkBay(
            Base.Entities.JobCards.WorkBay bay,
            IEnumerable<JobCard> jobs)
        {
            return new WorkBayDto
            {
                Id = bay.Id,
                Name = bay.Name,
                Note = bay.Note,
                Status = bay.Status,
                JobcardId = bay.JobcardId,
                CreateAt = bay.CreateAt,
                UpdateAt = bay.UpdateAt,
                StartAt = bay.StartAt,
                EndAt = bay.EndAt,
                JobCards = jobs
                    .OrderBy(x => x.QueueOrder)
                    .ThenBy(x => x.StartDate)
                    .ThenBy(x => x.JobCardId)
                    .Select(MapJobCard)
                    .ToList()
            };
        }

        private static JobCardListDto MapJobCard(JobCard jobCard)
        {
            return new JobCardListDto
            {
                JobCardId = jobCard.JobCardId,
                CustomerId = jobCard.CustomerId,
                CustomerName = $"{jobCard.Customer?.LastName} {jobCard.Customer?.FirstName}".Trim(),
                QueueOrder = jobCard.QueueOrder,
                Mechanics = jobCard.Mechanics
                    .Where(x => x.Employee != null)
                    .Select(x => new JobCardMechanicView
                    {
                        MechanicId = x.EmployeeId,
                        MechanicName = $"{x.Employee?.LastName} {x.Employee?.FirstName}".Trim(),
                        AssignedAt = x.AssignedAt,
                        StartedAt = x.StartedAt,
                        CompletedAt = x.CompletedAt,
                    })
                    .ToList(),


                Vehicle = jobCard.Vehicle == null
                    ? null!
                    : new DTOs.Vehicles.VehicleListDto
                    {
                        VehicleId = jobCard.Vehicle.VehicleId,
                        BrandName = jobCard.Vehicle.Brand?.BrandName ?? string.Empty,
                        ModelName = jobCard.Vehicle.Model?.ModelName ?? string.Empty,
                        LicensePlate = jobCard.Vehicle.LicensePlate
                    },
                Status = jobCard.Status,
                StartDate = jobCard.StartDate,
                AppointmentId = jobCard.AppointmentId,
                AppointmentDateTime = jobCard.Appointment?.AppointmentDateTime,
                Services = jobCard.Services
                    .Where(x => x.Service != null)
                    .Select(x => new DTOs.Services.ServiceResponse
                    {
                        ServiceId = x.Service.ServiceId,
                        ServiceName = x.Service.ServiceName,
                        BasePrice = x.Service.BasePrice,
                        Description = x.Service.Description
                    })
                    .ToList()
            };
        }

        public async Task<ApiResponse<RebalanceWorkBayQueueResponse>> RebalanceQueueAsync(
            int workBayId,
            CancellationToken cancellationToken)
        {
            var workBay = await _workBayRepository.GetByIdAsync(workBayId);
            if (workBay == null)
            {
                return ApiResponse<RebalanceWorkBayQueueResponse>.ErrorResponse("Khoang sửa chữa không tồn tại");
            }

            var jobs = await _jobCardRepository.GetTrackedByWorkBayIdAsync(workBayId, cancellationToken);

            decimal queueOrder = 1000m;
            foreach (var job in jobs)
            {
                job.QueueOrder = queueOrder;
                queueOrder += 1000m;
            }

            await _jobCardRepository.SaveAsync(cancellationToken);

            return ApiResponse<RebalanceWorkBayQueueResponse>.SuccessResponse(
                new RebalanceWorkBayQueueResponse
                {
                    WorkBayId = workBayId,
                    UpdatedCount = jobs.Count
                },
                "Rebalance queue thành công");
        }

        public async Task<ApiResponse<WorkBayDto>> CreateWorkBayAsync(CreateWorkBayRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                return ApiResponse<WorkBayDto>.ErrorResponse("Tên khoang sửa chữa không được để trống");
            }
            var branchId = ResolveBranchIdForCreate(request.BranchId);
            var workBay = new WorkBay
            {
                BranchId = branchId,
                Name = request.Name,
                Note = request.Note,
                Status = WorkBayStatus.Available,
                CreateAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
            };
            await _workBayRepository.AddAsync(workBay, cancellationToken);
            await _workBayRepository.SaveAsync(cancellationToken);
            return ApiResponse<WorkBayDto>.SuccessResponse(new WorkBayDto
            {
                Id = workBay.Id,
                BranchId = workBay.BranchId,
                Name = workBay.Name,
                Note = workBay.Note,
                Status = workBay.Status,
                JobcardId = workBay.JobcardId,
                CreateAt = workBay.CreateAt,
                StartAt = workBay.StartAt,
                EndAt = workBay.EndAt
            }, "Tạo khoang sửa chữa thành công");
        }

        public async Task<ApiResponse<object>> DeleteWorkBayAsync(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
                return ApiResponse<object>.ErrorResponse("Id không hợp lệ");

            var workBay = await _workBayRepository.GetByIdAsync(id);
            if (workBay == null)
                return ApiResponse<object>.ErrorResponse("Khoang sửa chữa không tồn tại");

            EnsureCanAccess(workBay.BranchId);

            if (workBay.Status == WorkBayStatus.Occupied || workBay.JobcardId.HasValue)
                return ApiResponse<object>.ErrorResponse("Không thể xóa khoang đang có phiếu sửa chữa");

            if (await _workBayRepository.HasJobCardsAsync(id, cancellationToken))
                return ApiResponse<object>.ErrorResponse("Không thể xóa khoang đã có lịch sử phiếu sửa chữa");

            _workBayRepository.Delete(workBay);
            await _workBayRepository.SaveAsync(cancellationToken);

            return ApiResponse<object>.SuccessResponse(new { }, "Xóa khoang sửa chữa thành công");
        }

        public async Task<ApiResponse<WorkBayDto>> UpdateWorkBayAsync(int id, UpdateWorkBayRequest request, CancellationToken cancellationToken)
        {
            var workBay = await _workBayRepository.GetByIdAsync(id);
            if (workBay == null)
            {
                return ApiResponse<WorkBayDto>.ErrorResponse("Khoang sửa chữa không tồn tại");
            }
            EnsureCanAccess(workBay.BranchId);
            workBay.Name = request.Name;
            workBay.Note = request.Note;
            workBay.Status = request.Status;
            if (request.Status == WorkBayStatus.Occupied)
            {
                workBay.StartAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                workBay.EndAt = null;
            }
            else if (request.Status == WorkBayStatus.Available)
            {
                workBay.EndAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                workBay.StartAt = null;
            }
             _workBayRepository.Update(workBay);
            await _workBayRepository.SaveAsync(cancellationToken);

            return ApiResponse<WorkBayDto>.SuccessResponse(new WorkBayDto
            {
                Id = workBay.Id,
                BranchId = workBay.BranchId,
                Name = workBay.Name,
                Note = workBay.Note,
                Status = workBay.Status,
                JobcardId = workBay.JobcardId,
                CreateAt = workBay.CreateAt,
                StartAt = workBay.StartAt,
                EndAt = workBay.EndAt
            },"Cập nhật khoang sửa chữa thành công");
        }
    }
}
