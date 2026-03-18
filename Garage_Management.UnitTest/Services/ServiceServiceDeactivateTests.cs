using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Services.Services;
using Garage_Management.Base.Entities.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Services
{
    [TestClass]
    public class ServiceServiceDeactivateTests
    {
        private Mock<IServiceRepository> _repo;
        private ServiceService _service;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IServiceRepository>();
            _service = new ServiceService(_repo.Object);
        }

        [TestMethod]
        public async Task DeactivateAsync_NotFound_ReturnsNull()
        {
            _repo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync((Service?)null);

            var result = await _service.DeactivateAsync(10, CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DeactivateAsync_AlreadyInactive_ReturnsResponse()
        {
            var entity = new Service
            {
                ServiceId = 11,
                ServiceName = "Dich vu test",
                BasePrice = 100000m,
                IsActive = false,
                ServiceTasks = new List<ServiceTask>()
            };
            _repo.Setup(x => x.GetByIdAsync(11)).ReturnsAsync(entity);

            var result = await _service.DeactivateAsync(11, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsActive);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task DeactivateAsync_Active_UpdatesToInactive()
        {
            var entity = new Service
            {
                ServiceId = 12,
                ServiceName = "Dich vu test",
                BasePrice = 100000m,
                IsActive = true,
                ServiceTasks = new List<ServiceTask>()
            };
            _repo.Setup(x => x.GetByIdAsync(12)).ReturnsAsync(entity);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.DeactivateAsync(12, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsActive);
            _repo.Verify(x => x.Update(entity), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

