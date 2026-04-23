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
    public class VehicleServiceDeleteTests
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
        public async Task DeleteAsync_NotFound_ReturnsFalse()
        {
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Vehicle?)null);

            var result = await _service.DeleteAsync(1, CancellationToken.None);

            Assert.IsFalse(result);
            _repo.Verify(x => x.Delete(It.IsAny<Vehicle>()), Times.Never);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task DeleteAsync_HasAppointments_Throws()
        {
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Vehicle { VehicleId = 1 });
            _repo.Setup(x => x.HasAppointmentsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                _service.DeleteAsync(1, CancellationToken.None));

            Assert.AreEqual("Không thể xóa vì đang có xe liên kết", ex.Message);
            _repo.Verify(x => x.Delete(It.IsAny<Vehicle>()), Times.Never);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task DeleteAsync_NoDependencies_ReturnsTrue()
        {
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Vehicle { VehicleId = 1 });
            _repo.Setup(x => x.HasAppointmentsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.DeleteAsync(1, CancellationToken.None);

            Assert.IsTrue(result);
            _repo.Verify(x => x.GetByIdAsync(1), Times.Once);
            _repo.Verify(x => x.Delete(It.IsAny<Vehicle>()), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_SaveFails_Throws()
        {
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Vehicle { VehicleId = 1 });
            _repo.Setup(x => x.HasAppointmentsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception("db error"));

            await Assert.ThrowsExceptionAsync<System.Exception>(() =>
                _service.DeleteAsync(1, CancellationToken.None));

            _repo.Verify(x => x.Delete(It.IsAny<Vehicle>()), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

