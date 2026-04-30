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
    public class ServiceServiceUpdatePriceTests
    {
        private Mock<IServiceRepository> _repo = null!;
        private ServiceService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IServiceRepository>();
            _service = new ServiceService(_repo.Object);
        }

        [TestMethod]
        public async Task UpdatePriceAsync_SetValidPrice_ActivatesService()
        {
            var entity = new Service
            {
                ServiceId = 1,
                ServiceName = "Rua xe",
                BasePrice = null,
                IsActive = false
            };
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.UpdatePriceAsync(
                1,
                new ServicePriceUpdateRequest { BasePrice = 50000m },
                CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(50000m, entity.BasePrice);
            Assert.IsTrue(entity.IsActive);
            _repo.Verify(x => x.Update(entity), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
