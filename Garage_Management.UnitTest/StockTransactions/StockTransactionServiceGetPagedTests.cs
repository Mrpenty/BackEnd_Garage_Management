using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Inventories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.UnitTest.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.StockTransactions
{
    [TestClass]
    public class StockTransactionServiceGetPagedTests
    {
        [TestMethod]
        public async Task GetPagedAsync_ReturnsMappedPagedResult()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var query = new ParamQuery { Page = 1, PageSize = 10 };

            var data = new List<StockTransaction>
            {
                new StockTransaction
                {
                    StockTransactionId = 1,
                    SparePartId = 10,
                    TransactionType = TransactionType.Import,
                    QuantityChange = 5,
                    UnitPrice = 10000,
                    Inventory = new Inventory { PartName = "Bugi", PartCode = "BG-001" },
                    Supplier = new Supplier { SupplierName = "ABC" }
                }
            }.AsQueryable();

            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<StockTransaction>(data));

            var result = await service.GetPagedAsync(query);

            Assert.AreEqual(1, result.Total);
            Assert.AreEqual(1, result.PageData.Count());
            Assert.AreEqual("Bugi", result.PageData.First().PartName);
        }

        [TestMethod]
        public async Task GetPagedAsync_FilterImport_ReturnsOnlyImport()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var query = new ParamQuery { Page = 1, PageSize = 10, Filter = "import" };

            var data = BuildMixedTransactions().AsQueryable();
            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<StockTransaction>(data));

            var result = await service.GetPagedAsync(query);

            Assert.IsTrue(result.PageData.All(x => x.TransactionType == TransactionType.Import));
        }

        /// <summary>
        /// UTCID03 - Normal: DB rỗng → total=0, pageData rỗng
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_Empty_ReturnsEmptyPagedResult()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var query = new ParamQuery { Page = 1, PageSize = 10 };

            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<StockTransaction>(new List<StockTransaction>().AsQueryable()));

            var result = await service.GetPagedAsync(query);

            Assert.AreEqual(0, result.Total);
            Assert.AreEqual(0, result.PageData.Count());
        }

        /// <summary>
        /// UTCID04 - Normal: Filter "export" → chỉ trả ExportToJobCard
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_FilterExport_ReturnsOnlyExport()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var query = new ParamQuery { Page = 1, PageSize = 10, Filter = "export" };

            var data = BuildMixedTransactions().AsQueryable();
            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<StockTransaction>(data));

            var result = await service.GetPagedAsync(query);

            Assert.IsTrue(result.PageData.All(x => x.TransactionType == TransactionType.ExportToJobCard));
        }

        /// <summary>
        /// UTCID05 - Normal: Filter "return" → chỉ trả ReturnFromJobCard
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_FilterReturn_ReturnsOnlyReturn()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var query = new ParamQuery { Page = 1, PageSize = 10, Filter = "return" };

            var data = BuildMixedTransactions().AsQueryable();
            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<StockTransaction>(data));

            var result = await service.GetPagedAsync(query);

            Assert.IsTrue(result.PageData.All(x => x.TransactionType == TransactionType.ReturnFromJobCard));
        }

        /// <summary>
        /// UTCID06 - Normal: Filter "adjust" → chỉ trả Adjustment
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_FilterAdjust_ReturnsOnlyAdjustment()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var query = new ParamQuery { Page = 1, PageSize = 10, Filter = "adjust" };

            var data = BuildMixedTransactions().AsQueryable();
            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<StockTransaction>(data));

            var result = await service.GetPagedAsync(query);

            Assert.IsTrue(result.PageData.All(x => x.TransactionType == TransactionType.Adjustment));
        }

        /// <summary>
        /// UTCID07 - Normal: Search theo tên phụ tùng
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_SearchByPartName_ReturnsMatches()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var query = new ParamQuery { Page = 1, PageSize = 10, Search = "bugi" };

            var data = BuildMixedTransactions().AsQueryable();
            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<StockTransaction>(data));

            var result = await service.GetPagedAsync(query);

            Assert.IsTrue(result.PageData.All(x => (x.PartName ?? string.Empty).ToLower().Contains("bugi")));
        }

        /// <summary>
        /// UTCID08 - Abnormal: PageSize âm → service chỉnh về default 10
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_NegativePageSize_UsesDefault10()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var query = new ParamQuery { Page = 1, PageSize = -5 };

            var data = BuildMixedTransactions().AsQueryable();
            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<StockTransaction>(data));

            var result = await service.GetPagedAsync(query);

            Assert.AreEqual(10, result.PageSize);
        }

        /// <summary>
        /// UTCID09 - Boundary: Page vượt quá số trang → pageData rỗng, total vẫn đúng
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_PageOutOfRange_ReturnsEmptyPageDataButCorrectTotal()
        {
            var repo = new Mock<IStockTransactionRepository>();
            var inventoryRepo = new Mock<IInventoryRepository>();
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object, MockCurrentUser.AsAdmin());
            var query = new ParamQuery { Page = 99, PageSize = 10 };

            var data = BuildMixedTransactions().AsQueryable();
            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<StockTransaction>(data));

            var result = await service.GetPagedAsync(query);

            Assert.AreEqual(0, result.PageData.Count());
            Assert.IsTrue(result.Total > 0);
        }

        private static List<StockTransaction> BuildMixedTransactions() => new()
        {
            new StockTransaction { StockTransactionId = 1, SparePartId = 10, TransactionType = TransactionType.Import, QuantityChange = 5, UnitPrice = 10000, Inventory = new Inventory { PartName = "Bugi", PartCode = "BG-001" } },
            new StockTransaction { StockTransactionId = 2, SparePartId = 10, TransactionType = TransactionType.Import, QuantityChange = 3, UnitPrice = 11000, Inventory = new Inventory { PartName = "Bugi", PartCode = "BG-001" } },
            new StockTransaction { StockTransactionId = 3, SparePartId = 11, TransactionType = TransactionType.ExportToJobCard, QuantityChange = -2, UnitPrice = 12000, JobCardId = 10, Inventory = new Inventory { PartName = "Loc gio", PartCode = "LG-001" } },
            new StockTransaction { StockTransactionId = 4, SparePartId = 10, TransactionType = TransactionType.ReturnFromJobCard, QuantityChange = 1, UnitPrice = 0, JobCardId = 10, Inventory = new Inventory { PartName = "Bugi", PartCode = "BG-001" } },
            new StockTransaction { StockTransactionId = 5, SparePartId = 12, TransactionType = TransactionType.Adjustment, QuantityChange = -3, UnitPrice = 0, Inventory = new Inventory { PartName = "Nhot Castrol", PartCode = "NHT-001" } }
        };
    }
}
