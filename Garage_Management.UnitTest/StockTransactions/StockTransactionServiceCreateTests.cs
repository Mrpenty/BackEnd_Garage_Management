using Garage_Management.Application.DTOs.Iventories.StockTransactions;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Inventories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.StockTransactions
{
    [TestClass]
    public class StockTransactionServiceCreateTests
    {
        [TestMethod]
        public async Task CreateAsync_SparePartNotFound_Throws()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Inventory?)null);

            var request = new StockTransactionCreateRequest { SparePartId = 1, TransactionType = TransactionType.Import, QuantityChange = 2, UnitPrice = 10000 };

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() => service.CreateAsync(request));
        }

        [TestMethod]
        public async Task CreateAsync_Export_UpdatesInventoryAndCreatesNegativeTransaction()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            var inventory = new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi" };
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(inventory);
            repo.Setup(x => x.AddAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransaction, CancellationToken>((e, _) => e.StockTransactionId = 5)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            var result = await service.CreateAsync(new StockTransactionCreateRequest
            {
                SparePartId = 1,
                TransactionType = TransactionType.ExportToJobCard,
                QuantityChange = 3,
                UnitPrice = 12000
            });

            Assert.AreEqual(5, result.StockTransactionId);
            Assert.AreEqual(7, inventory.Quantity);
        }
    }
}
