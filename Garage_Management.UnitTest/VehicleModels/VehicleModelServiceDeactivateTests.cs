using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Vehicles;
using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.VehicleModels
{
    [TestClass]
    public class VehicleModelServiceDeactivateTests
    {
        private Mock<IVehicleModelRepository> _repo = null!;
        private Mock<IVehicleBrandRepository> _brandRepo = null!;
        private VehicleModelService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IVehicleModelRepository>();
            _brandRepo = new Mock<IVehicleBrandRepository>();
            _service = new VehicleModelService(_repo.Object, _brandRepo.Object);
        }

        [TestMethod]
        public async Task DeActiveAsync_InvalidId_Throws()
        {
            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() => _service.DeActiveAsync(0));
            Assert.AreEqual("Id không hợp lệ", ex.Message);
        }

        [TestMethod]
        public async Task DeActiveAsync_NotFound_ReturnsFalse()
        {
            _repo.Setup(x => x.GetByIdAsync(9)).ReturnsAsync((VehicleModel?)null);

            var result = await _service.DeActiveAsync(9);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeActiveAsync_AlreadyInactive_Throws()
        {
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new VehicleModel { ModelId = 1, BrandId = 1, ModelName = "Vision", IsActive = false });

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() => _service.DeActiveAsync(1));
            Assert.AreEqual("Model đã được vô hiệu hóa trước đó", ex.Message);
        }

        [TestMethod]
        public async Task DeActiveAsync_Valid_ReturnsTrue()
        {
            var entity = new VehicleModel { ModelId = 1, BrandId = 1, ModelName = "Vision", IsActive = true };
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            var result = await _service.DeActiveAsync(1);

            Assert.IsTrue(result);
            Assert.IsFalse(entity.IsActive);
            _repo.Verify(x => x.Update(It.IsAny<VehicleModel>()), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
