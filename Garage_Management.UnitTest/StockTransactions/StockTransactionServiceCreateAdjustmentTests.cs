using Garage_Management.Application.DTOs.Iventories.StockTransactions;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Inventories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.UnitTest.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.StockTransactions
{
    [TestClass]
    public class StockTransactionServiceCreateAdjustmentTests
    {
        [TestMethod]
        public async Task CreateAsync_Adjustment_WithoutActualQuantity_Throws()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi" });

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new StockTransactionCreateRequest
                {
                    SparePartId = 1,
                    TransactionType = TransactionType.Adjustment,
                    QuantityChange = 0,
                    ActualQuantity = null
                }));
        }

        [TestMethod]
        public async Task CreateAsync_Adjustment_NegativeActualQuantity_Throws()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi" });

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new StockTransactionCreateRequest
                {
                    SparePartId = 1,
                    TransactionType = TransactionType.Adjustment,
                    QuantityChange = 0,
                    ActualQuantity = -5
                }));
        }

        [TestMethod]
        public async Task CreateAsync_Adjustment_SameQuantity_NoChange_Throws()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi" });

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new StockTransactionCreateRequest
                {
                    SparePartId = 1,
                    TransactionType = TransactionType.Adjustment,
                    QuantityChange = 0,
                    ActualQuantity = 10
                }));
        }

        [TestMethod]
        public async Task CreateAsync_Adjustment_IncreaseQuantity_UpdatesInventory()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var inventory = new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi" };
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(inventory);
            repo.Setup(x => x.AddAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransaction, CancellationToken>((e, _) => e.StockTransactionId = 3)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(x => x.GetByIdAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            var result = await service.CreateAsync(new StockTransactionCreateRequest
            {
                SparePartId = 1,
                TransactionType = TransactionType.Adjustment,
                QuantityChange = 0,
                ActualQuantity = 15
            });

            Assert.AreEqual(15, inventory.Quantity);
        }

        [TestMethod]
        public async Task CreateAsync_Adjustment_DecreaseQuantity_UpdatesInventory()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var inventory = new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi" };
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(inventory);
            repo.Setup(x => x.AddAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransaction, CancellationToken>((e, _) => e.StockTransactionId = 4)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(x => x.GetByIdAsync(4, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            var result = await service.CreateAsync(new StockTransactionCreateRequest
            {
                SparePartId = 1,
                TransactionType = TransactionType.Adjustment,
                QuantityChange = 0,
                ActualQuantity = 3
            });

            Assert.AreEqual(3, inventory.Quantity);
        }

        [TestMethod]
        public async Task CreateAsync_Export_InsufficientInventory_Throws()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Inventory { SparePartId = 1, Quantity = 2, PartName = "Bugi" });

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new StockTransactionCreateRequest
                {
                    SparePartId = 1,
                    TransactionType = TransactionType.ExportToJobCard,
                    QuantityChange = 5,
                    UnitPrice = 10000
                }));
        }

        /// <summary>
        /// UTCID07 - Boundary: Adjustment về 0 (biên min) - kho rỗng hoàn toàn
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Adjustment_ZeroActualQuantity_UpdatesInventoryToZero()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var inventory = new Inventory { SparePartId = 1, Quantity = 20, PartName = "Bugi" };
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(inventory);
            repo.Setup(x => x.AddAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransaction, CancellationToken>((e, _) => e.StockTransactionId = 7)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(x => x.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            await service.CreateAsync(new StockTransactionCreateRequest
            {
                SparePartId = 1,
                TransactionType = TransactionType.Adjustment,
                QuantityChange = 0,
                ActualQuantity = 0
            });

            Assert.AreEqual(0, inventory.Quantity);
        }
    }
}
