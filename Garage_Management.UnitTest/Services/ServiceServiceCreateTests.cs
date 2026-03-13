using Garage_Management.Application.DTOs.Services;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Services.Services;
using Garage_Management.Base.Entities.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Services
{
    [TestClass]
    public class ServiceServiceCreateTests
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
        public async Task CreateAsync_ValidRequest_ReturnsCreatedResponse()
        {
            var request = new ServiceCreateRequest
            {
                ServiceName = "Bao duong",
                BasePrice = 200000m,
                Description = "DV test",
                IsActive = true
            };

            _repo.Setup(x => x.ExistsByNameAsync("Bao duong", It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.AddAsync(It.IsAny<Service>(), It.IsAny<CancellationToken>()))
                .Callback<Service, CancellationToken>((e, _) => e.ServiceId = 5)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.AreEqual(5, result.ServiceId);
            Assert.AreEqual("Bao duong", result.ServiceName);
            Assert.AreEqual(200000m, result.BasePrice);
            Assert.IsTrue(result.IsActive);
        }

        [TestMethod]
        public async Task CreateAsync_InvalidServiceName_Throws()
        {
            var request = new ServiceCreateRequest
            {
                ServiceName = "   ",
                BasePrice = 100000m,
                IsActive = true
            };

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_InvalidBasePrice_Throws()
        {
            var request = new ServiceCreateRequest
            {
                ServiceName = "Rua xe",
                BasePrice = 0,
                IsActive = true
            };

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_DuplicateServiceName_Throws()
        {
            var request = new ServiceCreateRequest
            {
                ServiceName = "Rua xe",
                BasePrice = 100000m,
                IsActive = true
            };

            _repo.Setup(x => x.ExistsByNameAsync("Rua xe", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }
    }
}
