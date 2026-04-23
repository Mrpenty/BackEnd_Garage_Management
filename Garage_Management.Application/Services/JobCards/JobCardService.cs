using Garage_Management.Application.DTOs.Appointments;
using Garage_Management.Application.DTOs.JobCardMechanics;
using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.DTOs.JobCardServices;
using Garage_Management.Application.DTOs.Services;
using Garage_Management.Application.DTOs.User;
using Garage_Management.Application.DTOs.Vehicles;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Services.JobCard;
using Garage_Management.Application.Repositories.JobCards;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Format;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using JobCardServiceEntity = Garage_Management.Base.Entities.JobCards.JobCardService;



namespace Garage_Management.Application.Services.JobCards
{
    public class JobCardService : IJobCardService
    {
        private const int AppointmentEarlyCreationGraceMinutes = 30;
        private const int AppointmentLateCreationGraceMinutes = 120;
        private static readonly TimeZoneInfo GarageTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        private readonly IJobCardRepository _repository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IJobCardServiceRepository _jobCardServiceRepository;
        private readonly IJobCardSparePartRepository _jobCardSparePartRepository;
        private readonly IWorkBayRepository _workBayRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProgressCalculator _progressCalculator;
        private readonly ILogger<JobCardService> _logger;



        public JobCardService(
            IJobCardRepository repository,
            IServiceRepository serviceRepository,
            IInventoryRepository inventoryRepository,
            IJobCardServiceRepository jobCardServiceRepository,
            IJobCardSparePartRepository jobCardSparePartRepository,
            IWorkBayRepository workBayRepository,
            IAppointmentRepository appointmentRepository,
            IHttpContextAccessor httpContext,
            ProgressCalculator progressCalculator,
            ILogger<JobCardService> logger)
        {
            _repository = repository;
            _serviceRepository = serviceRepository;
            _inventoryRepository = inventoryRepository;
            _jobCardServiceRepository = jobCardServiceRepository;
            _jobCardSparePartRepository = jobCardSparePartRepository;
            _workBayRepository = workBayRepository;
            _appointmentRepository = appointmentRepository;
            _httpContextAccessor = httpContext;
            _progressCalculator = progressCalculator;
            _logger = logger;
        }

        public JobCardService(
            IJobCardRepository repository,
            IServiceRepository serviceRepository,
            IInventoryRepository inventoryRepository,
            IJobCardServiceRepository jobCardServiceRepository,
            IJobCardSparePartRepository jobCardSparePartRepository,
            IWorkBayRepository workBayRepository,
            IAppointmentRepository appointmentRepository,
            IHttpContextAccessor httpContext,
            ProgressCalculator progressCalculator)
            : this(
                repository,
                serviceRepository,
                inventoryRepository,
                jobCardServiceRepository,
                jobCardSparePartRepository,
                workBayRepository,
                appointmentRepository,
                httpContext,
                progressCalculator,
                NullLogger<JobCardService>.Instance)
        {
        }


        private int ResolveBranchIdFromContext()
        {
            var ctx = _httpContextAccessor.HttpContext;
            var branchClaim = ctx?.User?.FindFirst("BranchId")?.Value;
            if (int.TryParse(branchClaim, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var branchId) && branchId > 0)
                return branchId;
            throw new InvalidOperationException("Không xác định được chi nhánh cho JobCard (thiếu claim BranchId).");
        }

