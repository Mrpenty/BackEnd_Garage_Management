using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Services.Services;
using Garage_Management.Base.Entities.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Services
{
    [TestClass]
    public class ServiceServiceDeleteTests
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
        public async Task DeleteAsync_NotFound_ReturnsFalse()
        {
            _repo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync((Service?)null);

            var result = await _service.DeleteAsync(10, CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeleteAsync_Found_ReturnsTrue()
        {
            var entity = new Service { ServiceId = 11, ServiceTasks = new List<ServiceTask>() };
            _repo.Setup(x => x.GetByIdAsync(11)).ReturnsAsync(entity);
            _repo.Setup(x => x.HasDependenciesAsync(11, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.DeleteAsync(11, CancellationToken.None);

            Assert.IsTrue(result);
            _repo.Verify(x => x.Delete(entity), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_HasDependencies_Throws()
        {
            var entity = new Service { ServiceId = 12, ServiceTasks = new List<ServiceTask>() };
            _repo.Setup(x => x.GetByIdAsync(12)).ReturnsAsync(entity);
            _repo.Setup(x => x.HasDependenciesAsync(12, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.DeleteAsync(12, CancellationToken.None));
        }
    }
}
