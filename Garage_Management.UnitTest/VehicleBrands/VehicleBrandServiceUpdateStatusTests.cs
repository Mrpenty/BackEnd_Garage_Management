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
    public class VehicleBrandServiceUpdateStatusTests
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
        public async Task UpdateStatusAsync_NotFound_ReturnsFalse()
        {
            _repo.Setup(x => x.GetByIdAsync(20)).ReturnsAsync((VehicleBrand?)null);

            var result = await _service.UpdateStatusAsync(20, true, CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_SameStatus_ReturnsTrueWithoutSave()
        {
            var entity = new VehicleBrand { BrandId = 21, BrandName = "Honda", IsActive = true };
            _repo.Setup(x => x.GetByIdAsync(21)).ReturnsAsync(entity);

            var result = await _service.UpdateStatusAsync(21, true, CancellationToken.None);

            Assert.IsTrue(result);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_ChangedStatus_UpdatesAndSaves()
        {
            var entity = new VehicleBrand { BrandId = 22, BrandName = "Yamaha", IsActive = true };
            _repo.Setup(x => x.GetByIdAsync(22)).ReturnsAsync(entity);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.UpdateStatusAsync(22, false, CancellationToken.None);

            Assert.IsTrue(result);
            Assert.IsFalse(entity.IsActive);
            _repo.Verify(x => x.Update(entity), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_SameStatus_Inactive_ReturnsWithoutSave()
        {
            var entity = new VehicleBrand { BrandId = 23, BrandName = "Suzuki", IsActive = false };
            _repo.Setup(x => x.GetByIdAsync(23)).ReturnsAsync(entity);

            var result = await _service.UpdateStatusAsync(23, false, CancellationToken.None);

            Assert.IsTrue(result);
            Assert.IsFalse(entity.IsActive);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_InactiveToActive_UpdatesAndSaves()
        {
            var entity = new VehicleBrand { BrandId = 24, BrandName = "Kymco", IsActive = false };
            _repo.Setup(x => x.GetByIdAsync(24)).ReturnsAsync(entity);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.UpdateStatusAsync(24, true, CancellationToken.None);

            Assert.IsTrue(result);
            Assert.IsTrue(entity.IsActive);
            _repo.Verify(x => x.Update(entity), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
