using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Moq;
using JobCardServiceApp = Garage_Management.Application.Services.JobCards.JobCardServiceService;
using JobCardServiceEntity = Garage_Management.Base.Entities.JobCards.JobCardService;

namespace Garage_Management.UnitTest.JobCardServices
{
    [TestClass]
    public class JobCardServiceServiceGetTests
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

            _jobCardServiceRepo.Setup(x => x.GetByIdAsync(9))
                .ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(9);

            Assert.IsNotNull(result);
            Assert.AreEqual(9, result.JobCardServiceId);
            Assert.AreEqual("Rotate tires", result.Description);
            Assert.AreEqual(ServiceStatus.InProgress, result.Status);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsNull_WhenEntityDoesNotExist()
        {
            _jobCardServiceRepo.Setup(x => x.GetByIdAsync(99))
                .ReturnsAsync((JobCardServiceEntity?)null);

            var result = await _service.GetByIdAsync(99);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetPagedAsync_MapsPagedResult()
        {
            _jobCardServiceRepo.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<JobCardServiceEntity>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 1,
                    PageData =
                    [
                        new JobCardServiceEntity
                        {
                            JobCardServiceId = 5,
                            JobCardId = 1,
                            ServiceId = 2,
                            Description = "Alignment",
                            Price = 150,
                            Status = ServiceStatus.Pending
                        }
                    ]
                });

            var result = await _service.GetPagedAsync(1, 10, CancellationToken.None);
            var pageData = result.PageData.ToList();

            Assert.AreEqual(1, result.Total);
            Assert.AreEqual(1, pageData.Count);
            Assert.AreEqual("Alignment", pageData[0].Description);
        }
    }
}
