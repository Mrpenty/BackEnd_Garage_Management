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
    }
}
