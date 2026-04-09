using Garage_Management.Application.DTOs.Iventories;
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
    public class InventoryServiceUpdateTests
    {
        [TestMethod]
        public async Task UpdateAsync_NotFound_ReturnsNull()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Inventory?)null);

            var result = await service.UpdateAsync(99, new InventoryUpdateRequest { PartName = "Test" });

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateAsync_Valid_UpdatesFieldsAndReturnsResponse()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object);
            var entity = new Inventory { SparePartId = 1, PartName = "Bugi", PartCode = "BG-001", Quantity = 10 };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await service.UpdateAsync(1, new InventoryUpdateRequest
            {
                PartName = "Bugi NGK",
                SellingPrice = 50000m,
                MinQuantity = 5
            });

            Assert.IsNotNull(result);
            Assert.AreEqual("Bugi NGK", entity.PartName);
            Assert.AreEqual(50000m, entity.SellingPrice);
            Assert.AreEqual(5, entity.MinQuantity);
        }

        [TestMethod]
        public async Task UpdateAsync_NegativeMinQuantity_Throws()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object);
            var entity = new Inventory { SparePartId = 1, PartName = "Bugi", Quantity = 10 };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.UpdateAsync(1, new InventoryUpdateRequest { MinQuantity = -1 }));
        }

        [TestMethod]
        public async Task UpdateAsync_NegativeLastPurchasePrice_Throws()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object);
            var entity = new Inventory { SparePartId = 1, PartName = "Bugi", Quantity = 10 };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.UpdateAsync(1, new InventoryUpdateRequest { LastPurchasePrice = -100m }));
        }

        [TestMethod]
        public async Task UpdateAsync_NegativeSellingPrice_Throws()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object);
            var entity = new Inventory { SparePartId = 1, PartName = "Bugi", Quantity = 10 };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.UpdateAsync(1, new InventoryUpdateRequest { SellingPrice = -100m }));
        }
    }
}
