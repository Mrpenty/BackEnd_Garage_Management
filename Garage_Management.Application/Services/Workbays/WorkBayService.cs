using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.DTOs.Workbays;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Enums;

namespace Garage_Management.Application.Services.Workbays
{
    public class WorkBayService : IWorkBayService
    {
        private readonly IWorkBayRepository _workBayRepository;
        private readonly IJobCardRepository _jobCardRepository;

        public WorkBayService(
            IWorkBayRepository workBayRepository,
            IJobCardRepository jobCardRepository)
        {
            _workBayRepository = workBayRepository;
            _jobCardRepository = jobCardRepository;
        }

        public async Task<List<WorkBayDto>> GetListAsync(
            WorkBayStatus? status,
            CancellationToken cancellationToken)
        {
            var bays = await _workBayRepository.GetByStatusAsync(status, cancellationToken);

            var bayIds = bays.Select(x => x.Id).ToList();

            var allJobs = await _jobCardRepository
                .GetByWorkBayIdsAsync(bayIds, cancellationToken);

            var result = bays.Select(bay => new WorkBayDto
            {
                Id = bay.Id,
                Name = bay.Name,
                Note = bay.Note,
                Status = bay.Status,
                JobcardId = bay.JobcardId,
                CreateAt = bay.CreateAt,
                StartAt = bay.StartAt,
                EndAt = bay.EndAt,

                JobCards = allJobs
                    .Where(j => j.WorkBayId == bay.Id)
                    .Select(j => new JobCardListDto
                    {
                        JobCardId = j.JobCardId,
                        Status = j.Status,
                        StartDate = j.StartDate
                    }).ToList()
            }).ToList();

            return result;
        }

        public async Task<List<JobCardListDto>?> GetJobCardsByWorkBayAsync(
            int workBayId,
            CancellationToken cancellationToken)
        {
            var bay = await _workBayRepository.GetByIdAsync(workBayId);
            if (bay == null)
                return null;

            var jobs = await _jobCardRepository
                .GetByWorkBayIdAsync(workBayId, cancellationToken);

            return jobs.Select(j => new JobCardListDto
            {
                JobCardId = j.JobCardId,
                Status = j.Status,
                StartDate = j.StartDate
            }).ToList();
        }
    }
}