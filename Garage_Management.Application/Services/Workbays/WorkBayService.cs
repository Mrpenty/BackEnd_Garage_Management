using Garage_Management.Application.DTOs.Workbays;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services;
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

        public WorkBayService(IWorkBayRepository workBayRepository)
        {
            _workBayRepository = workBayRepository;
        }

        public async Task<List<WorkBayDto>> GetListAsync(WorkBayStatus? status, CancellationToken cancellationToken)
        {
            var query = _workBayRepository.Query();

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            return await query
                .Select(x => new WorkBayDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Note = x.Note,
                    Status = x.Status,
                    JobcardId = x.JobcardId,
                    CreateAt = x.CreateAt,
                    StartAt = x.StartAt,
                    EndAt = x.EndAt
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<ApiResponse<WorkBayDto>> CreateWorkBayAsync(CreateWorkBayRequest request, CancellationToken cancellationToken)
        {
            var workBay = new WorkBay
            {
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
                Name = workBay.Name,
                Note = workBay.Note,
                Status = workBay.Status,
                JobcardId = workBay.JobcardId,
                CreateAt = workBay.CreateAt,
                StartAt = workBay.StartAt,
                EndAt = workBay.EndAt
            }, "Tạo khoang sửa chữa thành công");
        }

        public async Task<ApiResponse<WorkBayDto>> UpdateWorkBayAsync(int id, UpdateWorkBayRequest request, CancellationToken cancellationToken)
        {
            var workBay = await _workBayRepository.GetByIdAsync(id);
            if (workBay == null)
            {
                return ApiResponse<WorkBayDto>.ErrorResponse("Khoang sửa chữa không tồn tại");
            }
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