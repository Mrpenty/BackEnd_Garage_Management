using Garage_Management.Application.DTOs.JobCardServices;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.Services;
using Moq;
using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;
using JobCardServiceApp = Garage_Management.Application.Services.JobCards.JobCardServiceService;
using JobCardServiceEntity = Garage_Management.Base.Entities.JobCards.JobCardService;

namespace Garage_Management.UnitTest.JobCardServices
{
    [TestClass]
    public class JobCardServiceServiceCreateTests
    {
        private Mock<IJobCardServiceRepository> _jobCardServiceRepo = null!;
        private Mock<IJobCardServiceTaskRepository> _jobCardServiceTaskRepo = null!;
        private Mock<IJobCardRepository> _jobCardRepo = null!;
        private Mock<IServiceRepository> _serviceRepo = null!;
        private JobCardServiceApp _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _jobCardServiceRepo = new Mock<IJobCardServiceRepository>();
            _jobCardServiceTaskRepo = new Mock<IJobCardServiceTaskRepository>();
            _jobCardRepo = new Mock<IJobCardRepository>();
            _serviceRepo = new Mock<IServiceRepository>();
            _service = new JobCardServiceApp(
                _jobCardServiceRepo.Object,
                _jobCardServiceTaskRepo.Object,
                _jobCardRepo.Object,
                _serviceRepo.Object);
        }

        [TestMethod]
        public async Task CreateAsync_ReturnsError_WhenJobCardDoesNotExist()
        {
            var request = new JobCardServiceCreateRequest
            {
                JobCardId = 100,
                ServiceId = 2
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(request.JobCardId))
                .ReturnsAsync((JobCardEntity?)null);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Data);
            StringAssert.Contains(result.Message, "JobCardId");
        }

        [TestMethod]
        public async Task CreateAsync_CreatesJobCardServiceAndTasks_WhenRequestIsValid()
        {
            var request = new JobCardServiceCreateRequest
            {
                JobCardId = 3,
                ServiceId = 7,
                Description = "Detailed check",
                Status = ServiceStatus.Pending,
                SourceInspectionItemId = 99
            };

            var service = new Service
            {
                ServiceId = request.ServiceId,
                Description = "Fallback description",
                BasePrice = 500,
                ServiceTasks =
                [
                    new ServiceTask { ServiceTaskId = 11, TaskOrder = 2, TaskName = "Second task" },
                    new ServiceTask { ServiceTaskId = 10, TaskOrder = 1, TaskName = "First task" }
                ]
            };

            JobCardServiceEntity? addedEntity = null;
            IReadOnlyCollection<JobCardServiceTask>? addedTasks = null;

            _jobCardRepo.Setup(x => x.GetByIdAsync(request.JobCardId))
                .ReturnsAsync(new JobCardEntity { JobCardId = request.JobCardId });

            _serviceRepo.Setup(x => x.GetByIdWithTasksAsync(request.ServiceId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(service);

            _jobCardServiceRepo.Setup(x => x.Add(It.IsAny<JobCardServiceEntity>()))
                .Callback<JobCardServiceEntity>(entity =>
                {
                    entity.JobCardServiceId = 44;
                    addedEntity = entity;
                });

            _jobCardServiceTaskRepo.Setup(x => x.AddRange(It.IsAny<IReadOnlyCollection<JobCardServiceTask>>()))
                .Callback<IReadOnlyCollection<JobCardServiceTask>>(tasks => addedTasks = tasks);

            _jobCardServiceRepo.Setup(x => x.GetByIdWithTasksAsync(44, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new JobCardServiceEntity
                {
                    JobCardServiceId = 44,
                    JobCardId = request.JobCardId,
                    ServiceId = request.ServiceId,
                    Description = request.Description,
                    Price = service.BasePrice!.Value,
                    Status = request.Status,
                    SourceInspectionItemId = request.SourceInspectionItemId
                });

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Data);
            Assert.IsNotNull(addedEntity);
            Assert.AreEqual(500m, addedEntity.Price);
            Assert.AreEqual("Detailed check", addedEntity.Description);
            Assert.IsNotNull(addedTasks);
            CollectionAssert.AreEqual(new[] { 10, 11 }, addedTasks.Select(x => x.ServiceTaskId).ToArray());
            _jobCardServiceRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
            _jobCardServiceTaskRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
