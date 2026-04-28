using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Moq;
using JobCardServiceApp = Garage_Management.Application.Services.JobCards.JobCardServiceService;
using JobCardServiceEntity = Garage_Management.Base.Entities.JobCards.JobCardService;

namespace Garage_Management.UnitTest.JobCardServices
{
    [TestClass]
    public class JobCardServiceServiceDeleteTests
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
        public async Task DeleteAsync_ReturnsFalse_WhenEntityDoesNotExist()
        {
            _jobCardServiceRepo.Setup(x => x.GetByIdAsync(77))
                .ReturnsAsync((JobCardServiceEntity?)null);

            var result = await _service.DeleteAsync(77, CancellationToken.None);

            Assert.IsFalse(result);
            _jobCardServiceRepo.Verify(x => x.Delete(It.IsAny<JobCardServiceEntity>()), Times.Never);
        }

        [TestMethod]
        public async Task DeleteAsync_ReturnsTrue_WhenEntityExists()
        {
            var entity = new JobCardServiceEntity
            {
                JobCardServiceId = 11
            };

            _jobCardServiceRepo.Setup(x => x.GetByIdAsync(11))
                .ReturnsAsync(entity);

            _jobCardServiceRepo
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _service.DeleteAsync(11, CancellationToken.None);

            Assert.IsTrue(result);
            _jobCardServiceRepo.Verify(x => x.Delete(entity), Times.Once);
            _jobCardServiceRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