        public async Task<JobCardDto?> CreateAsync(CreateJobCardDto dto, int currentUserId, CancellationToken cancellationToken)
        {
           // ❗ CHECK 1: Appointment đã có JobCard chưa
           if (dto.AppointmentId.HasValue)
           {
               var hasJobCard = await _repository.HasJobCardByAppointmentIdAsync(dto.AppointmentId);

               if (hasJobCard)
                   throw new Exception("Appointment này đã được tạo JobCard.");
           }

           // CHECK 2
           var hasActive = await _repository.HasActiveJobCardAsync(dto.VehicleId);

           if (hasActive)
               throw new Exception("Xe này đang có JobCard đang hoạt động.");

            Appointment? app = null;
            if (dto.AppointmentId.HasValue)
            {
                app = await _appointmentRepository.GetByIdAsync(dto.AppointmentId.Value);

                if (app == null)
                {
                    throw new Exception("Không tìm thấy lịch hẹn.");
                }

                // CHECK 4: status
                if (app.Status != AppointmentStatus.Confirmed)
                {
                    throw new Exception("Lịch hẹn đang ở trạng thái không phù hợp.");
                }

                var currentGarageTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GarageTimeZone);
                var earliestAllowedTime = app.AppointmentDateTime.AddMinutes(-AppointmentEarlyCreationGraceMinutes);
                var latestAllowedTime = app.AppointmentDateTime.AddMinutes(AppointmentLateCreationGraceMinutes);

                if (currentGarageTime < earliestAllowedTime)
                {
                    throw new Exception($"Chỉ được tạo JobCard trong vòng {AppointmentEarlyCreationGraceMinutes} phút trước giờ hẹn.");
                }

                if (currentGarageTime > latestAllowedTime)
                {
                    throw new Exception($"Lịch hẹn đã quá giờ tiếp nhận {AppointmentLateCreationGraceMinutes} phút. Vui lòng tạo lịch hẹn mới hoặc tiếp nhận dưới dạng walk-in.");
                }
            }
            var resolvedBranchId = app?.BranchId ?? ResolveBranchIdFromContext();

