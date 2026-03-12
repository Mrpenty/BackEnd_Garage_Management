using Garage_Management.Application.DTOs.JobCard;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Application.Repositories.JobCards;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;
using JobCardServiceEntity = Garage_Management.Base.Entities.JobCards.JobCardService;



namespace Garage_Management.Application.Services.JobCards
{
    public class JobCardService : IJobCardService
    {
        private readonly IJobCardRepository _repository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IJobCardServiceRepository _jobCardServiceRepository;
        private readonly IJobCardSparePartRepository _jobCardSparePartRepository;
        private readonly IWorkBayRepository _workBayRepository;
        private readonly IAppointmentRepository _appointmentRepository;


        public JobCardService(
            IJobCardRepository repository,
            IServiceRepository serviceRepository,
            IInventoryRepository inventoryRepository,
            IJobCardServiceRepository jobCardServiceRepository,
            IJobCardSparePartRepository jobCardSparePartRepository,
            IWorkBayRepository workBayRepository,
            IAppointmentRepository appointmentRepository)
        {
            _repository = repository;
            _serviceRepository = serviceRepository;
            _inventoryRepository = inventoryRepository;
            _jobCardServiceRepository = jobCardServiceRepository;
            _jobCardSparePartRepository = jobCardSparePartRepository;
            _workBayRepository = workBayRepository;
            _appointmentRepository = appointmentRepository;
        }


