using Garage_Management.Application.DTOs.Vehicles.VehicleBrand;
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
    public class VehicleBrandServiceCreateTests
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
        public async Task CreateAsync_ValidRequest_ReturnsCreatedResponse()
        {
            var request = new VehicleBrandCreateRequest
            {
                BrandName = "Suzuki",
                isActive = true
            };

            _repo.Setup(x => x.HasExistAsync("Suzuki", 0, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.AddAsync(It.IsAny<VehicleBrand>(), It.IsAny<CancellationToken>()))
                .Callback<VehicleBrand, CancellationToken>((e, _) => e.BrandId = 7)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(7, result.BrandId);
            Assert.AreEqual("Suzuki", result.BrandName);
            Assert.IsTrue(result.isActive);
            _repo.Verify(x => x.AddAsync(It.IsAny<VehicleBrand>(), It.IsAny<CancellationToken>()), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateAsync_ValidRequest_IsActiveFalse_ReturnsCreatedResponse()
        {
            var request = new VehicleBrandCreateRequest
            {
                BrandName = "SYM",
                isActive = false
            };

            _repo.Setup(x => x.HasExistAsync("SYM", 0, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.AddAsync(It.IsAny<VehicleBrand>(), It.IsAny<CancellationToken>()))
                .Callback<VehicleBrand, CancellationToken>((e, _) => e.BrandId = 8)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(8, result.BrandId);
            Assert.AreEqual("SYM", result.BrandName);
            Assert.IsFalse(result.isActive);
        }

        [TestMethod]
        public async Task CreateAsync_DuplicateBrand_Throws()
        {
            var request = new VehicleBrandCreateRequest
            {
                BrandName = "Honda",
                isActive = true
            };

            _repo.Setup(x => x.HasExistAsync("Honda", 0, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }
    }
}
