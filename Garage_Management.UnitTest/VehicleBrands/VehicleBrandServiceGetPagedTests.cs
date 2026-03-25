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

namespace Garage_Management.UnitTest.VehicleBrands
{
    [TestClass]
    public class VehicleBrandServiceGetPagedTests
    {
        private Mock<IVehicleBrandRepository> _repo = null!;
        private VehicleBrandService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IVehicleBrandRepository>();
            _service = new VehicleBrandService(_repo.Object);
        }

        [TestMethod]
        public async Task GetPagedAsync_WithData_ReturnsMappedPagedResult()
        {
            _repo.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<VehicleBrand>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 2,
                    PageData = new List<VehicleBrand>
                    {
                        new VehicleBrand { BrandId = 1, BrandName = "Honda", IsActive = true },
                        new VehicleBrand { BrandId = 2, BrandName = "Yamaha", IsActive = false }
                    }
                });

            var result = await _service.GetPagedAsync(1, 10, CancellationToken.None);

            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(10, result.PageSize);
            Assert.AreEqual(2, result.Total);
            Assert.AreEqual(2, result.PageData.Count());
            Assert.AreEqual(1, result.PageData.First().BrandId);
            Assert.AreEqual("Honda", result.PageData.First().BrandName);
            Assert.IsTrue(result.PageData.First().isActive);
            Assert.AreEqual(2, result.PageData.Last().BrandId);
            Assert.AreEqual("Yamaha", result.PageData.Last().BrandName);
            Assert.IsFalse(result.PageData.Last().isActive);
        }

        [TestMethod]
        public async Task GetPagedAsync_Empty_ReturnsEmptyPagedResult()
        {
            _repo.Setup(x => x.GetPagedAsync(2, 5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<VehicleBrand>
                {
                    Page = 2,
                    PageSize = 5,
                    Total = 0,
                    PageData = new List<VehicleBrand>()
                });

            var result = await _service.GetPagedAsync(2, 5, CancellationToken.None);

            Assert.AreEqual(2, result.Page);
            Assert.AreEqual(5, result.PageSize);
            Assert.AreEqual(0, result.Total);
            Assert.AreEqual(0, result.PageData.Count());
        }
    }
}