        public async Task<JobCardDto?> CreateAsync(CreateJobCardDto dto ,int currentUserId, CancellationToken cancellationToken)
        {
            //  CHECK 1: Appointment đã có JobCard chưa
            var hasJobCard = await _repository.HasJobCardByAppointmentIdAsync(dto.AppointmentId);

            if (hasJobCard)
                throw new Exception("Appointment already has a JobCard.");

            //  CHECK 2: Xe đã có JobCard active chưa
            var hasActive = await _repository.HasActiveJobCardAsync(dto.VehicleId);

            if (hasActive)
                throw new Exception("Vehicle already has an active JobCard.");

            var app = await _appointmentRepository.GetByIdAsync((int)dto.AppointmentId);

            if (app == null)
                throw new Exception("Appointment not found.");

            // CHECK 4: status
            if (app.Status != AppointmentStatus.Confirmed)
                throw new Exception("Appointment must be confirmed.");

            var entity = new JobCard
            {
                AppointmentId = dto.AppointmentId,
                CustomerId = dto.CustomerId,
                VehicleId = dto.VehicleId,
                Note = dto.Note,
                SupervisorId = dto.SupervisorId,
                StartDate = DateTime.UtcNow,
                Status = JobCardStatus.Created,
                CreatedBy = currentUserId
            };

            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveAsync(cancellationToken);


            // 🔹 ADD SERVICES
            if (dto.Services != null && dto.Services.Any())
            {
                foreach (var service in dto.Services)
                {
                    await AddServiceAsync(entity.JobCardId, service, cancellationToken);
                }
            }

            // ❗ UPDATE APPOINTMENT STATUS
            if (dto.AppointmentId.HasValue)
            {
                await _appointmentRepository.UpdateStatusAsync(
                    dto.AppointmentId.Value,
                    AppointmentStatus.ConvertedToJobCard,
                    cancellationToken);
            }

            return new JobCardDto
            {
                JobCardId = entity.JobCardId,
                AppointmentId = entity.AppointmentId,
                CustomerId = entity.CustomerId,
                VehicleId = entity.VehicleId,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Status = entity.Status,
                Service = entity.Services,
                Note = entity.Note,
                SupervisorId = entity.SupervisorId
            };
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
                Service = entity.Services,
                Note = entity.Note,
                SupervisorId = entity.SupervisorId,
                CreatedByEmployeeId = entity.CreatedBy
            };
        }
        public async Task<bool> UpdateStatusAsync(int id, JobCardStatus status, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.Status = status;

            if (status == JobCardStatus.Completed)
                entity.EndDate = DateTime.UtcNow;

            _repository.Update(entity);
            await _repository.SaveAsync(cancellationToken);

            return true;
        }
        public async Task<bool> AssignMechanicAsync(int jobCardId, AssignMechanicDto dto, CancellationToken cancellationToken)
        {
            var jobCard = await _repository.GetAll()
                .Include(x => x.Mechanics)
                .FirstOrDefaultAsync(x => x.JobCardId == jobCardId);

            if (jobCard == null)
                return false;

            if (jobCard.Mechanics.Any(x => x.EmployeeId == dto.MechanicId))
                return true;

            jobCard.Mechanics.Add(new JobCardMechanic
            {
                JobCardId = jobCardId,
                EmployeeId = dto.MechanicId,
                AssignedAt = DateTime.UtcNow,
                Note = dto.Note,
                Status = (MechanicAssignmentStatus)1
            });
            jobCard.Status = JobCardStatus.WaitingMechanic;
            _repository.Update(jobCard);
            await _repository.SaveAsync(cancellationToken);

            return true;
        }
        public async Task<IEnumerable<JobCardListDto>> GetActiveAsync(
        string? search,
        string? sortBy,
        string? sortDirection)
        {
            var data = await _repository.GetActiveAsync(search, sortBy, sortDirection);

            return data.Select(x => new JobCardListDto
            {
                JobCardId = x.JobCardId,
                CustomerId = x.CustomerId,
                CustomerName = x.Customer.FirstName + " " + x.Customer.LastName,

                VehicleId = x.VehicleId,
                LicensePlate = x.Vehicle.LicensePlate,
                Service = x.Services,
                StartDate = x.StartDate,
                Status = x.Status
            });
        }
        public async Task<bool> UpdateAsync(
            int id,
            UpdateJobCardDto dto,
            CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(id);

            if (entity == null || entity.DeletedAt != null)
                return false;

            // Chỉ update nếu có giá trị truyền vào
            if (dto.Note != null)
                entity.Note = dto.Note;

            if (dto.SupervisorId.HasValue)
                entity.SupervisorId = dto.SupervisorId.Value;

            if (dto.EndDate.HasValue)
                entity.EndDate = dto.EndDate.Value;

            await _repository.SaveAsync(cancellationToken);

            return true;
        }
        public async Task<bool> AddServiceAsync(
     int jobCardId,
     AddServiceToJobCardDto dto,
     CancellationToken cancellationToken)
        {
            // 1️⃣ Kiểm tra JobCard tồn tại
            var jobCard = await _repository.GetByIdAsync(jobCardId);
            if (jobCard == null) return false;

            // 2️⃣ Lấy Service + ServiceTasks
            var service = await _serviceRepository
                .GetAll()
                .Include(x => x.ServiceTasks)
                .FirstOrDefaultAsync(x => x.ServiceId == dto.ServiceId, cancellationToken);

            if (service == null) return false;

            // 3️⃣ Tạo JobCardService (entity)
            var jobCardService = new JobCardServiceEntity
            {
                JobCardId = jobCardId,
                ServiceId = service.ServiceId,
                Description = dto.Description,
                Price = service.BasePrice,
                Status = ServiceStatus.Pending
            };

            // 4️⃣ Tự động tạo JobCardServiceTask
            foreach (var task in service.ServiceTasks.OrderBy(x => x.TaskOrder))
            {
                jobCardService.ServiceTasks.Add(new JobCardServiceTask
                {
                    JobCardId = jobCardId,
                    ServiceTaskId = task.ServiceTaskId,
                    TaskOrder = task.TaskOrder,
                    Status = ServiceStatus.Pending,
                });
            }

            // 5️⃣ Lưu
            await _jobCardServiceRepository.AddAsync(jobCardService, cancellationToken);
            await _jobCardServiceRepository.SaveAsync(cancellationToken);

            return true;
        }
        public async Task<bool> AddSparePartAsync(
    int jobCardId,
    AddSparePartToJobCardDto dto,
    CancellationToken cancellationToken)
        {
            // 1️⃣ Kiểm tra JobCard
            var jobCard = await _repository.GetByIdAsync(jobCardId);
            if (jobCard == null) return false;

            // 2️⃣ Lấy Inventory theo SparePartId
            var inventory = await _inventoryRepository
                .Query()
                .FirstOrDefaultAsync(x => x.SparePartId == dto.SparePartId, cancellationToken);

            if (inventory == null) return false;

            if (dto.Quantity <= 0) return false;

            // 3️⃣ Lấy giá bán (nullable → decimal)
            var unitPrice = inventory.SellingPrice ?? 0m;

            var entity = new JobCardSparePart
            {
                JobCardId = jobCardId,
                SparePartId = dto.SparePartId,
                Quantity = dto.Quantity,
                UnitPrice = unitPrice,
                TotalAmount = unitPrice * dto.Quantity,
                IsUnderWarranty = dto.IsUnderWarranty,
                Note = dto.Note,
                CreatedAt = DateTime.UtcNow
            };

            await _jobCardSparePartRepository.AddAsync(entity, cancellationToken);
            await _jobCardSparePartRepository.SaveAsync(cancellationToken);

            return true;
        }