            var entity = new JobCard
           {
               BranchId = resolvedBranchId,
               AppointmentId = dto.AppointmentId,
               CustomerId = dto.CustomerId,
               VehicleId = dto.VehicleId,
               Note = dto.Note,
               SupervisorId = dto.SupervisorId,
               StartDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GarageTimeZone),
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
                    var added = await AddServiceAsync(entity.JobCardId, service, cancellationToken);
                    if (!added)
                        throw new InvalidOperationException($"Service {service.ServiceId} không tồn tại.");
                }
            }

            if (dto.AppointmentId.HasValue)
            {
                // Khi tạo JobCard thành công, chuyển Appointment sang trạng thái "Đã chuyển thành phiếu sửa chữa" (=3).
                var convertedStatus = AppointmentStatus.ConvertedToJobCard;
                await _appointmentRepository.UpdateStatusAsync(
                    dto.AppointmentId.Value,
                    convertedStatus,
                    cancellationToken);
            }

            return new JobCardDto
            {
                JobCardId = entity.JobCardId,
                AppointmentId = entity.AppointmentId,
                CustomerId = entity.CustomerId,
                VehicleId = entity.VehicleId,
                QueueOrder = entity.QueueOrder,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Status = entity.Status,
                Services = entity.Services.Select(MapJobCardService).ToList(),
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
                CustomerName = entity.Customer != null? $"{entity.Customer.LastName} {entity.Customer.FirstName}".Trim(): null,
                VehicleId = entity.VehicleId,
                CustomerPhone = entity.Customer?.User.PhoneNumber,
                Vehicles = entity.Vehicle != null? new List<VehicleDto>{
                new VehicleDto
                {
                    VehicleId = entity.Vehicle.VehicleId,
                    LicensePlate = entity.Vehicle.LicensePlate ?? "",
                    Brand = entity.Vehicle.Brand?.BrandName,
                    Model = entity.Vehicle.Model?.ModelName,
                    Year = entity.Vehicle.Year
                }}: new List<VehicleDto>(),
                WorkbayId = entity.WorkBay?.Id,
                QueueOrder = entity.QueueOrder,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Status = entity.Status,
                ProgressPercentage = entity.ProgressPercentage,
                CompletedSteps = entity.CompletedSteps,
                ProgressNotes = entity.ProgressNotes,
                Services = entity.Services.Where(s => s.DeletedAt == null).Select(MapJobCardService).ToList(),
                Note = entity.Note,
                SupervisorId = entity.SupervisorId,
                SupervisorName = entity.Supervisor != null ? $"{entity.Supervisor.LastName} {entity.Supervisor.FirstName}".Trim() : null,
                CreatedByEmployeeId = entity.CreatedBy,

                Mechanics = entity.Mechanics.Select(m => new JobCardMechanicView
                {
                    MechanicId = m.EmployeeId,
                    MechanicName = m.Employee != null ? $"{m.Employee.LastName} {m.Employee.FirstName}".Trim() : "Không rõ",
                    AssignedAt = m.AssignedAt,
                    StartedAt = m.StartedAt,
                    CompletedAt = m.CompletedAt,
                }).ToList(),
                SpareParts = entity.SpareParts.Select(sp => new JobCardSparePartView
                {
                   SparePartId = sp.SparePartId,
                   SparePartName = sp.Inventory.PartName,
                   Quantity = sp.Quantity,
                   UnitPrice = sp.UnitPrice,
                   TotalAmount = sp.TotalAmount,
                   IsUnderWarranty= sp.IsUnderWarranty,
                   Note = sp.Note,
                   CreatedAt = sp.CreatedAt,

                }).ToList() ?? new List<JobCardSparePartView>()
            };
        }

        private static JobCardServiceResponse MapJobCardService(JobCardServiceEntity service)
        {
            
            return new JobCardServiceResponse
            {
                JobCardServiceId = service.JobCardServiceId,
                JobCardId = service.JobCardId,
                ServiceId = service.ServiceId,
                ServiceName = service.Service?.ServiceName,
                Description = service.Description,
                Price = service.Price, // Removed '?? 0' because 'service.Price' is already decimal (non-nullable)
                Status = service.Status,
                SourceInspectionItemId = service.SourceInspectionItemId,
                CreatedAt = service.CreatedAt,
                UpdatedAt = service.UpdatedAt,

                ServiceTasks = service.ServiceTasks?
                    .Select(t => new JobCardServiceTaskDto
                    {
                        JobCardServiceTaskId = t.JobCardServiceTaskId,
                        TaskName = t.ServiceTask?.TaskName ?? "Không có tên công việc",
                        Status = t.Status
                    })
                    .OrderBy(t => t.JobCardServiceTaskId)
                    .ToList() ?? new List<JobCardServiceTaskDto>()
            };
        }
        public async Task<bool> UpdateStatusAsync(int id, JobCardStatus status, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            entity.Status = status;

            if (status == JobCardStatus.Completed)
                entity.EndDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            _repository.Update(entity);
            await _repository.SaveAsync(cancellationToken);

            return true;
        }
        public async Task<bool> AssignMechanicAsync(int jobCardId, AssignMechanicDto dto, CancellationToken cancellationToken)
        {
            var jobCard = await _repository.GetWithMechanicsAsync(jobCardId);

            if (jobCard == null)
            {
                _logger.LogWarning("Phân công kỹ thuật viên thất bại: không tìm thấy JobCardId {JobCardId}.", jobCardId);
                return false;
            }

            if (jobCard.Mechanics.Any(x => x.EmployeeId == dto.MechanicId))
            {
                _logger.LogInformation(
                    "Phân công kỹ thuật viên bị bỏ qua: JobCardId {JobCardId} đã có MechanicId {MechanicId}.",
                    jobCardId,
                    dto.MechanicId);
                return true;
            }

            jobCard.Mechanics.Add(new JobCardMechanic
            {
                JobCardId = jobCardId,
                EmployeeId = dto.MechanicId,
                AssignedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                Note = dto.Note,
                Status = MechanicAssignmentStatus.Assigned
            });

            jobCard.Status = JobCardStatus.WaitingInspection;

            _repository.Update(jobCard);
            await _repository.SaveAsync(cancellationToken);

            _logger.LogInformation(
                "Phân công kỹ thuật viên thành công: JobCardId {JobCardId} đã gán MechanicId {MechanicId} và chuyển trạng thái sang {Status}.",
                jobCardId,
                dto.MechanicId,
                jobCard.Status);

            return true;
        }
        public async Task<PagedResult<JobCardListDto>> GetActiveAsync(string? search, string? sortBy,string? sortDirection, int page,int pageSize)
        {
            var (jobCards, totalCount) = await _repository.GetActiveAsync(
                search, sortBy, sortDirection, page, pageSize);

            var result = jobCards.Select(x => new JobCardListDto
            {
                JobCardId = x.JobCardId,

                CustomerId = x.CustomerId,
                CustomerName = x.Customer.LastName + " " + x.Customer.FirstName,
                QueueOrder = x.QueueOrder,
                CustomerPhone = x.Customer.User.PhoneNumber,

                Vehicle = new VehicleListDto
                {
                    VehicleId = x.VehicleId,
                    BrandName = x.Vehicle.Brand.BrandName,
                    ModelName = x.Vehicle.Model.ModelName,
                    LicensePlate = x.Vehicle.LicensePlate
                },

                Status = x.Status,
                StartDate = x.StartDate,
                Mechanics = x.Mechanics.Select(m => new JobCardMechanicView
                {
                    MechanicId = m.EmployeeId,
                    MechanicName = m.Employee != null ? $"{m.Employee.FirstName} {m.Employee.LastName}".Trim() : "Không rõ",
                    AssignedAt = m.AssignedAt,
                    StartedAt = m.StartedAt,
                    CompletedAt = m.CompletedAt,
                }).ToList(),
                Services = x.Services.Select(s => new ServiceResponse
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.Service.ServiceName
                }).ToList()

            }).ToList();

            return new PagedResult<JobCardListDto>
            {
                PageData = result,
                Page = page,
                PageSize = pageSize,
                Total = totalCount
            };
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
        public async Task<bool> AddServiceAsync(int jobCardId, AddServiceToJobCardDto dto, CancellationToken cancellationToken)
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
            if (!service.BasePrice.HasValue || service.BasePrice.Value <= 0)
                throw new InvalidOperationException("Dịch vụ chưa có giá, không thể thêm vào JobCard");

            // 3️⃣ Tạo JobCardService (entity)
            var jobCardService = new JobCardServiceEntity
            {
                JobCardId = jobCardId,
                ServiceId = service.ServiceId,
                Description = dto.Description,
                Price = service.BasePrice.Value,
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


        public async Task<bool> AssignWorkBayAsync(AssignWorkBayRequestDto dto, CancellationToken cancellationToken)
        {
            var jobCard = await _repository.GetByIdAsync(dto.JobCardId);
            if (jobCard == null)
                return false;

            var previousWorkBayId = jobCard.WorkBayId;

            var workBay = await _workBayRepository.GetByIdAsync(dto.WorkBayId);
            if (workBay == null)
                return false;

            // cho phép gán dù bay đang bận
            jobCard.WorkBayId = workBay.Id;
            if (previousWorkBayId != dto.WorkBayId || jobCard.QueueOrder <= 0)
            {
                jobCard.QueueOrder = ((await _repository.GetMaxQueueOrderByWorkBayAsync(dto.WorkBayId, cancellationToken)) ?? 0m) + 1000m;
            }

            // nếu bay đang trống → bắt đầu luôn
            if (workBay.JobcardId == null)
            {
                workBay.JobcardId = jobCard.JobCardId;
                workBay.StartAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                workBay.Status = WorkBayStatus.Occupied;

                jobCard.Status = JobCardStatus.OnwaitingList;
            }
            else
            {
                // nếu bay bận → đưa vào hàng chờ
                jobCard.Status = JobCardStatus.OnwaitingList;
            }

            jobCard.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            workBay.UpdateAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            await _workBayRepository.SaveAsync(cancellationToken);
            await _repository.SaveAsync(cancellationToken);

            return true;
        }

        public async Task<bool> ReorderWorkBayQueueAsync(
     ReorderJobCardQueueDto dto,
     CancellationToken cancellationToken)
        {
            if (dto.JobCardId <= 0 || dto.WorkBayId <= 0)
                throw new Exception("JobCardId hoặc WorkBayId không hợp lệ.");

            if (dto.PreviousJobCardId == dto.JobCardId || dto.NextJobCardId == dto.JobCardId)
                throw new Exception("Previous/Next không được trùng với JobCardId.");

            if (dto.PreviousJobCardId.HasValue &&
                dto.NextJobCardId.HasValue &&
                dto.PreviousJobCardId.Value == dto.NextJobCardId.Value)
            {
                throw new Exception("PreviousJobCardId và NextJobCardId không được trùng nhau.");
            }

            var jobCard = await _repository.GetByIdAsync(dto.JobCardId);
            if (jobCard == null)
                throw new Exception("JobCard không tồn tại.");

            if (jobCard.WorkBayId != dto.WorkBayId)
                throw new Exception("JobCard không thuộc WorkBay này.");

            var jobsInWorkBay = await _repository.GetTrackedByWorkBayIdAsync(dto.WorkBayId, cancellationToken);
            if (!jobsInWorkBay.Any())
                throw new Exception("WorkBay không có JobCard nào.");

            var otherJobs = jobsInWorkBay
                .Where(x => x.JobCardId != dto.JobCardId)
                .ToList();

            decimal newQueueOrder;

            if (dto.PreviousJobCardId.HasValue && dto.NextJobCardId.HasValue)
            {
                var previousJob = otherJobs.FirstOrDefault(x => x.JobCardId == dto.PreviousJobCardId.Value);
                var nextJob = otherJobs.FirstOrDefault(x => x.JobCardId == dto.NextJobCardId.Value);

                if (previousJob == null)
                    throw new Exception("PreviousJobCard không tồn tại trong WorkBay.");

                if (nextJob == null)
                    throw new Exception("NextJobCard không tồn tại trong WorkBay.");

                if (previousJob.QueueOrder >= nextJob.QueueOrder)
                    throw new Exception("Thứ tự Queue không hợp lệ: Previous >= Next.");

                var gap = nextJob.QueueOrder - previousJob.QueueOrder;
                if (gap <= 0.000001m)
                    throw new Exception("Khoảng cách QueueOrder quá nhỏ, cần re-index.");

                newQueueOrder = (previousJob.QueueOrder + nextJob.QueueOrder) / 2m;
            }
            else if (dto.NextJobCardId.HasValue)
            {
                var nextJob = otherJobs.FirstOrDefault(x => x.JobCardId == dto.NextJobCardId.Value);
                if (nextJob == null)
                    throw new Exception("NextJobCard không tồn tại trong WorkBay.");

                newQueueOrder = otherJobs.Any()
                    ? otherJobs.Min(x => x.QueueOrder) - 1000m
                    : 1000m;
            }
            else if (dto.PreviousJobCardId.HasValue)
            {
                var previousJob = otherJobs.FirstOrDefault(x => x.JobCardId == dto.PreviousJobCardId.Value);
                if (previousJob == null)
                    throw new Exception("PreviousJobCard không tồn tại trong WorkBay.");

                newQueueOrder = otherJobs.Any()
                    ? otherJobs.Max(x => x.QueueOrder) + 1000m
                    : 1000m;
            }
            else
            {
                throw new Exception("Phải cung cấp PreviousJobCardId hoặc NextJobCardId.");
            }

            jobCard.QueueOrder = newQueueOrder;
            jobCard.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

            await _repository.SaveAsync(cancellationToken);
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
            workBay.EndAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            workBay.Status = WorkBayStatus.Available;
            workBay.UpdateAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

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
                         EstimateMinute = s.ServiceTasks.Sum(st => st.ServiceTask.EstimateMinute)

                    })
    .Cast<object>()
    .ToList(),
                
                TotalEstimateMinute = x.Services.SelectMany(s => s.ServiceTasks)
                    .Sum(st => st.ServiceTask.EstimateMinute)
            }).ToList();
        }

        public async Task<List<JobCardDto>> GetJobCardsByCustomerIdAsync(int customerId)
        {
            var jobCards = await _repository.GetByCustomerIdAsync(customerId);

            return jobCards.Select(x => new JobCardDto
            {
                JobCardId = x.JobCardId,
                AppointmentId = x.AppointmentId,
                CustomerId = x.CustomerId,
                VehicleId = x.VehicleId,
                QueueOrder = x.QueueOrder,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Status = x.Status,
                ProgressPercentage = x.ProgressPercentage,
                CompletedSteps = x.CompletedSteps,
                ProgressNotes = x.ProgressNotes,
                Services = x.Services.Select(MapJobCardService).ToList(),
                Note = x.Note,
                SupervisorId = x.SupervisorId,
                CreatedByEmployeeId = x.CreatedBy,
                Mechanics = x.Mechanics.Select(m => new JobCardMechanicView
                {
                    MechanicId = m.EmployeeId,
                    MechanicName = m.Employee != null ? $"{m.Employee.FirstName} {m.Employee.LastName}".Trim() : "Không rõ",
                    AssignedAt = m.AssignedAt,
                    StartedAt = m.StartedAt,
                    CompletedAt = m.CompletedAt,
                }).ToList(),
            }).ToList();
        }

        public async Task<ApiResponse<UpdateProgressResponse>> UpdateRepairProgressAsync(int jobCardId, UpdateJobCardProgressDto dto, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || !httpContext.User.Identity?.IsAuthenticated == true)
            {
                return ApiResponse<UpdateProgressResponse>.ErrorResponse("Vui lòng đăng nhập để cập nhật tiến độ");
            }

            var currentUserId = int.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var employeeIdClaim = httpContext.User.FindFirst("EmployeeId")
               ?? httpContext.User.FindFirst(ClaimTypes.NameIdentifier);

            if (employeeIdClaim == null || !int.TryParse(employeeIdClaim.Value, out int currentEmployeeId))
            {
                return ApiResponse<UpdateProgressResponse>.ErrorResponse("Không thể xác định EmployeeId từ token");
            }


            // Kiểm tra JobCard tồn tại
            var jobCard = await _repository.GetByIdAsync(jobCardId);
            if (jobCard == null)
                return ApiResponse<UpdateProgressResponse>.ErrorResponse("Không tìm thấy phiếu sửa chữa");

            bool isMechanic = jobCard.Mechanics.Any(m => m.EmployeeId == currentEmployeeId);
            bool isSupervisor = jobCard.SupervisorId == currentEmployeeId;

            if (!isMechanic && !isSupervisor)
            {
                return ApiResponse<UpdateProgressResponse>.ErrorResponse($"Bạn không có quyền cập nhật tiến độ phiếu sửa chữa này. EmployeeId: {currentEmployeeId}, JobCardId: {jobCardId}");
            }

            // Cập nhật các trường progress
            if (dto.StatusJobCard.HasValue)
                jobCard.Status = dto.StatusJobCard.Value;

            if (!string.IsNullOrEmpty(dto.ProgressNotes))
                jobCard.ProgressNotes = dto.ProgressNotes;

            // Nếu có faults mới, thêm vào ProgressNotes hoặc tạo notification
            if (!string.IsNullOrEmpty(dto.AdditionalFaults))
            {
                // Thêm vào ProgressNotes
                jobCard.ProgressNotes = string.IsNullOrEmpty(jobCard.ProgressNotes)
                    ? $"Phát hiện lỗi mới: {dto.AdditionalFaults}"
                    : $"{jobCard.ProgressNotes}\nPhát hiện lỗi mới: {dto.AdditionalFaults}";

                // TODO: Tạo notification cho Supervisor
                // Có thể inject INotificationService và gọi CreateNotificationAsync(jobCard.SupervisorId, "Đã phát hiện lỗi mới", dto.AdditionalFaults);
            }

            // Cập nhật trạng thái các services 
            if (dto.ServiceUpdates != null && dto.ServiceUpdates.Any())
            {
                foreach (var serviceUpdate in dto.ServiceUpdates)
                {
                    var jobCardService = jobCard.Services.FirstOrDefault(s => s.JobCardServiceId == serviceUpdate.JobCardServiceId);
                    if (jobCardService != null)
                    {
                        jobCardService.Status = serviceUpdate.StatusService;
                        jobCardService.UpdatedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                        if (jobCardService.Status == ServiceStatus.Completed)
                        {
                            foreach (var task in jobCardService.ServiceTasks)
                            {
                                if (task.Status != ServiceStatus.Completed)
                                {
                                    task.Status = ServiceStatus.Completed;
                                    task.CompletedAt ??= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));   
                                    task.PerformedById = currentEmployeeId;
                                }
                            }
                        }

                    }
                }
            }
            // Cập nhật trạng thái các service tasks
            if (dto.ServiceTaskUpdates != null && dto.ServiceTaskUpdates.Any())
            {
                foreach (var taskUpdate in dto.ServiceTaskUpdates)
                {
                    // Tìm JobCardServiceTask theo ServiceTaskId
                    var jobCardServiceTask = jobCard.Services
                        .SelectMany(s => s.ServiceTasks)
                        .FirstOrDefault(t => t.JobCardServiceTaskId == taskUpdate.JobCardServiceTaskId);

                    if (jobCardServiceTask != null)
                    {
                        jobCardServiceTask.Status = taskUpdate.StatusServiceTask;
                        // Cập nhật thời gian bắt đầu và hoàn thành dựa trên trạng thái
                        if (taskUpdate.StatusServiceTask == ServiceStatus.InProgress && !jobCardServiceTask.StartedAt.HasValue)
                        {
                            jobCardServiceTask.StartedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                        }
                        else if (taskUpdate.StatusServiceTask == ServiceStatus.Completed && !jobCardServiceTask.CompletedAt.HasValue)
                        {
                            jobCardServiceTask.CompletedAt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                        }
                        jobCardServiceTask.PerformedById = currentEmployeeId;
                    }
                }
            }
            // Tính ProgressPercentage theo logic mới
            jobCard.ProgressPercentage = _progressCalculator.CalculateJobCardProgress(jobCard);
            // Ưu tiên giá trị người dùng truyền vào (nếu có)
            if (dto.ProgressPercentage.HasValue)
                jobCard.ProgressPercentage = Math.Clamp(dto.ProgressPercentage.Value, 0, 100);
            // Kiểm tra nếu tất cả services đã completed, thì đánh dấu JobCard completed
            bool allServicesCompleted = jobCard.Services.All(s => s.Status == ServiceStatus.Completed);
            bool allTasksCompleted = jobCard.Services
                .SelectMany(s => s.ServiceTasks)
                .All(t => t.Status == ServiceStatus.Completed);

            if (allServicesCompleted && allTasksCompleted)
            {
                jobCard.Status = JobCardStatus.Completed;
                jobCard.EndDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
                jobCard.ProgressPercentage = 100;
            }

            _repository.Update(jobCard);
            await _repository.SaveAsync(cancellationToken);

            // Tạo response
            var response = new UpdateProgressResponse
            {
                JobCardId = jobCard.JobCardId,
                StatusJobCard = jobCard.Status,
                StatusJobCardName = jobCard.Status.ToString(),
                ProgressPercentage = jobCard.ProgressPercentage,
                ProgressNotes = jobCard.ProgressNotes,
                EndDate = jobCard.EndDate,
                Services = jobCard.Services.Select(s => new JobCardServiceProgressDto
                {
                    JobCardServiceId = s.JobCardServiceId,
                    ServiceId = s.ServiceId,
                    ServiceName = s.Service?.ServiceName ?? "Không rõ dịch vụ",
                    StatusService = s.Status,
                    StatusName = s.Status.ToString(),
                    Description = s.Description,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    ServiceTasks = s.ServiceTasks.Select(t => new JobCardServiceTaskProgressDto
                    {
                        JobCardServiceTaskId = t.JobCardServiceTaskId,
                        ServiceTaskId = t.ServiceTaskId,
                        StatusServiceTask = t.Status,
                        ServiceTaskName = t.Status.ToString(),
                        StartedAt = t.StartedAt,
                        CompletedAt = t.CompletedAt
                    }).ToList()
                }).ToList()
                
            };

            return ApiResponse<UpdateProgressResponse>.SuccessResponse(response, "Cập nhật tiến độ thành công.");
        }

        public async Task<ApiResponse<ViewRepairProgressResponse>> ViewRepairProgressAsync(int jobCardId, CancellationToken cancellationToken)
        {
            var formatdate = new FormatDateTime();
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || !httpContext.User.Identity?.IsAuthenticated == true)
            {
                return ApiResponse<ViewRepairProgressResponse>.ErrorResponse("Vui lòng đăng nhập để cập nhật tiến độ");
            }
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var customerIdClaim = httpContext.User.FindFirst("CustomerId")?.Value;
            var employeeIdClaim = httpContext.User.FindFirst("EmployeeId")?.Value;
            int? employeeId = null;

            if (!string.IsNullOrEmpty(employeeIdClaim) && int.TryParse(employeeIdClaim, out int empId))
            {
                employeeId = empId;
            }


            var userRoleClaim = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRoleClaim))
            {
                return ApiResponse<ViewRepairProgressResponse>.ErrorResponse("Thông tin người dùng không hợp lệ");
            }
            var userId = int.Parse(userIdClaim);
            string userRole = userRoleClaim;

            // Lấy JobCard với đầy đủ thông tin, bao gồm ServiceTasks
            var jobCard = await _repository.GetByIdAsync(jobCardId);
            if (jobCard == null)
                return ApiResponse<ViewRepairProgressResponse>.ErrorResponse("Không tìm thấy phiếu sửa chữa.");

            // Kiểm tra permissions
            bool hasPermission = userRole switch
            {
                "Customer" => jobCard.Customer?.UserId == userId,
                "Mechanic" => jobCard.Mechanics?.Any(m => m.EmployeeId == employeeId) == true,
                "Supervisor" or "Admin" => true,
                _ => false
            };

            if (!hasPermission)
            {
                return ApiResponse<ViewRepairProgressResponse>.ErrorResponse("Bạn không có quyền xem phiếu sửa chữa này.");
            }

            // Tính tổng thời gian ước tính còn lại
            long totalRemainingMinutes = 0;
            var services = new List<ServiceProgressDto>();

            foreach (var service in jobCard.Services)
            {
                var tasks = service.ServiceTasks.Select(t => new TaskProgressDto
                {
                    JobCardServiceTaskId = t.JobCardServiceTaskId,
                    TaskName = t.ServiceTask?.TaskName ?? "Không rõ công việc",
                    Status = t.Status,
                    ServiceTaskStatusName = t.Status.ToString(),
                    EstimateMinute = t.ServiceTask?.EstimateMinute ?? 0,
                    StartedAt = t.StartedAt,
                    CompletedAt = t.CompletedAt
                }).ToList();

                var remainingMinutes = tasks.Where(t => t.Status != ServiceStatus.Completed).Sum(t => (long)t.EstimateMinute);
                totalRemainingMinutes += remainingMinutes;

                services.Add(new ServiceProgressDto
                {
                    JobCardServiceId = service.JobCardServiceId,
                    ServiceId = service.ServiceId,
                    ServiceName = service.Service?.ServiceName ?? "Không rõ dịch vụ",
                    Status = service.Status,
                    ServiceStatusName = service.Status.ToString(),
                    Description = service.Description,
                    EstimatedMinutesRemaining = remainingMinutes,
                    Tasks = tasks
                });
            }

            int bufferMinutes = 5;
            // Tạo response
            var response = new ViewRepairProgressResponse
            {
                JobCardId = jobCard.JobCardId,
                CustomerName = jobCard.Customer != null ? $"{jobCard.Customer.FirstName} {jobCard.Customer.LastName}" : "Không rõ khách hàng",
                CustomerPhoneNumber = jobCard.Customer?.User.PhoneNumber,
                VehicleBrand = jobCard.Vehicle?.Brand.BrandName ?? "Không rõ hãng xe",
                VehicleModel = jobCard.Vehicle?.Model.ModelName ?? "Không rõ mẫu xe",
                VehicleLicensePlate = jobCard.Vehicle?.LicensePlate ?? "Không rõ biển số",
                Status = jobCard.Status,
                StatusJobCardName = jobCard.Status.ToString(),
                ProgressPercentage = jobCard.ProgressPercentage,
               // CompletedSteps = jobCard.CompletedSteps,
                ProgressNotes = jobCard.ProgressNotes,
                StartDate = jobCard.StartDate,
                EndDate = jobCard.EndDate,
                EstimatedCompletionTime = _progressCalculator.CalculateEstimatedCompletionDisplay(jobCard.StartDate, totalRemainingMinutes, bufferMinutes),
                EstimatedJobCardMinutesRemaining = totalRemainingMinutes,
                Services = services,
                AssignedMechanic = jobCard.Mechanics.Any() ? string.Join(", ", jobCard.Mechanics.Select(m => m.Employee?.FullName ?? "Không rõ")) : null,
                Supervisor = jobCard.Supervisor?.FullName
            };

            // Nếu là Customer, ẩn internal details
            if (userRole == "Customer")
            {
                response.ProgressNotes = null;
            }

            return ApiResponse<ViewRepairProgressResponse>.SuccessResponse(response, "Lấy thông tin tiến độ thành công.");
        }      
    }
}
