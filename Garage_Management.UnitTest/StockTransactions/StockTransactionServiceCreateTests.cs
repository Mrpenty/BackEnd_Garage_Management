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

        /// <summary>
        /// UTCID03 - Boundary: Export Qty=1 (biên min)
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Export_MinBoundaryQuantity_DecreasesByOne()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            var inventory = new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi" };
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(inventory);
            repo.Setup(x => x.AddAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransaction, CancellationToken>((e, _) => e.StockTransactionId = 1)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            await service.CreateAsync(new StockTransactionCreateRequest
            {
                SparePartId = 1,
                TransactionType = TransactionType.ExportToJobCard,
                QuantityChange = 1,
                JobCardId = 10,
                UnitPrice = 12000
            });

            Assert.AreEqual(9, inventory.Quantity);
        }

        /// <summary>
        /// UTCID04 - Boundary: Export toàn bộ tồn kho (Qty = Inventory.Quantity)
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Export_EqualInventory_EmptiesStock()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            var inventory = new Inventory { SparePartId = 1, Quantity = 20, PartName = "Bugi" };
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(inventory);
            repo.Setup(x => x.AddAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransaction, CancellationToken>((e, _) => e.StockTransactionId = 1)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            await service.CreateAsync(new StockTransactionCreateRequest
            {
                SparePartId = 1,
                TransactionType = TransactionType.ExportToJobCard,
                QuantityChange = 20,
                JobCardId = 10,
                UnitPrice = 12000
            });

            Assert.AreEqual(0, inventory.Quantity);
        }

        /// <summary>
        /// UTCID05 - Abnormal: Export Qty vượt tồn kho (nextQuantity < 0)
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Export_ExceedsInventory_Throws()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi" });

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new StockTransactionCreateRequest
                {
                    SparePartId = 1,
                    TransactionType = TransactionType.ExportToJobCard,
                    QuantityChange = 25,
                    JobCardId = 10,
                    UnitPrice = 12000
                }));
            Assert.AreEqual("Số lượng tồn kho không đủ", ex.Message);
        }

        /// <summary>
        /// UTCID06 - Abnormal: Export Qty=0
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Export_ZeroQuantity_Throws()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi" });

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new StockTransactionCreateRequest
                {
                    SparePartId = 1,
                    TransactionType = TransactionType.ExportToJobCard,
                    QuantityChange = 0,
                    JobCardId = 10,
                    UnitPrice = 12000
                }));
            Assert.AreEqual("QuantityChange phải lớn hơn 0", ex.Message);
        }

        /// <summary>
        /// UTCID07 - Abnormal: Export Qty âm
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Export_NegativeQuantity_Throws()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi" });

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new StockTransactionCreateRequest
                {
                    SparePartId = 1,
                    TransactionType = TransactionType.ExportToJobCard,
                    QuantityChange = -5,
                    JobCardId = 10,
                    UnitPrice = 12000
                }));
            Assert.AreEqual("QuantityChange phải lớn hơn 0", ex.Message);
        }

        /// <summary>
        /// UTCID08 - Normal: Export kế thừa BranchId và lưu QuantityChange âm
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Export_StoresNegativeQuantityAndBranchId()
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
                    e.StockTransactionId = 5;
                    captured = e;
                })
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(x => x.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            await service.CreateAsync(new StockTransactionCreateRequest
            {
                SparePartId = 1,
                TransactionType = TransactionType.ExportToJobCard,
                QuantityChange = 3,
                JobCardId = 10,
                UnitPrice = 12000
            });

            Assert.AreEqual(-3, captured?.QuantityChange);
            Assert.AreEqual(2, captured?.BranchId);
        }
    }
}
