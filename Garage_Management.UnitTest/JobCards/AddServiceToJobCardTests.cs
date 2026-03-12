using Garage_Management.Application.DTOs.JobCard;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Entities.Services;
using Garage_Management.Base.Entities.JobCards; // Add this import for JobCard
using Moq;
using JobCardService = Garage_Management.Application.Services.JobCards.JobCardService;

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

        private JobCardService _service;

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

            _service = new JobCardService(
                _jobCardRepo.Object,
                _serviceRepo.Object,
                _inventoryRepo.Object,
                _jobCardServiceRepo.Object,
                _jobCardSparePartRepo.Object,
                _workBayRepo.Object,
                _appointmentRepo.Object
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

            var jobCards = new List<JobCard>
            {
                new JobCard { JobCardId = 1 }
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

            var asyncJobCards = new TestAsyncEnumerable<JobCard>(jobCards);
            var asyncServices = new TestAsyncEnumerable<Service>(services);

            _jobCardRepo.Setup(x => x.GetAll()).Returns(asyncJobCards);
            _serviceRepo.Setup(x => x.GetAll()).Returns(asyncServices);

            JobCardService? captured = null;

            _jobCardServiceRepo.Setup(x => x.AddAsync(It.IsAny<JobCardService>(), It.IsAny<CancellationToken>()))
                .Callback<JobCardService, CancellationToken>((entity, _) =>
                {
                    captured = entity;
                })
                .Returns(Task.CompletedTask);

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

            var jobCards = new List<JobCard>().AsQueryable();
            var asyncJobCards = new TestAsyncEnumerable<JobCard>(jobCards);

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

            var jobCards = new List<JobCard>
            {
                new JobCard { JobCardId = 1 }
            }.AsQueryable();

            var services = new List<Service>().AsQueryable();

            var asyncJobCards = new TestAsyncEnumerable<JobCard>(jobCards);
            var asyncServices = new TestAsyncEnumerable<Service>(services);

            _jobCardRepo.Setup(x => x.GetAll()).Returns(asyncJobCards);
            _serviceRepo.Setup(x => x.GetAll()).Returns(asyncServices);

            var result = await _service.AddServiceAsync(1, dto, CancellationToken.None);

            Assert.IsFalse(result);
        }
    }
}