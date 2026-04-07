using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Vehicles;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.VehicleModels
{
    [TestClass]
    public class VehicleModelServiceGetPagedTests
    {
        [TestMethod]
        public async Task GetPagedAsync_ReturnsMapped()
        {
            var repo = new Mock<IVehicleModelRepository>();
            var brandRepo = new Mock<IVehicleBrandRepository>();
            var service = new VehicleModelService(repo.Object, brandRepo.Object);

            repo.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<VehicleModel>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 1,
                    PageData = new List<VehicleModel>
                    {
                        new VehicleModel{ ModelId = 1, BrandId = 2, VehicleTypeId = 3, ModelName = "Exciter", IsActive = true }
                    }
                });

            var result = await service.GetPagedAsync(1, 10);

            Assert.AreEqual(1, result.Total);
            Assert.AreEqual(1, result.PageData.Count());
        }
    }
}
