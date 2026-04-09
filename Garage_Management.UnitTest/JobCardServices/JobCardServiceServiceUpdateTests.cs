using Garage_Management.Application.DTOs.JobCardServices;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.UnitTest.Helper;
using Moq;
using JobCardServiceApp = Garage_Management.Application.Services.JobCards.JobCardServiceService;
using JobCardServiceEntity = Garage_Management.Base.Entities.JobCards.JobCardService;

namespace Garage_Management.UnitTest.JobCardServices
{
    [TestClass]
    public class JobCardServiceServiceUpdateTests
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

            _jobCardServiceRepo.Setup(x => x.GetByIdAsync(15))
                .ReturnsAsync(entity);

            var result = await _service.UpdateAsync(15, new JobCardServiceUpdateRequest
            {
                Description = "Updated",
                Price = 300,
                Status = ServiceStatus.Completed
            }, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual("Updated", entity.Description);
            Assert.AreEqual(300m, entity.Price);
            Assert.AreEqual(ServiceStatus.Completed, entity.Status);
            Assert.AreEqual(5, entity.SourceInspectionItemId);
            _jobCardServiceRepo.Verify(x => x.Update(entity), Times.Once);
            _jobCardServiceRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateStatusByServiceIdAsync_Throws_WhenMultipleRowsMatchWithoutJobCardId()
        {
            var data = new TestAsyncEnumerable<JobCardServiceEntity>(
            [
                new JobCardServiceEntity { JobCardServiceId = 1, JobCardId = 10, ServiceId = 8 },
                new JobCardServiceEntity { JobCardServiceId = 2, JobCardId = 11, ServiceId = 8 }
            ]);

            _jobCardServiceRepo.Setup(x => x.GetAll()).Returns(data);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.UpdateStatusByServiceIdAsync(
                    8,
                    null,
                    new JobCardServiceStatusUpdateRequest { Status = ServiceStatus.Completed },
                    CancellationToken.None));
        }
    }
}
