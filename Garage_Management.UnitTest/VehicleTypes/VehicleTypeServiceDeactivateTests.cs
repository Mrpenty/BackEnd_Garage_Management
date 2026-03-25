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
    public class VehicleTypeServiceDeactivateTests
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
        public async Task DeactivateAsync_NotFound_ReturnsNull()
        {
            _repo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync((VehicleType?)null);

            var result = await _service.DeactivateAsync(10, CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DeactivateAsync_AlreadyInactive_ReturnsResponse()
        {
            var entity = new VehicleType
            {
                VehicleTypeId = 11,
                TypeName = "Xe may",
                IsActive = false
            };

            _repo.Setup(x => x.GetByIdAsync(11)).ReturnsAsync(entity);

            var result = await _service.DeactivateAsync(11, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsActive);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task DeactivateAsync_Active_UpdatesToInactive()
        {
            var entity = new VehicleType
            {
                VehicleTypeId = 12,
                TypeName = "Xe dien",
                IsActive = true
            };

            _repo.Setup(x => x.GetByIdAsync(12)).ReturnsAsync(entity);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.DeactivateAsync(12, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsActive);
            _repo.Verify(x => x.Update(entity), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
