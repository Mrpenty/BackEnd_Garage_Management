using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Format;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.Services;
using Garage_Management.UnitTest.Helper;
using Microsoft.AspNetCore.Http;
using Moq;
using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;
using JobCardServiceApp = Garage_Management.Application.Services.JobCards.JobCardService;
using JobCardServiceEntity = Garage_Management.Base.Entities.JobCards.JobCardService;

namespace Garage_Management.UnitTest.JobCards
{
    [TestClass]
    public class CreateJobCardTests
    {
        private Mock<IJobCardRepository> _jobCardRepo = null!;
        private Mock<IServiceRepository> _serviceRepo = null!;
        private Mock<IInventoryRepository> _inventoryRepo = null!;
        private Mock<IJobCardServiceRepository> _jobCardServiceRepo = null!;
        private Mock<IJobCardSparePartRepository> _jobCardSparePartRepo = null!;
        private Mock<IWorkBayRepository> _workBayRepo = null!;
        private Mock<IAppointmentRepository> _appointmentRepo = null!;
        private Mock<IHttpContextAccessor> _httpContextAccessor = null!;
        private Mock<ProgressCalculator> _progressCalculator = null!;
        private JobCardServiceApp _service = null!;

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
                _httpContextAccessor.Object,
                _progressCalculator.Object);
        }

        private static DateTime GarageNow()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
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
            var now = GarageNow();
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
                .Setup(x => x.GetByIdAsync(20))
                .ReturnsAsync(new Appointment
                {
                    AppointmentId = 20,
                    Status = AppointmentStatus.Confirmed,
                    AppointmentDateTime = now.AddMinutes(10)
                });

            var result = await _service.CreateAsync(dto, 1, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(dto.AppointmentId, result.AppointmentId);
            Assert.AreEqual(dto.CustomerId, result.CustomerId);
            Assert.AreEqual(dto.VehicleId, result.VehicleId);
            Assert.AreEqual(0m, captured?.QueueOrder);
            Assert.AreEqual(0m, result.QueueOrder);
            _appointmentRepo.Verify(
                x => x.UpdateStatusAsync(20, AppointmentStatus.ConvertedToJobCard, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task CreateAsync_AllowsWalkIn_WhenAppointmentIsNull()
        {
            var dto = new CreateJobCardDto
            {
                AppointmentId = null,
                CustomerId = 2,
                VehicleId = 3,
                Note = "walk-in"
            };

            JobCardEntity? captured = null;

            _jobCardRepo
                .Setup(x => x.HasActiveJobCardAsync(3))
                .ReturnsAsync(false);

            _jobCardRepo
                .Setup(x => x.AddAsync(It.IsAny<JobCardEntity>(), It.IsAny<CancellationToken>()))
                .Callback<JobCardEntity, CancellationToken>((entity, _) =>
                {
                    entity.JobCardId = 101;
                    captured = entity;
                })
                .Returns(Task.CompletedTask);

            _jobCardRepo
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _service.CreateAsync(dto, 7, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsNull(result.AppointmentId);
            Assert.AreEqual(2, result.CustomerId);
            Assert.AreEqual(3, result.VehicleId);
            Assert.AreEqual("walk-in", captured?.Note);
            _appointmentRepo.Verify(
                x => x.GetByIdAsync(It.IsAny<int>()),
                Times.Never);
            _appointmentRepo.Verify(
                x => x.UpdateStatusAsync(It.IsAny<int>(), It.IsAny<AppointmentStatus>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenAppointmentNotFound()
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

            _appointmentRepo.Setup(x => x.GetByIdAsync(6))
                .ReturnsAsync((Appointment?)null);

            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _service.CreateAsync(dto, 1, CancellationToken.None));

            _appointmentRepo.Verify(
                x => x.UpdateStatusAsync(It.IsAny<int>(), It.IsAny<AppointmentStatus>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenAppointmentNotConfirmed()
        {
            var now = GarageNow();
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
                AppointmentDateTime = now.AddMinutes(10)
            };

            _appointmentRepo.Setup(x => x.GetByIdAsync(6))
                .ReturnsAsync(appointment);

            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _service.CreateAsync(dto, 1, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenAppointmentIsEarlierThanGraceWindow()
        {
            var now = GarageNow();
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
                    AppointmentDateTime = now.AddMinutes(31)
                });

            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _service.CreateAsync(dto, 1, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_AllowsCreation_WhenAppointmentIsWithinThirtyMinutes()
        {
            var now = GarageNow();
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
                    AppointmentDateTime = now.AddMinutes(30)
                });

            var result = await _service.CreateAsync(dto, 1, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(9, result.AppointmentId);
        }

        [TestMethod]
        public async Task CreateAsync_AllowsCreation_WhenAppointmentIsLateWithinTwoHours()
        {
            var now = GarageNow();
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
                    AppointmentDateTime = now.AddMinutes(-119)
                });

            var result = await _service.CreateAsync(dto, 1, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(11, result.AppointmentId);
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenAppointmentIsLateBeyondTwoHours()
        {
            var now = GarageNow();
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
                    AppointmentDateTime = now.AddMinutes(-121)
                });

            await Assert.ThrowsExceptionAsync<Exception>(() =>
                _service.CreateAsync(dto, 1, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_WithServices_CreatesJobCardServices_AndConvertsAppointment()
        {
            var now = GarageNow();
            var dto = new CreateJobCardDto
            {
                AppointmentId = 30,
                CustomerId = 9,
                VehicleId = 12,
                Note = "with services",
                Services =
                [
                    new AddServiceToJobCardDto
                    {
                        ServiceId = 501,
                        Description = "Oil change"
                    }
                ]
            };

            JobCardEntity? capturedJobCard = null;
            JobCardServiceEntity? capturedJobCardService = null;

            _jobCardRepo.Setup(x => x.HasJobCardByAppointmentIdAsync(30))
                .ReturnsAsync(false);
            _jobCardRepo.Setup(x => x.HasActiveJobCardAsync(12))
                .ReturnsAsync(false);

            _jobCardRepo
                .Setup(x => x.AddAsync(It.IsAny<JobCardEntity>(), It.IsAny<CancellationToken>()))
                .Callback<JobCardEntity, CancellationToken>((entity, _) =>
                {
                    entity.JobCardId = 200;
                    capturedJobCard = entity;
                })
                .Returns(Task.CompletedTask);

            _jobCardRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _jobCardRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(() => capturedJobCard);

            _serviceRepo.Setup(x => x.GetAll()).Returns(new TestAsyncEnumerable<Service>(
                new List<Service>
                {
                    new Service
                    {
                        ServiceId = 501,
                        BasePrice = 350000,
                        ServiceTasks =
                        [
                            new ServiceTask { ServiceTaskId = 2, TaskOrder = 2, TaskName = "Second task" },
                            new ServiceTask { ServiceTaskId = 1, TaskOrder = 1, TaskName = "First task" }
                        ]
                    }
                }.AsQueryable()));

            _jobCardServiceRepo
                .Setup(x => x.AddAsync(It.IsAny<JobCardServiceEntity>(), It.IsAny<CancellationToken>()))
                .Callback<JobCardServiceEntity, CancellationToken>((entity, _) =>
                {
                    capturedJobCardService = entity;
                    capturedJobCard?.Services.Add(entity);
                })
                .Returns(Task.CompletedTask);

            _jobCardServiceRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _appointmentRepo.Setup(x => x.GetByIdAsync(30))
                .ReturnsAsync(new Appointment
                {
                    AppointmentId = 30,
                    Status = AppointmentStatus.Confirmed,
                    AppointmentDateTime = now.AddMinutes(10)
                });

            var result = await _service.CreateAsync(dto, 5, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsNotNull(capturedJobCard);
            Assert.IsNotNull(capturedJobCardService);
            Assert.AreEqual(1, result.Services.Count);
            Assert.AreEqual(501, result.Services[0].ServiceId);
            Assert.AreEqual("Oil change", result.Services[0].Description);
            Assert.AreEqual(2, capturedJobCardService.ServiceTasks.Count);
            CollectionAssert.AreEqual(
                new[] { 1, 2 },
                capturedJobCardService.ServiceTasks.Select(x => x.TaskOrder).ToArray());
            _jobCardServiceRepo.Verify(
                x => x.AddAsync(It.IsAny<JobCardServiceEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _appointmentRepo.Verify(
                x => x.UpdateStatusAsync(30, AppointmentStatus.ConvertedToJobCard, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task CreateAsync_WithMissingService_Throws_WhenServiceDoesNotExist()
        {
            var now = GarageNow();
            var dto = new CreateJobCardDto
            {
                AppointmentId = 31,
                CustomerId = 10,
                VehicleId = 13,
                Services =
                [
                    new AddServiceToJobCardDto
                    {
                        ServiceId = 999,
                        Description = "Missing service"
                    }
                ]
            };

            JobCardEntity? capturedJobCard = null;

            _jobCardRepo.Setup(x => x.HasJobCardByAppointmentIdAsync(31))
                .ReturnsAsync(false);
            _jobCardRepo.Setup(x => x.HasActiveJobCardAsync(13))
                .ReturnsAsync(false);

            _jobCardRepo
                .Setup(x => x.AddAsync(It.IsAny<JobCardEntity>(), It.IsAny<CancellationToken>()))
                .Callback<JobCardEntity, CancellationToken>((entity, _) =>
                {
                    entity.JobCardId = 201;
                    capturedJobCard = entity;
                })
                .Returns(Task.CompletedTask);

            _jobCardRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _jobCardRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(() => capturedJobCard);

            _serviceRepo.Setup(x => x.GetAll()).Returns(new TestAsyncEnumerable<Service>(
                new List<Service>().AsQueryable()));

            _appointmentRepo.Setup(x => x.GetByIdAsync(31))
                .ReturnsAsync(new Appointment
                {
                    AppointmentId = 31,
                    Status = AppointmentStatus.Confirmed,
                    AppointmentDateTime = now.AddMinutes(10)
                });

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.CreateAsync(dto, 5, CancellationToken.None));

            StringAssert.Contains(ex.Message, "999");
            StringAssert.Contains(ex.Message, "không tồn tại");
            _jobCardServiceRepo.Verify(
                x => x.AddAsync(It.IsAny<JobCardServiceEntity>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _appointmentRepo.Verify(
                x => x.UpdateStatusAsync(31, AppointmentStatus.ConvertedToJobCard, It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
