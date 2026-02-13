using Garage_Management.Application.DTOs.JobCard;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.JobCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Garage_Management.Application.Services.JobCards
{
    public class JobCardService : IJobCardService
    {
        private readonly IJobCardRepository _repository;

        public JobCardService(IJobCardRepository repository)
        {
            _repository = repository;
        }

        public async Task<JobCardDto> CreateAsync(CreateJobCardDto dto, CancellationToken cancellationToken)
        {
            var entity = new JobCard
            {
                AppointmentId = dto.AppointmentId,
                CustomerId = dto.CustomerId,
                VehicleId = dto.VehicleId,
                Note = dto.Note,
                SupervisorId = dto.SupervisorId,
                CreatedByEmployeeId = dto.CreatedByEmployeeId,
                StartDate = DateTime.UtcNow,
                Status = ServiceStatus.Pending
            };

            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveAsync(cancellationToken);

            return new JobCardDto
            {
                JobCardId = entity.JobCardId,
                AppointmentId = entity.AppointmentId,
                CustomerId = entity.CustomerId,
                VehicleId = entity.VehicleId,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Status = entity.Status,
                Note = entity.Note,
                SupervisorId = entity.SupervisorId,
                CreatedByEmployeeId = entity.CreatedByEmployeeId
            };
        }

        public async Task<IEnumerable<JobCardDto>> GetAllAsync()
        {
            return await _repository
                .GetAll()
                .Select(x => new JobCardDto
                {
                    JobCardId = x.JobCardId,
                    AppointmentId = x.AppointmentId,
                    CustomerId = x.CustomerId,
                    VehicleId = x.VehicleId,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Status = x.Status,
                    Note = x.Note,
                    SupervisorId = x.SupervisorId,
                    CreatedByEmployeeId = x.CreatedByEmployeeId
                })
                .ToListAsync();
        }


        public async Task<JobCardDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null) return null;

            return new JobCardDto
            {
                JobCardId = entity.JobCardId,
                AppointmentId = entity.AppointmentId,
                CustomerId = entity.CustomerId,
                VehicleId = entity.VehicleId,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Status = entity.Status,
                Note = entity.Note,
                SupervisorId = entity.SupervisorId,
                CreatedByEmployeeId = entity.CreatedByEmployeeId
            };
        }
        public async Task<bool> UpdateStatusAsync(int id, ServiceStatus status, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.Status = status;

            if (status == ServiceStatus.Completed)
                entity.EndDate = DateTime.UtcNow;

            _repository.Update(entity);
            await _repository.SaveAsync(cancellationToken);

            return true;
        }

    }


}
