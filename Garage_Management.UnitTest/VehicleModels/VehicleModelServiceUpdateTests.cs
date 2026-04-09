using Garage_Management.Application.DTOs.Vehicles.VehicleModel;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Vehicles;
using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.VehicleModels
{
    [TestClass]
    public class VehicleModelServiceUpdateTests
    {
        [TestMethod]
        public async Task UpdateAsync_HasVehicles_Throws()
        {
            var repo = new Mock<IVehicleModelRepository>();
            var brandRepo = new Mock<IVehicleBrandRepository>();
            var service = new VehicleModelService(repo.Object, brandRepo.Object);

            repo.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(new VehicleModel { ModelId = 5, BrandId = 1, ModelName = "Old", VehicleTypeId = 1 });
            brandRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new VehicleBrand { BrandId = 1, BrandName = "Honda" });
            repo.Setup(x => x.ExistsAsync(1, 1, "Vision", 5, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.HasVehiclesAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.UpdateAsync(5, new VehicleModelUpdate { TypeId = 1, BrandId = 1, ModelName = "Vision", isActive = true }));
        }

        [TestMethod]
        public async Task UpdateAsync_NotFound_ReturnsNull()
        {
            var repo = new Mock<IVehicleModelRepository>();
            var brandRepo = new Mock<IVehicleBrandRepository>();
            var service = new VehicleModelService(repo.Object, brandRepo.Object);

            repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((VehicleModel?)null);

            var result = await service.UpdateAsync(99, new VehicleModelUpdate { TypeId = 1, BrandId = 1, ModelName = "Test", isActive = true });

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateAsync_BrandNotFound_Throws()
        {
            var repo = new Mock<IVehicleModelRepository>();
            var brandRepo = new Mock<IVehicleBrandRepository>();
            var service = new VehicleModelService(repo.Object, brandRepo.Object);

            repo.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(new VehicleModel { ModelId = 5, BrandId = 1, ModelName = "Old", VehicleTypeId = 1 });
            brandRepo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((VehicleBrand?)null);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.UpdateAsync(5, new VehicleModelUpdate { TypeId = 1, BrandId = 99, ModelName = "Vision", isActive = true }));
        }

        [TestMethod]
        public async Task UpdateAsync_Valid_ReturnsUpdatedResponse()
        {
            var repo = new Mock<IVehicleModelRepository>();
            var brandRepo = new Mock<IVehicleBrandRepository>();
            var service = new VehicleModelService(repo.Object, brandRepo.Object);

            repo.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(new VehicleModel { ModelId = 5, BrandId = 1, ModelName = "Old", VehicleTypeId = 1 });
            brandRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new VehicleBrand { BrandId = 2, BrandName = "Yamaha" });
            repo.Setup(x => x.ExistsAsync(2, 1, "Exciter", 5, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.HasVehiclesAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var result = await service.UpdateAsync(5, new VehicleModelUpdate { TypeId = 1, BrandId = 2, ModelName = "Exciter", isActive = true });

            Assert.IsNotNull(result);
            Assert.AreEqual("Exciter", result.ModelName);
            Assert.AreEqual(2, result.BrandId);
            Assert.IsTrue(result.isActive);
            repo.Verify(x => x.Update(It.IsAny<VehicleModel>()), Times.Once);
            repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
