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
    public class StockTransactionServiceCreateReturnTests
    {
        [TestMethod]
        public async Task CreateAsync_Return_IncreasesInventory()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var inventory = new Inventory { SparePartId = 1, Quantity = 7, PartName = "Bugi" };
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(inventory);
            repo.Setup(x => x.AddAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransaction, CancellationToken>((e, _) => e.StockTransactionId = 2)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            var result = await service.CreateAsync(new StockTransactionCreateRequest
            {
                SparePartId = 1,
                TransactionType = TransactionType.ReturnFromJobCard,
                QuantityChange = 3,
                UnitPrice = 0
            });

            Assert.AreEqual(10, inventory.Quantity);
            Assert.AreEqual(2, result.StockTransactionId);
        }

        [TestMethod]
        public async Task CreateAsync_Return_ZeroQuantity_Throws()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi" });

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new StockTransactionCreateRequest
                {
                    SparePartId = 1,
                    TransactionType = TransactionType.ReturnFromJobCard,
                    QuantityChange = 0,
                    UnitPrice = 0
                }));
            Assert.AreEqual("QuantityChange phải lớn hơn 0", ex.Message);
        }

        /// <summary>
        /// UTCID03 - Boundary: Return QuantityChange=1 (biên min)
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Return_MinBoundaryQuantity_IncreasesByOne()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var inventory = new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi" };
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(inventory);
            repo.Setup(x => x.AddAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransaction, CancellationToken>((e, _) => e.StockTransactionId = 2)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            await service.CreateAsync(new StockTransactionCreateRequest
            {
                SparePartId = 1,
                TransactionType = TransactionType.ReturnFromJobCard,
                QuantityChange = 1,
                JobCardId = 10,
                UnitPrice = 0
            });

            Assert.AreEqual(11, inventory.Quantity);
        }

        /// <summary>
        /// UTCID04 - Abnormal: Return QuantityChange âm
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Return_NegativeQuantity_Throws()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Inventory { SparePartId = 1, Quantity = 10, PartName = "Bugi" });

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new StockTransactionCreateRequest
                {
                    SparePartId = 1,
                    TransactionType = TransactionType.ReturnFromJobCard,
                    QuantityChange = -2,
                    JobCardId = 10,
                    UnitPrice = 0
                }));
            Assert.AreEqual("QuantityChange phải lớn hơn 0", ex.Message);
        }

        /// <summary>
        /// UTCID05 - Abnormal: Return với SparePartId không tồn tại
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Return_SparePartIdNotFound_Throws()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            inventoryRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Inventory?)null);

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new StockTransactionCreateRequest
                {
                    SparePartId = 999,
                    TransactionType = TransactionType.ReturnFromJobCard,
                    QuantityChange = 3,
                    JobCardId = 10
                }));
            Assert.AreEqual("SparePartId không tồn tại", ex.Message);
        }

        /// <summary>
        /// UTCID06 - Normal: Return kế thừa BranchId từ Inventory và lưu JobCardId
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Return_StoresJobCardIdAndBranchId()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var inventory = new Inventory { SparePartId = 1, BranchId = 3, Quantity = 10, PartName = "Bugi" };
            inventoryRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(inventory);

            StockTransaction? captured = null;
            repo.Setup(x => x.AddAsync(It.IsAny<StockTransaction>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransaction, CancellationToken>((e, _) =>
                {
                    e.StockTransactionId = 2;
                    captured = e;
                })
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            repo.Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            await service.CreateAsync(new StockTransactionCreateRequest
            {
                SparePartId = 1,
                TransactionType = TransactionType.ReturnFromJobCard,
                QuantityChange = 2,
                JobCardId = 10,
                UnitPrice = 0
            });

            Assert.AreEqual(10, captured?.JobCardId);
            Assert.AreEqual(3, captured?.BranchId);
        }
    }
}
