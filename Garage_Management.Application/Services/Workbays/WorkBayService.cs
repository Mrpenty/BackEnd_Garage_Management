using Garage_Management.Application.DTOs.JobCardMechanics;
using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.DTOs.Workbays;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.JobCards;

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
                JobCards = jobs.Select(MapJobCard).ToList()
            };
        }

        private static JobCardListDto MapJobCard(JobCard jobCard)
        {
            return new JobCardListDto
            {
                JobCardId = jobCard.JobCardId,
                CustomerId = jobCard.CustomerId,
                CustomerName = $"{jobCard.Customer?.FirstName} {jobCard.Customer?.LastName}".Trim(),
                Mechanics = jobCard.Mechanics
                    .Where(x => x.Employee != null)
                    .Select(x => new JobCardMechanicView
                    {
                        MechanicId = x.EmployeeId,
                        MechanicName = $"{x.Employee?.FirstName} {x.Employee?.LastName}".Trim(),
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
    }
}
