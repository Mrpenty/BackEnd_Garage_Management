using Garage_Management.Application.DTOs.Vehicles.VehicleModel;
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
    public class VehicleModelServiceUpdateTests
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
        public async Task UpdateAsync_InvalidId_Throws()
        {
            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                _service.UpdateAsync(0, new VehicleModelUpdate { TypeId = 1, BrandId = 1, ModelName = "Test", isActive = true }));
            Assert.AreEqual("Id không hợp lệ", ex.Message);
        }

        [TestMethod]
        public async Task UpdateAsync_NotFound_ReturnsNull()
        {
            _repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((VehicleModel?)null);

            var result = await _service.UpdateAsync(99, new VehicleModelUpdate { TypeId = 1, BrandId = 1, ModelName = "Test", isActive = true });

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateAsync_AlreadyInactive_Throws()
        {
            _repo.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(new VehicleModel { ModelId = 5, BrandId = 1, ModelName = "Old", VehicleTypeId = 1, IsActive = false });

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                _service.UpdateAsync(5, new VehicleModelUpdate { TypeId = 1, BrandId = 1, ModelName = "Vision", isActive = true }));
            Assert.AreEqual("Không thể cập nhật model đã bị vô hiệu hóa", ex.Message);
        }

        [TestMethod]
        public async Task UpdateAsync_InvalidTypeId_Throws()
        {
            _repo.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(new VehicleModel { ModelId = 5, BrandId = 1, ModelName = "Old", VehicleTypeId = 1 });

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                _service.UpdateAsync(5, new VehicleModelUpdate { TypeId = 0, BrandId = 1, ModelName = "Vision", isActive = true }));
            Assert.AreEqual("TypeId không hợp lệ", ex.Message);
        }

        [TestMethod]
        public async Task UpdateAsync_InvalidBrandId_Throws()
        {
            _repo.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(new VehicleModel { ModelId = 5, BrandId = 1, ModelName = "Old", VehicleTypeId = 1 });

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                _service.UpdateAsync(5, new VehicleModelUpdate { TypeId = 1, BrandId = 0, ModelName = "Vision", isActive = true }));
            Assert.AreEqual("BrandId không hợp lệ", ex.Message);
        }

        [TestMethod]
        public async Task UpdateAsync_EmptyModelName_Throws()
        {
            _repo.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(new VehicleModel { ModelId = 5, BrandId = 1, ModelName = "Old", VehicleTypeId = 1 });

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                _service.UpdateAsync(5, new VehicleModelUpdate { TypeId = 1, BrandId = 1, ModelName = "  ", isActive = true }));
            Assert.AreEqual("ModelName không hợp lệ", ex.Message);
        }

        [TestMethod]
        public async Task UpdateAsync_BrandNotFound_Throws()
        {
            _repo.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(new VehicleModel { ModelId = 5, BrandId = 1, ModelName = "Old", VehicleTypeId = 1 });
            _brandRepo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((VehicleBrand?)null);

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                _service.UpdateAsync(5, new VehicleModelUpdate { TypeId = 1, BrandId = 99, ModelName = "Vision", isActive = true }));
            Assert.AreEqual("BrandId không tồn tại", ex.Message);
        }

        [TestMethod]
        public async Task UpdateAsync_Duplicate_Throws()
        {
            _repo.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(new VehicleModel { ModelId = 5, BrandId = 1, ModelName = "Old", VehicleTypeId = 1 });
            _brandRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new VehicleBrand { BrandId = 1, BrandName = "Honda" });
            _repo.Setup(x => x.ExistsAsync(1, 1, "Vision", 5, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                _service.UpdateAsync(5, new VehicleModelUpdate { TypeId = 1, BrandId = 1, ModelName = "Vision", isActive = true }));
            Assert.AreEqual("ModelName đã tồn tại", ex.Message);
        }

        [TestMethod]
        public async Task UpdateAsync_HasVehicles_Throws()
        {
            _repo.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(new VehicleModel { ModelId = 5, BrandId = 1, ModelName = "Old", VehicleTypeId = 1 });
            _brandRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new VehicleBrand { BrandId = 1, BrandName = "Honda" });
            _repo.Setup(x => x.ExistsAsync(1, 1, "Vision", 5, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.HasVehiclesAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                _service.UpdateAsync(5, new VehicleModelUpdate { TypeId = 1, BrandId = 1, ModelName = "Vision", isActive = true }));
            Assert.AreEqual("Không thể cập nhật vì đang có xe liên kết", ex.Message);
        }

        [TestMethod]
        public async Task UpdateAsync_Valid_ReturnsUpdatedResponse()
        {
            _repo.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(new VehicleModel { ModelId = 5, BrandId = 1, ModelName = "Old", VehicleTypeId = 1 });
            _brandRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new VehicleBrand { BrandId = 2, BrandName = "Yamaha" });
            _repo.Setup(x => x.ExistsAsync(2, 1, "Exciter", 5, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.HasVehiclesAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var result = await _service.UpdateAsync(5, new VehicleModelUpdate { TypeId = 1, BrandId = 2, ModelName = "Exciter", isActive = true });

            Assert.IsNotNull(result);
            Assert.AreEqual("Exciter", result.ModelName);
            Assert.AreEqual(2, result.BrandId);
            Assert.AreEqual(1, result.TypeId);
            Assert.IsTrue(result.isActive);
            _repo.Verify(x => x.Update(It.IsAny<VehicleModel>()), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
