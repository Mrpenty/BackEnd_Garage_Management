using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Inventories
{
    [TestClass]
    public class InventoryServiceGetByIdTests
    {
        [TestMethod]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object);
            repo.Setup(x => x.GetByIdWithDetailsAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Inventory?)null);

            var result = await service.GetByIdAsync(99);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_Found_ReturnsMappedResponse()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object);
            var entity = new Inventory
            {
                SparePartId = 1,
                PartCode = "BG-001",
                PartName = "Bugi NGK",
                Quantity = 10,
                IsActive = true,
                SparePartBrand = new SparePartBrand { BrandName = "NGK" },
                SparePartCategory = new SparePartCategory { CategoryName = "Bugi" }
            };
            repo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await service.GetByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.SparePartId);
            Assert.AreEqual("Bugi NGK", result.PartName);
            Assert.AreEqual("NGK", result.SparePartBrandName);
            Assert.AreEqual("Bugi", result.CategoryName);
        }
    }
}
