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
    public class StockTransactionServiceGetByIdTests
    {
        [TestMethod]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            repo.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            var result = await service.GetByIdAsync(99);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_Found_ReturnsMappedResponse()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            var entity = new StockTransaction
            {
                StockTransactionId = 1,
                SparePartId = 10,
                TransactionType = TransactionType.Import,
                QuantityChange = 5,
                UnitPrice = 10000,
                Inventory = new Inventory { PartName = "Bugi", PartCode = "BG-001" },
                Supplier = new Supplier { SupplierName = "ABC" }
            };
            repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await service.GetByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.StockTransactionId);
            Assert.AreEqual("Bugi", result.PartName);
            Assert.AreEqual("ABC", result.SupplierName);
            Assert.AreEqual(TransactionType.Import, result.TransactionType);
        }
    }
}
