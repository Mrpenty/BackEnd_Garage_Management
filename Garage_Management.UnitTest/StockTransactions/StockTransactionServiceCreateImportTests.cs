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
    public class StockTransactionServiceCreateImportTests
    {
        [TestMethod]
        public async Task CreateAsync_Import_IncreasesInventoryAndUpdatesLastPurchasePrice()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            var inventory = new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi", LastPurchasePrice = 5000 };
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(inventory);
            repo.Setup(x => x.AddAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransaction, CancellationToken>((e, _) => e.StockTransactionId = 1)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            var result = await service.CreateAsync(new StockTransactionCreateRequest
            {
                SparePartId = 1,
                TransactionType = TransactionType.Import,
                QuantityChange = 5,
                UnitPrice = 15000
            });

            Assert.AreEqual(15, inventory.Quantity);
            Assert.AreEqual(15000, inventory.LastPurchasePrice);
        }

        [TestMethod]
        public async Task CreateAsync_Import_ZeroUnitPrice_DoesNotUpdateLastPurchasePrice()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            var inventory = new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi", LastPurchasePrice = 5000 };
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(inventory);
            repo.Setup(x => x.AddAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransaction, CancellationToken>((e, _) => e.StockTransactionId = 1)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            await service.CreateAsync(new StockTransactionCreateRequest
            {
                SparePartId = 1,
                TransactionType = TransactionType.Import,
                QuantityChange = 5,
                UnitPrice = 0
            });

            Assert.AreEqual(15, inventory.Quantity);
            Assert.AreEqual(5000, inventory.LastPurchasePrice);
        }

        [TestMethod]
        public async Task CreateAsync_Import_ZeroQuantity_Throws()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi" });

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new StockTransactionCreateRequest
                {
                    SparePartId = 1,
                    TransactionType = TransactionType.Import,
                    QuantityChange = 0,
                    UnitPrice = 10000
                }));
            Assert.AreEqual("QuantityChange phải lớn hơn 0", ex.Message);
        }

        /// <summary>
        /// UTCID04 - Boundary: Import QuantityChange=1 (biên min hợp lệ)
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Import_MinBoundaryQuantity_IncreasesByOne()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            var inventory = new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi", LastPurchasePrice = 5000 };
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(inventory);
            repo.Setup(x => x.AddAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransaction, CancellationToken>((e, _) => e.StockTransactionId = 1)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            var result = await service.CreateAsync(new StockTransactionCreateRequest
            {
                SparePartId = 1,
                TransactionType = TransactionType.Import,
                QuantityChange = 1,
                UnitPrice = 15000
            });

            Assert.AreEqual(11, inventory.Quantity);
            Assert.AreEqual(15000, inventory.LastPurchasePrice);
        }

        /// <summary>
        /// UTCID05 - Abnormal: SparePartId không tồn tại
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Import_SparePartIdNotFound_Throws()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            inventoryRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Inventory?)null);

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new StockTransactionCreateRequest
                {
                    SparePartId = 999,
                    TransactionType = TransactionType.Import,
                    QuantityChange = 5,
                    UnitPrice = 15000
                }));
            Assert.AreEqual("SparePartId không tồn tại", ex.Message);
        }

        /// <summary>
        /// UTCID06 - Abnormal: Import QuantityChange âm
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Import_NegativeQuantity_Throws()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi" });

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new StockTransactionCreateRequest
                {
                    SparePartId = 1,
                    TransactionType = TransactionType.Import,
                    QuantityChange = -3,
                    UnitPrice = 15000
                }));
            Assert.AreEqual("QuantityChange phải lớn hơn 0", ex.Message);
        }

        /// <summary>
        /// UTCID07 - Normal: Import kế thừa BranchId từ Inventory
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Import_InheritsBranchIdFromInventory()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            var inventory = new Inventory { SparePartId = 1, BranchId = 2, Quantity = 10, PartName = "Bugi" };
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(inventory);

            StockTransaction? captured = null;
            repo.Setup(x => x.AddAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransaction, CancellationToken>((e, _) =>
                {
                    e.StockTransactionId = 1;
                    captured = e;
                })
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            await service.CreateAsync(new StockTransactionCreateRequest
            {
                SparePartId = 1,
                TransactionType = TransactionType.Import,
                QuantityChange = 5,
                UnitPrice = 15000
            });

            Assert.AreEqual(2, captured?.BranchId);
        }
    }
}
