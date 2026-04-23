using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Vehicles;
using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Vehicles
{
    [TestClass]
    public class VehicleServiceGetByIdTests
    {
        private Mock<IVehicleRepository> _repo = null!;
        private VehicleService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IVehicleRepository>();
            _service = new VehicleService(
                _repo.Object,
                new Mock<ICustomerRepository>().Object,
                new Mock<IVehicleModelRepository>().Object,
                new Mock<IHttpContextAccessor>().Object);
        }

        [TestMethod]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Vehicle?)null);

            var result = await _service.GetByIdAsync(1, CancellationToken.None);

            Assert.IsNull(result);
            _repo.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [TestMethod]
        public async Task GetByIdAsync_Found_ReturnsMappedResponse()
        {
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Vehicle
            {
                VehicleId = 1,
                CustomerId = 5,
                ModelId = 2,
                Brand = new VehicleBrand { BrandName = "Honda" },
                Model = new VehicleModel { ModelName = "Vision" }
            });

            var result = await _service.GetByIdAsync(1, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.VehicleId);
            Assert.AreEqual(5, result.CustomerId);
            Assert.AreEqual("Honda", result.BrandName);
            Assert.AreEqual("Vision", result.ModelName);
            _repo.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [TestMethod]
        public async Task GetByIdAsync_FoundWithNullBrandModel_MapsEmptyNames()
        {
            _repo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new Vehicle
            {
                VehicleId = 2,
                CustomerId = 6,
                ModelId = 3,
                Brand = null,
                Model = null
            });

            var result = await _service.GetByIdAsync(2, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(string.Empty, result.BrandName);
            Assert.AreEqual(string.Empty, result.ModelName);
            _repo.Verify(x => x.GetByIdAsync(2), Times.Once);
        }
    }
}
