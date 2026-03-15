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

namespace Garage_Management.UnitTest.VehicleTypes
{
    [TestClass]
    public class VehicleTypeServiceGetPagedTests
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
        public async Task GetPagedAsync_WithData_ReturnsMappedPagedResult()
        {
            _repo.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<VehicleType>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 2,
                    PageData = new List<VehicleType>
                    {
                        new VehicleType { VehicleTypeId = 1, TypeName = "Xe may", Description = "2 banh", IsActive = true },
                        new VehicleType { VehicleTypeId = 2, TypeName = "Xe dien", Description = "Dien", IsActive = false }
                    }
                });

            var result = await _service.GetPagedAsync(1, 10, CancellationToken.None);

            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(10, result.PageSize);
            Assert.AreEqual(2, result.Total);
            Assert.AreEqual(2, result.PageData.Count());
            Assert.AreEqual("Xe may", result.PageData.First().TypeName);
        }

        [TestMethod]
        public async Task GetPagedAsync_ItemWithNullDescription_MapsNullDescription()
        {
            _repo.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<VehicleType>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 1,
                    PageData = new List<VehicleType>
                    {
                        new VehicleType { VehicleTypeId = 1, TypeName = "Xe may", Description = null, IsActive = true }
                    }
                });

            var result = await _service.GetPagedAsync(1, 10, CancellationToken.None);

            Assert.AreEqual(1, result.Total);
            var item = result.PageData.First();
            Assert.AreEqual(1, item.VehicleTypeId);
            Assert.AreEqual("Xe may", item.TypeName);
            Assert.IsNull(item.Description);
            Assert.IsTrue(item.IsActive);
        }

        [TestMethod]
        public async Task GetPagedAsync_WithData_MapsAllFieldsCorrectly()
        {
            _repo.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<VehicleType>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 2,
                    PageData = new List<VehicleType>
                    {
                        new VehicleType { VehicleTypeId = 1, TypeName = "Xe may", Description = "2 banh", IsActive = true },
                        new VehicleType { VehicleTypeId = 2, TypeName = "Xe dien", Description = "Dien", IsActive = false }
                    }
                });

            var result = await _service.GetPagedAsync(1, 10, CancellationToken.None);

            var items = result.PageData.ToList();

            Assert.AreEqual(1, items[0].VehicleTypeId);
            Assert.AreEqual("Xe may", items[0].TypeName);
            Assert.AreEqual("2 banh", items[0].Description);
            Assert.IsTrue(items[0].IsActive);

            Assert.AreEqual(2, items[1].VehicleTypeId);
            Assert.AreEqual("Xe dien", items[1].TypeName);
            Assert.AreEqual("Dien", items[1].Description);
            Assert.IsFalse(items[1].IsActive);
        }

        [TestMethod]
        public async Task GetPagedAsync_Empty_ReturnsEmptyPagedResult()
        {
            _repo.Setup(x => x.GetPagedAsync(2, 5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<VehicleType>
                {
                    Page = 2,
                    PageSize = 5,
                    Total = 0,
                    PageData = new List<VehicleType>()
                });

            var result = await _service.GetPagedAsync(2, 5, CancellationToken.None);

            Assert.AreEqual(2, result.Page);
            Assert.AreEqual(5, result.PageSize);
            Assert.AreEqual(0, result.Total);
            Assert.AreEqual(0, result.PageData.Count());
        }
    }
}
