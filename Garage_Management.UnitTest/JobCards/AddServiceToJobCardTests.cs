using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.Services;
using Garage_Management.UnitTest.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using JobCardServiceApp = Garage_Management.Application.Services.JobCards.JobCardService;
using JobCardServiceEntity = Garage_Management.Base.Entities.JobCards.JobCardService;
using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;
using Microsoft.AspNetCore.Http;


namespace Garage_Management.UnitTest.JobCards
{
    [TestClass]
    public class JobCardServiceAddServiceTests
    {
        
        private Mock<IJobCardRepository> _jobCardRepo;
        private Mock<IServiceRepository> _serviceRepo;
        private Mock<IInventoryRepository> _inventoryRepo;
        private Mock<IJobCardServiceRepository> _jobCardServiceRepo;
        private Mock<IJobCardSparePartRepository> _jobCardSparePartRepo;
        private Mock<IWorkBayRepository> _workBayRepo;
        private Mock<IAppointmentRepository> _appointmentRepo;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
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
            _service = new JobCardServiceApp(
                _jobCardRepo.Object,
                _serviceRepo.Object,
                _inventoryRepo.Object,
                _jobCardServiceRepo.Object,
                _jobCardSparePartRepo.Object,
                _workBayRepo.Object,
                _appointmentRepo.Object,
                  _httpContextAccessor.Object
            );
        }

        [TestMethod]
        public async Task AddServiceAsync_WithValidData_ReturnsTrue()
        {
            var dto = new AddServiceToJobCardDto
            {
                ServiceId = 1,
                Description = "Test service"
            };

            var jobCards = new List<JobCardEntity>
            {
                new JobCardEntity { JobCardId = 1 }
            }.AsQueryable();

            var services = new List<Service>
            {
                new Service
                {
                    ServiceId = 1,
                    BasePrice = 100,
                    ServiceTasks = new List<ServiceTask>()
                }
            }.AsQueryable();

            var asyncJobCards = new TestAsyncEnumerable<JobCardEntity>(jobCards);
            var asyncServices = new TestAsyncEnumerable<Service>(services);

            _jobCardRepo.Setup(x => x.GetAll()).Returns(asyncJobCards);
            _serviceRepo.Setup(x => x.GetAll()).Returns(asyncServices);

            JobCardServiceEntity? captured = null;

            _jobCardServiceRepo
                .Setup(x => x.AddAsync(It.IsAny<JobCardServiceEntity>(), It.IsAny<CancellationToken>()))
                .Callback<JobCardServiceEntity, CancellationToken>((entity, _) =>
                {
                    captured = entity;
                })
                .Returns(Task.CompletedTask);
            _jobCardRepo
    .Setup(x => x.GetByIdAsync(1))
    .ReturnsAsync(new JobCardEntity { JobCardId = 1 });
            var result = await _service.AddServiceAsync(1, dto, CancellationToken.None);

            Assert.IsTrue(result);
            Assert.IsNotNull(captured);
            Assert.AreEqual(1, captured.ServiceId);
            Assert.AreEqual(1, captured.JobCardId);
        }

        [TestMethod]
        public async Task AddServiceAsync_WhenJobCardNotFound_ReturnsFalse()
        {
            var dto = new AddServiceToJobCardDto
            {
                ServiceId = 1
            };

            var jobCards = new List<JobCardEntity>().AsQueryable();
            var asyncJobCards = new TestAsyncEnumerable<JobCardEntity>(jobCards);

            _jobCardRepo.Setup(x => x.GetAll()).Returns(asyncJobCards);

            var result = await _service.AddServiceAsync(1, dto, CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AddServiceAsync_WhenServiceNotFound_ReturnsFalse()
        {
            var dto = new AddServiceToJobCardDto
            {
                ServiceId = 10
            };

            var jobCards = new List<JobCardEntity>
            {
                new JobCardEntity { JobCardId = 1 }
            }.AsQueryable();

            var services = new List<Service>().AsQueryable();

            var asyncJobCards = new TestAsyncEnumerable<JobCardEntity>(jobCards);
            var asyncServices = new TestAsyncEnumerable<Service>(services);

            _jobCardRepo.Setup(x => x.GetAll()).Returns(asyncJobCards);
            _serviceRepo.Setup(x => x.GetAll()).Returns(asyncServices);

            var result = await _service.AddServiceAsync(1, dto, CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AddServiceAsync_WhenServiceAlreadyExists_ReturnsFalse()
        {
            var dto = new AddServiceToJobCardDto
            {
                ServiceId = 1
            };

            var jobCards = new List<JobCardEntity>
    {
        new JobCardEntity { JobCardId = 1 }
    }.AsQueryable();

            var services = new List<Service>
    {
        new Service
        {
ServiceId = 1,
            BasePrice = 100
        }
    }.AsQueryable();

            var asyncJobCards = new TestAsyncEnumerable<JobCardEntity>(jobCards);
            var asyncServices = new TestAsyncEnumerable<Service>(services);

            _jobCardRepo.Setup(x => x.GetAll()).Returns(asyncJobCards);
            _serviceRepo.Setup(x => x.GetAll()).Returns(asyncServices);

            var result = await _service.AddServiceAsync(1, dto, CancellationToken.None);

            Assert.IsFalse(result);
        }
    }
}