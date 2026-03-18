using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Vehicles;
using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.VehicleTypes
{
    [TestClass]
    public class VehicleTypeServiceGetByIdTests
    {
        private Mock<IVehicleTypeRepository> _repo;
        private VehicleTypeService _service;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IVehicleTypeRepository>();
            _service = new VehicleTypeService(_repo.Object);
        }

        [TestMethod]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            _repo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((VehicleType?)null);

            var result = await _service.GetByIdAsync(999, CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_Found_ReturnsMappedResponse()
        {
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new VehicleType
            {
                VehicleTypeId = 1,
                TypeName = "Xe may",
                Description = "Mo ta",
                IsActive = true
            });

            var result = await _service.GetByIdAsync(1, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.VehicleTypeId);
            Assert.AreEqual("Xe may", result.TypeName);
            Assert.AreEqual("Mo ta", result.Description);
            Assert.IsTrue(result.IsActive);
        }

        [TestMethod]
        public async Task GetByIdAsync_FoundInactive_ReturnsMappedResponse()
        {
            _repo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new VehicleType
            {
                VehicleTypeId = 2,
                TypeName = "Xe dien",
                Description = "EV",
                IsActive = false
            });

            var result = await _service.GetByIdAsync(2, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.VehicleTypeId);
            Assert.AreEqual("Xe dien", result.TypeName);
            Assert.AreEqual("EV", result.Description);
            Assert.IsFalse(result.IsActive);
        }

        [TestMethod]
        public async Task GetByIdAsync_DescriptionNull_MapsNull()
        {
            _repo.Setup(x => x.GetByIdAsync(3)).ReturnsAsync(new VehicleType
            {
                VehicleTypeId = 3,
                TypeName = "Xe so",
                Description = null,
                IsActive = true
            });

            var result = await _service.GetByIdAsync(3, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.VehicleTypeId);
            Assert.AreEqual("Xe so", result.TypeName);
            Assert.IsNull(result.Description);
            Assert.IsTrue(result.IsActive);
        }
    }
}
