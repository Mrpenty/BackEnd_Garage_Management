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
    public class VehicleModelServiceCreateTests
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
        public async Task CreateAsync_InvalidTypeId_Throws()
        {
            var request = new VehicleModelCreateRequest { TypeId = 0, BrandId = 1, ModelName = "Vision", isActive = true };

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() => _service.CreateAsync(request));
            Assert.AreEqual("TypeId không hợp lệ", ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_InvalidBrandId_Throws()
        {
            var request = new VehicleModelCreateRequest { TypeId = 1, BrandId = 0, ModelName = "Vision", isActive = true };

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() => _service.CreateAsync(request));
            Assert.AreEqual("BrandId không hợp lệ", ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_EmptyModelName_Throws()
        {
            var request = new VehicleModelCreateRequest { TypeId = 1, BrandId = 1, ModelName = "  ", isActive = true };

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() => _service.CreateAsync(request));
            Assert.AreEqual("ModelName không hợp lệ", ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_BrandNotFound_Throws()
        {
            _brandRepo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync((VehicleBrand?)null);
            var request = new VehicleModelCreateRequest { TypeId = 1, BrandId = 10, ModelName = "Vision", isActive = true };

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() => _service.CreateAsync(request));
            Assert.AreEqual("BrandId không tồn tại", ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_Duplicate_Throws()
        {
            _brandRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new VehicleBrand { BrandId = 1, BrandName = "Honda" });
            _repo.Setup(x => x.ExistsAsync(1, 1, "Vision", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            var request = new VehicleModelCreateRequest { TypeId = 1, BrandId = 1, ModelName = "Vision", isActive = true };

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() => _service.CreateAsync(request));
            Assert.AreEqual("ModelName đã tồn tại trong brand này", ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_Valid_ReturnsResponse()
        {
            _brandRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new VehicleBrand { BrandId = 1, BrandName = "Honda" });
            _repo.Setup(x => x.ExistsAsync(1, 1, "Vision", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var request = new VehicleModelCreateRequest { TypeId = 1, BrandId = 1, ModelName = "Vision", isActive = true };

            var result = await _service.CreateAsync(request);

            Assert.IsNotNull(result);
            Assert.AreEqual("Vision", result.ModelName);
            Assert.AreEqual(1, result.BrandId);
            Assert.AreEqual(1, result.TypeId);
            Assert.IsTrue(result.isActive);
            _repo.Verify(x => x.AddAsync(It.IsAny<VehicleModel>(), It.IsAny<CancellationToken>()), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
