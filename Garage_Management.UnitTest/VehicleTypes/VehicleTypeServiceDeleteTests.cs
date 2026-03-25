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
    public class VehicleTypeServiceDeleteTests
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
        public async Task DeleteAsync_NotFound_ReturnsFalse()
        {
            _repo.Setup(x => x.GetByIdAsync(30)).ReturnsAsync((VehicleType?)null);

            var result = await _service.DeleteAsync(30, CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeleteAsync_HasModels_Throws()
        {
            var entity = new VehicleType { VehicleTypeId = 31, TypeName = "Xe may", IsActive = true };
            _repo.Setup(x => x.GetByIdAsync(31)).ReturnsAsync(entity);
            _repo.Setup(x => x.HasModelsAsync(31, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(
                () => _service.DeleteAsync(31, CancellationToken.None));
        }

        [TestMethod]
        public async Task DeleteAsync_HasServiceMappings_Throws()
        {
            var entity = new VehicleType { VehicleTypeId = 32, TypeName = "Xe dien", IsActive = true };
            _repo.Setup(x => x.GetByIdAsync(32)).ReturnsAsync(entity);
            _repo.Setup(x => x.HasModelsAsync(32, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.HasServiceMappingsAsync(32, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(
                () => _service.DeleteAsync(32, CancellationToken.None));
        }

        [TestMethod]
        public async Task DeleteAsync_NoDependencies_ReturnsTrue()
        {
            var entity = new VehicleType { VehicleTypeId = 33, TypeName = "Xe so", IsActive = true };
            _repo.Setup(x => x.GetByIdAsync(33)).ReturnsAsync(entity);
            _repo.Setup(x => x.HasModelsAsync(33, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.HasServiceMappingsAsync(33, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.DeleteAsync(33, CancellationToken.None);

            Assert.IsTrue(result);
            _repo.Verify(x => x.Delete(entity), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
