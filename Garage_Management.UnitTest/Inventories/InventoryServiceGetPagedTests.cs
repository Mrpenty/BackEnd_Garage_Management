using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.UnitTest.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Inventories
{
    [TestClass]
    public class InventoryServiceGetPagedTests
    {
        [TestMethod]
        public async Task GetPagedAsync_WithData_ReturnsSuccessPagedResult()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object);
            var query = new ParamQuery { Page = 1, PageSize = 10 };

            var data = new List<Inventory>
            {
                new Inventory
                {
                    SparePartId = 1,
                    PartCode = "BG-001",
                    PartName = "Bugi NGK",
                    Quantity = 20,
                    IsActive = true,
                    SparePartBrand = new SparePartBrand { BrandName = "NGK" },
                    SparePartCategory = new SparePartCategory { CategoryName = "Đánh lửa" }
                }
            }.AsQueryable();

            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<Inventory>(data));

            var result = await service.GetPagedAsync(query);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.Data.Total);
            Assert.AreEqual(1, result.Data.PageData.Count());
            Assert.AreEqual("Bugi NGK", result.Data.PageData.First().PartName);
        }

        [TestMethod]
        public async Task GetPagedAsync_Empty_ReturnsEmptyPagedResult()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object);
            var query = new ParamQuery { Page = 1, PageSize = 10 };

            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<Inventory>(new List<Inventory>().AsQueryable()));

            var result = await service.GetPagedAsync(query);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.Data.Total);
            Assert.AreEqual(0, result.Data.PageData.Count());
        }
    }
}
