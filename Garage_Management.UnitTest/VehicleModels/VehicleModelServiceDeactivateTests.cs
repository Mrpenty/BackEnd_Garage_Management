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
    public class VehicleModelServiceDeactivateTests
    {
        [TestMethod]
        public async Task DeActiveAsync_NotFound_ReturnsFalse()
        {
            var repo = new Mock<IVehicleModelRepository>();
            var brandRepo = new Mock<IVehicleBrandRepository>();
            var service = new VehicleModelService(repo.Object, brandRepo.Object);

            repo.Setup(x => x.GetByIdAsync(9)).ReturnsAsync((VehicleModel?)null);

            var result = await service.DeActiveAsync(9);

            Assert.IsFalse(result);
        }
    }
}
