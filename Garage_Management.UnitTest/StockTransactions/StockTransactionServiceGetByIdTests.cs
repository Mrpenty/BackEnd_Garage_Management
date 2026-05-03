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
    public class StockTransactionServiceGetByIdTests
    {
        [TestMethod]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            repo.Setup(x => x.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            var result = await service.GetByIdAsync(99);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_Found_ReturnsMappedResponse()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
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

        /// <summary>
        /// UTCID03 - Normal: Lấy giao dịch Export với JobCardId
        /// </summary>
        [TestMethod]
        public async Task GetByIdAsync_Export_ReturnsWithJobCardId()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var entity = new StockTransaction
            {
                StockTransactionId = 2,
                SparePartId = 10,
                TransactionType = TransactionType.ExportToJobCard,
                QuantityChange = -3,
                UnitPrice = 35000,
                JobCardId = 10,
                Inventory = new Inventory { PartName = "Bugi", PartCode = "BG-001" },
                Supplier = null
            };
            repo.Setup(x => x.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await service.GetByIdAsync(2);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.StockTransactionId);
            Assert.AreEqual(TransactionType.ExportToJobCard, result.TransactionType);
            Assert.AreEqual(-3, result.QuantityChange);
            Assert.AreEqual(10, result.JobCardId);
            Assert.IsNull(result.SupplierName);
        }

        /// <summary>
        /// UTCID04 - Normal: Lấy giao dịch Adjustment (không có Supplier/JobCard)
        /// </summary>
        [TestMethod]
        public async Task GetByIdAsync_Adjustment_ReturnsWithoutSupplierOrJobCard()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var entity = new StockTransaction
            {
                StockTransactionId = 3,
                SparePartId = 10,
                TransactionType = TransactionType.Adjustment,
                QuantityChange = -5,
                UnitPrice = 0,
                Inventory = new Inventory { PartName = "Bugi", PartCode = "BG-001" },
                Supplier = null,
                JobCardId = null,
                Note = "Kiểm kê cuối tháng"
            };
            repo.Setup(x => x.GetByIdAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await service.GetByIdAsync(3);

            Assert.IsNotNull(result);
            Assert.AreEqual(TransactionType.Adjustment, result.TransactionType);
            Assert.IsNull(result.SupplierName);
            Assert.IsNull(result.JobCardId);
            Assert.AreEqual("Kiểm kê cuối tháng", result.Note);
        }

        /// <summary>
        /// UTCID05 - Boundary: Id = 0
        /// </summary>
        [TestMethod]
        public async Task GetByIdAsync_ZeroId_ReturnsNull()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            repo.Setup(x => x.GetByIdAsync(0, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            var result = await service.GetByIdAsync(0);

            Assert.IsNull(result);
        }

        /// <summary>
        /// UTCID06 - Abnormal: Id âm
        /// </summary>
        [TestMethod]
        public async Task GetByIdAsync_NegativeId_ReturnsNull()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            repo.Setup(x => x.GetByIdAsync(-1, It.IsAny<CancellationToken>())).ReturnsAsync((StockTransaction?)null);

            var result = await service.GetByIdAsync(-1);

            Assert.IsNull(result);
        }
    }
}
