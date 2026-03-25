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
    public class VehicleBrandServiceGetByIdTests
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
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            _repo.Setup(x => x.GetByIdAsync(100)).ReturnsAsync((VehicleBrand?)null);

            var result = await _service.GetByIdAsync(100, CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_Found_ReturnsMappedResponse()
        {
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new VehicleBrand
            {
                BrandId = 1,
                BrandName = "Honda",
                IsActive = true
            });

            var result = await _service.GetByIdAsync(1, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.BrandId);
            Assert.AreEqual("Honda", result.BrandName);
            Assert.IsTrue(result.isActive);
        }

        [TestMethod]
        public async Task GetByIdAsync_Found_IsActiveFalse_ReturnsMappedResponse()
        {
            _repo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new VehicleBrand
            {
                BrandId = 2,
                BrandName = "Yamaha",
                IsActive = false
            });

            var result = await _service.GetByIdAsync(2, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.BrandId);
            Assert.AreEqual("Yamaha", result.BrandName);
            Assert.IsFalse(result.isActive);
        }
    }
}
