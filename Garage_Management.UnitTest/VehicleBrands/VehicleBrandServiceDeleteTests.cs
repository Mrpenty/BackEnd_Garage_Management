using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Vehicles;
using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.VehicleBrands
{
    [TestClass]
    public class VehicleBrandServiceDeleteTests
    {
        private Mock<IVehicleBrandRepository> _repo = null!;
        private VehicleBrandService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IVehicleBrandRepository>();
            _service = new VehicleBrandService(_repo.Object);
        }

        [TestMethod]
        public async Task DeleteAsync_NotFound_ReturnsFalse()
        {
            _repo.Setup(x => x.GetByIdAsync(100)).ReturnsAsync((VehicleBrand?)null);

            var result = await _service.DeleteAsync(100, CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeleteAsync_HasModels_Throws()
        {
            var entity = new VehicleBrand { BrandId = 101, BrandName = "Honda", IsActive = true };
            _repo.Setup(x => x.GetByIdAsync(101)).ReturnsAsync(entity);
            _repo.Setup(x => x.HasModelsAsync(101, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(
                () => _service.DeleteAsync(101, CancellationToken.None));
        }

        [TestMethod]
        public async Task DeleteAsync_HasVehicles_Throws()
        {
            var entity = new VehicleBrand { BrandId = 102, BrandName = "Yamaha", IsActive = true };
            _repo.Setup(x => x.GetByIdAsync(102)).ReturnsAsync(entity);
            _repo.Setup(x => x.HasModelsAsync(102, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.HasVehiclesAsync(102, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(
                () => _service.DeleteAsync(102, CancellationToken.None));
        }

        [TestMethod]
        public async Task DeleteAsync_NoDependencies_ReturnsTrue()
        {
            var entity = new VehicleBrand { BrandId = 103, BrandName = "Suzuki", IsActive = true };
            _repo.Setup(x => x.GetByIdAsync(103)).ReturnsAsync(entity);
            _repo.Setup(x => x.HasModelsAsync(103, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.HasVehiclesAsync(103, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.DeleteAsync(103, CancellationToken.None);

            Assert.IsTrue(result);
            _repo.Verify(x => x.Delete(entity), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
