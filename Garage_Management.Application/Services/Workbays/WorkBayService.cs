using Garage_Management.Application.DTOs.Workbays;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services;
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

        public async Task<List<WorkBayDto>> GetListAsync(CancellationToken cancellationToken)
        {
            return await _workBayRepository
                .Query()
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
    }
}
