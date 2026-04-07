using Garage_Management.Application.DTOs.Vehicles;
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
    public class VehicleServiceUpdateTests
    {
        private Mock<IVehicleRepository> _repo = null!;
        private Mock<ICustomerRepository> _customerRepo = null!;
        private Mock<IVehicleModelRepository> _modelRepo = null!;
        private VehicleService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IVehicleRepository>();
            _customerRepo = new Mock<ICustomerRepository>();
            _modelRepo = new Mock<IVehicleModelRepository>();

            _service = new VehicleService(
                _repo.Object,
                _customerRepo.Object,
                _modelRepo.Object,
                new Mock<IHttpContextAccessor>().Object);
        }

        [TestMethod]
        public async Task UpdateAsync_NotFound_ReturnsNull()
        {
            _repo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync((Vehicle?)null);

            var result = await _service.UpdateAsync(10, new VehicleUpdateRequest(), CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateAsync_ModelNotFound_Throws()
        {
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Vehicle { VehicleId = 1, BrandId = 1, ModelId = 1 });
            _modelRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((VehicleModel?)null);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                _service.UpdateAsync(1, new VehicleUpdateRequest { ModelId = 999 }, CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateAsync_ValidRequest_UpdatesBrandAndModel()
        {
            var entity = new Vehicle
            {
                VehicleId = 1,
                CustomerId = 1,
                BrandId = 1,
                ModelId = 1,
                LicensePlate = "29A11111"
            };
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _modelRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new VehicleModel { ModelId = 2, BrandId = 5 });
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.UpdateAsync(1, new VehicleUpdateRequest
            {
                ModelId = 2,
                LicensePlate = "29A22222",
                Year = 2024,
                Vin = "VIN-002",
                UpdatedBy = 12
            }, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.ModelId);
            Assert.AreEqual(5, entity.BrandId);
            Assert.AreEqual(12, result.UpdatedBy);
            _repo.Verify(x => x.Update(It.IsAny<Vehicle>()), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

