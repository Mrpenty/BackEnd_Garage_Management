using Garage_Management.Application.DTOs.JobCardServices;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.Services;
using Garage_Management.UnitTest.Helper;
using Moq;
using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;
using JobCardServiceApp = Garage_Management.Application.Services.JobCards.JobCardServiceService;
using JobCardServiceEntity = Garage_Management.Base.Entities.JobCards.JobCardService;

namespace Garage_Management.UnitTest.JobCards.JobCardServices
{
    [TestClass]
    public class JobCardServiceServiceTests
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
        public async Task GetByIdAsync_ReturnsMappedResponse_WhenEntityExists()
        {
            var entity = new JobCardServiceEntity
            {
                JobCardServiceId = 9,
                JobCardId = 2,
                ServiceId = 5,
                Description = "Rotate tires",
                Price = 250,
                Status = ServiceStatus.InProgress,
                SourceInspectionItemId = 12
            };

            _jobCardServiceRepo
                .Setup(x => x.GetByIdAsync(9))
                .ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(9);

            Assert.IsNotNull(result);
            Assert.AreEqual(entity.JobCardServiceId, result.JobCardServiceId);
            Assert.AreEqual(entity.Description, result.Description);
            Assert.AreEqual(entity.Status, result.Status);
        }

        [TestMethod]
        public async Task CreateAsync_ReturnsError_WhenJobCardDoesNotExist()
        {
            var request = new JobCardServiceCreateRequest
            {
                JobCardId = 100,
                ServiceId = 2
            };

            _jobCardRepo
                .Setup(x => x.GetByIdAsync(request.JobCardId))
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
                ServiceTasks = new List<ServiceTask>
                {
                    new ServiceTask { ServiceTaskId = 11, TaskOrder = 2, TaskName = "Second task" },
                    new ServiceTask { ServiceTaskId = 10, TaskOrder = 1, TaskName = "First task" }
                }
            };

            JobCardServiceEntity? addedEntity = null;
            IReadOnlyCollection<JobCardServiceTask>? addedTasks = null;

            _jobCardRepo
                .Setup(x => x.GetByIdAsync(request.JobCardId))
                .ReturnsAsync(new JobCardEntity { JobCardId = request.JobCardId });

            _serviceRepo
                .Setup(x => x.GetByIdWithTasksAsync(request.ServiceId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(service);

            _jobCardServiceRepo
                .Setup(x => x.Add(It.IsAny<JobCardServiceEntity>()))
                .Callback<JobCardServiceEntity>(entity =>
                {
                    entity.JobCardServiceId = 44;
                    addedEntity = entity;
                });

            _jobCardServiceTaskRepo
                .Setup(x => x.AddRange(It.IsAny<IReadOnlyCollection<JobCardServiceTask>>()))
                .Callback<IReadOnlyCollection<JobCardServiceTask>>(tasks => addedTasks = tasks);

            _jobCardServiceRepo
                .Setup(x => x.GetByIdWithTasksAsync(44, It.IsAny<CancellationToken>()))
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
            Assert.AreEqual(service.BasePrice.Value, addedEntity.Price);
            Assert.AreEqual(request.Description, addedEntity.Description);
            Assert.IsNotNull(addedTasks);
            Assert.AreEqual(2, addedTasks.Count);
            CollectionAssert.AreEqual(
                new[] { 10, 11 },
                addedTasks.Select(x => x.ServiceTaskId).ToArray());
            _jobCardServiceRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
            _jobCardServiceTaskRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateAsync_UpdatesOnlyProvidedFields()
        {
            var entity = new JobCardServiceEntity
            {
                JobCardServiceId = 15,
                JobCardId = 1,
                ServiceId = 2,
                Description = "Old",
                Price = 120,
                Status = ServiceStatus.Pending,
                SourceInspectionItemId = 5
            };

            _jobCardServiceRepo
                .Setup(x => x.GetByIdAsync(15))
                .ReturnsAsync(entity);

            var request = new JobCardServiceUpdateRequest
            {
                Description = "Updated",
                Price = 300,
                Status = ServiceStatus.Completed
            };

            var result = await _service.UpdateAsync(15, request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual("Updated", entity.Description);
            Assert.AreEqual(300, entity.Price);
            Assert.AreEqual(ServiceStatus.Completed, entity.Status);
            Assert.AreEqual(5, entity.SourceInspectionItemId);
            Assert.IsNotNull(entity.UpdatedAt);
            _jobCardServiceRepo.Verify(x => x.Update(entity), Times.Once);
            _jobCardServiceRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateStatusByServiceIdAsync_Throws_WhenMultipleRowsMatchWithoutJobCardId()
        {
            var data = new TestAsyncEnumerable<JobCardServiceEntity>(new[]
            {
                new JobCardServiceEntity { JobCardServiceId = 1, JobCardId = 10, ServiceId = 8 },
                new JobCardServiceEntity { JobCardServiceId = 2, JobCardId = 11, ServiceId = 8 }
            });

            _jobCardServiceRepo
                .Setup(x => x.GetAll())
                .Returns(data);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.UpdateStatusByServiceIdAsync(
                    8,
                    null,
                    new JobCardServiceStatusUpdateRequest { Status = ServiceStatus.Completed },
                    CancellationToken.None));
        }

        [TestMethod]
        public async Task DeleteAsync_ReturnsFalse_WhenEntityDoesNotExist()
        {
            _jobCardServiceRepo
                .Setup(x => x.GetByIdAsync(77))
                .ReturnsAsync((JobCardServiceEntity?)null);

            var result = await _service.DeleteAsync(77, CancellationToken.None);

            Assert.IsFalse(result);
            _jobCardServiceRepo.Verify(x => x.Delete(It.IsAny<JobCardServiceEntity>()), Times.Never);
        }

        [TestMethod]
        public async Task GetPagedAsync_MapsPagedResult()
        {
            _jobCardServiceRepo
                .Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<JobCardServiceEntity>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 1,
                    PageData = new List<JobCardServiceEntity>
                    {
                        new JobCardServiceEntity
                        {
                            JobCardServiceId = 5,
                            JobCardId = 1,
                            ServiceId = 2,
                            Description = "Alignment",
                            Price = 150,
                            Status = ServiceStatus.Pending
                        }
                    }
                });

            var result = await _service.GetPagedAsync(1, 10, CancellationToken.None);
            var pageData = result.PageData.ToList();

            Assert.AreEqual(1, result.Total);
            Assert.AreEqual(1, pageData.Count);
            Assert.AreEqual("Alignment", pageData[0].Description);
        }
    }
}