        public async Task<bool> AssignWorkBayAsync(
    AssignWorkBayRequestDto dto,
    CancellationToken cancellationToken)
        {
            var jobCard = await _repository.GetByIdAsync(dto.JobCardId);
            if (jobCard == null)
                return false;

            var workBay = await _workBayRepository.GetByIdAsync(dto.WorkBayId);
            if (workBay == null)
                return false;

            if (workBay.JobcardId != null)
                return false; // bay already occupied

            workBay.JobcardId = dto.JobCardId;
            workBay.StartAt = DateTime.UtcNow;
            workBay.Status  = WorkBayStatus.Occupied;
            workBay.UpdateAt = DateTime.UtcNow;

            await _workBayRepository.SaveAsync(cancellationToken);

            return true;
        }


        public async Task<bool> ReleaseWorkBayAsync(
    ReleaseWorkBayDto dto,
    CancellationToken cancellationToken)
        {
            var workBay = await _workBayRepository.GetByIdAsync(dto.WorkBayId);
            if (workBay == null)
                return false;

            workBay.JobcardId = null;
            workBay.EndAt = DateTime.UtcNow;
            workBay.Status = WorkBayStatus.Available;
            workBay.UpdateAt = DateTime.UtcNow;

            await _workBayRepository.SaveAsync(cancellationToken);

            return true;
        }

        public async Task<List<JobcardListBySupervisor>> GetJobCardsBySupervisorIdAsync(int supervisorId)
        {
            var jobCards = await _repository.GetBySupervisorIdAsync(supervisorId);

            return jobCards.Select(x => new JobcardListBySupervisor
            {
                JobCardId = x.JobCardId,
                AppointmentId = x.AppointmentId,
                Appointment = x.Appointment == null ? null : new
                {
                    x.Appointment.AppointmentDateTime
                },
                CustomerId = x.CustomerId,
                Customer = x.Customer == null ? null : new
                {
                    x.Customer.FirstName,
                    x.Customer.LastName,
                },

                VehicleId = x.VehicleId,
                Vehicle = x.Vehicle == null ? null : new
                {
                  
                    x.Vehicle.LicensePlate
                },

                StartDate = x.StartDate,
                EndDate = x.EndDate,

                Status = (int)x.Status,
                Note = x.Note,

                SupervisorId = x.SupervisorId ?? 0,
                Supervisor = x.Supervisor == null ? null : new
                {
                    x.Supervisor.FullName
                },

                Services = x.Services
                    .Select(s => new
                        {
                         s.ServiceId,
                         ServiceName = s.Service.ServiceName,
        
    })
    .Cast<object>()
    .ToList()
            }).ToList();
        }

    }
}
