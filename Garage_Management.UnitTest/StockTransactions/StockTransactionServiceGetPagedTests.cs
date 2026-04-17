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
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
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
            var service = new StockTransactionService(repo.Object, inventoryRepo.Object);
            var query = new ParamQuery { Page = 1, PageSize = 10, Filter = "import" };

            var data = new List<StockTransaction>
            {
                new StockTransaction
                {
                    StockTransactionId = 1,
                    SparePartId = 10,
                    TransactionType = TransactionType.Import,
                    QuantityChange = 5,
                    UnitPrice = 10000,
                    Inventory = new Inventory { PartName = "Bugi", PartCode = "BG-001" }
                },
                new StockTransaction
                {
                    StockTransactionId = 2,
                    SparePartId = 11,
                    TransactionType = TransactionType.ExportToJobCard,
                    QuantityChange = -2,
                    UnitPrice = 12000,
                    Inventory = new Inventory { PartName = "Loc gio", PartCode = "LG-001" }
                }
            }.AsQueryable();

            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<StockTransaction>(data));

            var result = await service.GetPagedAsync(query);

            Assert.AreEqual(1, result.Total);
            Assert.AreEqual(TransactionType.Import, result.PageData.First().TransactionType);
        }
    }
}
