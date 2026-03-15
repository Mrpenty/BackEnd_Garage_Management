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
    public class VehicleTypeServiceActivateTests
    {
        private Mock<IVehicleTypeRepository> _repo = null!;
        private VehicleTypeService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IVehicleTypeRepository>();
            _service = new VehicleTypeService(_repo.Object);
        }

        [TestMethod]
        public async Task ActivateAsync_NotFound_ReturnsNull()
        {
            _repo.Setup(x => x.GetByIdAsync(20)).ReturnsAsync((VehicleType?)null);

            var result = await _service.ActivateAsync(20, CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task ActivateAsync_AlreadyActive_ReturnsResponse()
        {
            var entity = new VehicleType
            {
                VehicleTypeId = 21,
                TypeName = "Xe may",
                IsActive = true
            };

            _repo.Setup(x => x.GetByIdAsync(21)).ReturnsAsync(entity);

            var result = await _service.ActivateAsync(21, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsActive);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task ActivateAsync_Inactive_UpdatesToActive()
        {
            var entity = new VehicleType
            {
                VehicleTypeId = 22,
                TypeName = "Xe dien",
                IsActive = false
            };

            _repo.Setup(x => x.GetByIdAsync(22)).ReturnsAsync(entity);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.ActivateAsync(22, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsActive);
            _repo.Verify(x => x.Update(entity), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
