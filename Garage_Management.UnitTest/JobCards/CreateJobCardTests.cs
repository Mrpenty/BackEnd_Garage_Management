using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Format;
using Garage_Management.Base.Entities.Accounts;
using Microsoft.AspNetCore.Http;
using Moq;
using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;
using JobCardServiceApp = Garage_Management.Application.Services.JobCards.JobCardService;
namespace Garage_Management.UnitTest.JobCards
{
    [TestClass]
    public class CreateJobCardTests
    {
        private Mock<IJobCardRepository> _jobCardRepo;
        private Mock<IServiceRepository> _serviceRepo;
        private Mock<IInventoryRepository> _inventoryRepo;
        private Mock<IJobCardServiceRepository> _jobCardServiceRepo;
        private Mock<IJobCardSparePartRepository> _jobCardSparePartRepo;
        private Mock<IWorkBayRepository> _workBayRepo;
        private Mock<IAppointmentRepository> _appointmentRepo;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Mock<ProgressCalculator> _progressCalculator;
        private JobCardServiceApp _service;

        [TestInitialize]
        public void Setup()
        {
            _jobCardRepo = new Mock<IJobCardRepository>();
            _serviceRepo = new Mock<IServiceRepository>();
            _inventoryRepo = new Mock<IInventoryRepository>();
            _jobCardServiceRepo = new Mock<IJobCardServiceRepository>();
            _jobCardSparePartRepo = new Mock<IJobCardSparePartRepository>();
            _workBayRepo = new Mock<IWorkBayRepository>();
            _appointmentRepo = new Mock<IAppointmentRepository>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _progressCalculator = new Mock<ProgressCalculator>();
            _service = new JobCardServiceApp(
                _jobCardRepo.Object,
                _serviceRepo.Object,
                _inventoryRepo.Object,
                _jobCardServiceRepo.Object,
                _jobCardSparePartRepo.Object,
                _workBayRepo.Object,
                _appointmentRepo.Object,
                 _httpContextAccessor.Object
                 , _progressCalculator.Object
            );
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenAppointmentAlreadyHasJobCard()
        {
            var dto = new CreateJobCardDto
            {
                AppointmentId = 1,
                CustomerId = 1,
                VehicleId = 1
            };

            _jobCardRepo
                .Setup(x => x.HasJobCardByAppointmentIdAsync(1))
                .ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<Exception>(
                () => _service.CreateAsync(dto, 42, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenVehicleHasActiveJobCard()
        {
            var dto = new CreateJobCardDto
            {
                AppointmentId = 2,
                CustomerId = 2,
                VehicleId = 5
            };

            _jobCardRepo
                .Setup(x => x.HasJobCardByAppointmentIdAsync(2))
                .ReturnsAsync(false);

            _jobCardRepo
                .Setup(x => x.HasActiveJobCardAsync(5))
                .ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<Exception>(
                () => _service.CreateAsync(dto, 1, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_ReturnsDto_WhenValid()
        {
            var dto = new CreateJobCardDto
            {
                AppointmentId = 20,
                CustomerId = 1,
                VehicleId = 15,
                Note = "test",
                SupervisorId = 2
            };

            JobCardEntity? captured = null;

            _jobCardRepo
                .Setup(x => x.HasJobCardByAppointmentIdAsync(20))
                .ReturnsAsync(false);

            _jobCardRepo
                .Setup(x => x.HasActiveJobCardAsync(15))
                .ReturnsAsync(false);

            _jobCardRepo
                .Setup(x => x.AddAsync(It.IsAny<JobCardEntity>(), It.IsAny<CancellationToken>()))
                .Callback<JobCardEntity, CancellationToken>((entity, _) =>
                {
                    entity.JobCardId = 1;
                    captured = entity;
                })
                .Returns(Task.CompletedTask);

            _jobCardRepo
      .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
      .ReturnsAsync(1);
            _appointmentRepo
    .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
    .ReturnsAsync(new Appointment
    {
        AppointmentId = 20,
        Status = AppointmentStatus.Confirmed,
        AppointmentDateTime = DateTime.UtcNow.AddMinutes(10)
    });
            var result = await _service.CreateAsync(dto, 1, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(dto.AppointmentId, result.AppointmentId);
            Assert.AreEqual(dto.CustomerId, result.CustomerId);
            Assert.AreEqual(dto.VehicleId, result.VehicleId);
            Assert.AreEqual(0m, captured?.QueueOrder);
            Assert.AreEqual(0m, result.QueueOrder);
        }
        [TestMethod]
        public async Task UpdateStatusAsync_ShouldNotSetEndDate_WhenStatusNotCompleted()
        {
            var jobCard = new JobCardEntity
            {
                JobCardId = 1,
                Status = JobCardStatus.Created
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(jobCard);

            await _service.UpdateStatusAsync(
                1,
                JobCardStatus.InProgress,
                CancellationToken.None);

            Assert.IsNull(jobCard.EndDate);
        }
        [TestMethod]
        public async Task UpdateAsync_ShouldIgnoreNullFields()
        {
            var jobCard = new JobCardEntity
            {
                JobCardId = 1,
                Note = "old"
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(jobCard);

            await _service.UpdateAsync(
                1,
                new UpdateJobCardDto(),
                CancellationToken.None);

            Assert.AreEqual("old", jobCard.Note);
        }
        
        [TestMethod]
        public async Task CreateAsync_Throws_WhenAppointmentNotConfirmed()
        {
            var dto = new CreateJobCardDto
            {
                AppointmentId = 6,
                CustomerId = 1,
                VehicleId = 2
            };

            _jobCardRepo.Setup(x => x.HasJobCardByAppointmentIdAsync(6))
                .ReturnsAsync(false);

            _jobCardRepo.Setup(x => x.HasActiveJobCardAsync(2))
                .ReturnsAsync(false);

            var appointment = new Appointment
            {
                AppointmentId = 6,
                Status = AppointmentStatus.Pending,
                AppointmentDateTime = DateTime.UtcNow.AddMinutes(10)
            };

            _appointmentRepo.Setup(x => x.GetByIdAsync(6))
                .ReturnsAsync(appointment);

            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _service.CreateAsync(dto, 1, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenAppointmentIsEarlierThanGraceWindow()
        {
            var dto = new CreateJobCardDto
            {
                AppointmentId = 8,
                CustomerId = 1,
                VehicleId = 2
            };

            _jobCardRepo.Setup(x => x.HasJobCardByAppointmentIdAsync(8))
                .ReturnsAsync(false);

            _jobCardRepo.Setup(x => x.HasActiveJobCardAsync(2))
                .ReturnsAsync(false);

            _appointmentRepo.Setup(x => x.GetByIdAsync(8))
                .ReturnsAsync(new Appointment
                {
                    AppointmentId = 8,
                    Status = AppointmentStatus.Confirmed,
                    AppointmentDateTime = DateTime.UtcNow.AddMinutes(31)
                });

            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _service.CreateAsync(dto, 1, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_AllowsCreation_WhenAppointmentIsWithinThirtyMinutes()
        {
            var dto = new CreateJobCardDto
            {
                AppointmentId = 9,
                CustomerId = 3,
                VehicleId = 4
            };

            _jobCardRepo.Setup(x => x.HasJobCardByAppointmentIdAsync(9))
                .ReturnsAsync(false);

            _jobCardRepo.Setup(x => x.HasActiveJobCardAsync(4))
                .ReturnsAsync(false);

            _jobCardRepo
                .Setup(x => x.AddAsync(It.IsAny<JobCardEntity>(), It.IsAny<CancellationToken>()))
                .Callback<JobCardEntity, CancellationToken>((entity, _) => entity.JobCardId = 99)
                .Returns(Task.CompletedTask);

            _jobCardRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _appointmentRepo.Setup(x => x.GetByIdAsync(9))
                .ReturnsAsync(new Appointment
                {
                    AppointmentId = 9,
                    Status = AppointmentStatus.Confirmed,
                    AppointmentDateTime = DateTime.UtcNow.AddMinutes(30)
                });

            var result = await _service.CreateAsync(dto, 1, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(9, result.AppointmentId);
        }

        [TestMethod]
        public async Task CreateAsync_AllowsCreation_WhenAppointmentIsLateWithinTwoHours()
        {
            var dto = new CreateJobCardDto
            {
                AppointmentId = 11,
                CustomerId = 3,
                VehicleId = 4
            };

            _jobCardRepo.Setup(x => x.HasJobCardByAppointmentIdAsync(11))
                .ReturnsAsync(false);

            _jobCardRepo.Setup(x => x.HasActiveJobCardAsync(4))
                .ReturnsAsync(false);

            _jobCardRepo
                .Setup(x => x.AddAsync(It.IsAny<JobCardEntity>(), It.IsAny<CancellationToken>()))
                .Callback<JobCardEntity, CancellationToken>((entity, _) => entity.JobCardId = 100)
                .Returns(Task.CompletedTask);

            _jobCardRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _appointmentRepo.Setup(x => x.GetByIdAsync(11))
                .ReturnsAsync(new Appointment
                {
                    AppointmentId = 11,
                    Status = AppointmentStatus.Confirmed,
                    AppointmentDateTime = DateTime.UtcNow.AddMinutes(-120)
                });

            var result = await _service.CreateAsync(dto, 1, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(11, result.AppointmentId);
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenAppointmentIsLateBeyondTwoHours()
        {
            var dto = new CreateJobCardDto
            {
                AppointmentId = 12,
                CustomerId = 1,
                VehicleId = 2
            };

            _jobCardRepo.Setup(x => x.HasJobCardByAppointmentIdAsync(12))
                .ReturnsAsync(false);

            _jobCardRepo.Setup(x => x.HasActiveJobCardAsync(2))
                .ReturnsAsync(false);

            _appointmentRepo.Setup(x => x.GetByIdAsync(12))
                .ReturnsAsync(new Appointment
                {
                    AppointmentId = 12,
                    Status = AppointmentStatus.Confirmed,
                    AppointmentDateTime = DateTime.UtcNow.AddMinutes(-121)
                });

            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _service.CreateAsync(dto, 1, CancellationToken.None));
        }
    }
}
