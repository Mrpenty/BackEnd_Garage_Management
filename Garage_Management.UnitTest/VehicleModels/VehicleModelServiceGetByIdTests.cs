using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Vehicles;
using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.VehicleModels
{
    [TestClass]
    public class VehicleModelServiceGetByIdTests
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
        public async Task GetByIdAsync_Found_ReturnsResponse()
        {
            var entity = new VehicleModel
            {
                ModelId = 1,
                BrandId = 2,
                VehicleTypeId = 3,
                ModelName = "Exciter",
                IsActive = true
            };
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.ModelId);
            Assert.AreEqual(2, result.BrandId);
            Assert.AreEqual(3, result.TypeId);
            Assert.AreEqual("Exciter", result.ModelName);
            Assert.IsTrue(result.isActive);
        }

        [TestMethod]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            _repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((VehicleModel?)null);

            var result = await _service.GetByIdAsync(99);

            Assert.IsNull(result);
        }
    }
}
