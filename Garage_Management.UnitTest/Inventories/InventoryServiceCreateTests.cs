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
    public class InventoryServiceCreateTests
    {
        [TestMethod]
        public async Task CreateAsync_InvalidPartName_Throws()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new InventoryCreateRequest { PartName = " ", Quantity = 1 }));
        }

        [TestMethod]
        public async Task CreateAsync_Valid_ReturnsResponse()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object);
            repo.Setup(x => x.AddAsync(It.IsAny<Inventory>(), It.IsAny<CancellationToken>()))
                .Callback<Inventory, CancellationToken>((e, _) => e.SparePartId = 11)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(x => x.GetByIdWithDetailsAsync(11, It.IsAny<CancellationToken>())).ReturnsAsync((Inventory?)null);

            var result = await service.CreateAsync(new InventoryCreateRequest { PartCode = "BG-001", PartName = " Bugi NGK ", Quantity = 20, IsActive = true });

            Assert.AreEqual(11, result.SparePartId);
            Assert.AreEqual("Bugi NGK", result.PartName);
        }
    }
}
