using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.UnitTest.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Inventories
{
    [TestClass]
    public class InventoryServiceGetPagedTests
    {
        [TestMethod]
        public async Task GetPagedAsync_WithData_ReturnsSuccessPagedResult()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object, new Mock<ISparePartCategoryRepository>().Object, new Mock<ISparePartBrandRepository>().Object);
            var query = new ParamQuery { Page = 1, PageSize = 10 };

            var data = new List<Inventory>
            {
                new Inventory
                {
                    SparePartId = 1,
                    BranchId = 1,
                    PartCode = "BG-001",
                    PartName = "Bugi NGK",
                    Quantity = 20,
                    IsActive = true,
                    SparePartBrand = new SparePartBrand { BrandName = "NGK" },
                    SparePartCategory = new SparePartCategory { CategoryName = "Đánh lửa" }
                }
            }.AsQueryable();

            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<Inventory>(data));

            var result = await service.GetPagedAsync(query);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.Data.Total);
            Assert.AreEqual(1, result.Data.PageData.Count());
            Assert.AreEqual("Bugi NGK", result.Data.PageData.First().PartName);
        }

        [TestMethod]
        public async Task GetPagedAsync_Empty_ReturnsEmptyPagedResult()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object, new Mock<ISparePartCategoryRepository>().Object, new Mock<ISparePartBrandRepository>().Object);
            var query = new ParamQuery { Page = 1, PageSize = 10 };

            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<Inventory>(new List<Inventory>().AsQueryable()));

            var result = await service.GetPagedAsync(query);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.Data.Total);
            Assert.AreEqual(0, result.Data.PageData.Count());
        }

        /// <summary>
        /// UTCID03 - Normal: Search "bugi" → chỉ trả items có PartName match
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_SearchByKeyword_ReturnsMatches()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object, new Mock<ISparePartCategoryRepository>().Object, new Mock<ISparePartBrandRepository>().Object);
            var query = new ParamQuery { Page = 1, PageSize = 10, Search = "bugi" };

            var data = BuildMixedInventories().AsQueryable();
            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<Inventory>(data));

            var result = await service.GetPagedAsync(query);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.PageData.All(x => (x.PartName ?? string.Empty).ToLower().Contains("bugi")));
        }

        /// <summary>
        /// UTCID04 - Normal: Filter "active" → chỉ trả items IsActive=true
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_FilterActive_ReturnsOnlyActive()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object, new Mock<ISparePartCategoryRepository>().Object, new Mock<ISparePartBrandRepository>().Object);
            var query = new ParamQuery { Page = 1, PageSize = 10, Filter = "active" };

            var data = BuildMixedInventories().AsQueryable();
            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<Inventory>(data));

            var result = await service.GetPagedAsync(query);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.PageData.All(x => x.IsActive));
        }

        /// <summary>
        /// UTCID05 - Normal: Filter "inactive" → chỉ trả items IsActive=false
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_FilterInactive_ReturnsOnlyInactive()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object, new Mock<ISparePartCategoryRepository>().Object, new Mock<ISparePartBrandRepository>().Object);
            var query = new ParamQuery { Page = 1, PageSize = 10, Filter = "inactive" };

            var data = BuildMixedInventories().AsQueryable();
            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<Inventory>(data));

            var result = await service.GetPagedAsync(query);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.PageData.All(x => !x.IsActive));
        }

        /// <summary>
        /// UTCID06 - Normal: Admin bypass filter branch → thấy cross-branch
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_AdminSeesAllBranches()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object, new Mock<ISparePartCategoryRepository>().Object, new Mock<ISparePartBrandRepository>().Object);
            var query = new ParamQuery { Page = 1, PageSize = 10 };

            var data = new List<Inventory>
            {
                new Inventory { SparePartId = 1, BranchId = 1, PartName = "Bugi HN", IsActive = true },
                new Inventory { SparePartId = 2, BranchId = 2, PartName = "Bugi SG", IsActive = true }
            }.AsQueryable();
            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<Inventory>(data));

            // branchId = null → không filter theo branch, thấy tất cả các chi nhánh
            var result = await service.GetPagedAsync(query, null);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(2, result.Data.Total);
        }

        /// <summary>
        /// UTCID07 - Boundary: Page vượt số trang → pageData rỗng, total vẫn đúng
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_PageOutOfRange_ReturnsEmptyButCorrectTotal()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object, new Mock<ISparePartCategoryRepository>().Object, new Mock<ISparePartBrandRepository>().Object);
            var query = new ParamQuery { Page = 99, PageSize = 10 };

            var data = BuildMixedInventories().AsQueryable();
            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<Inventory>(data));

            var result = await service.GetPagedAsync(query);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.Data.PageData.Count());
            Assert.IsTrue(result.Data.Total > 0);
        }

        /// <summary>
        /// UTCID08 - Abnormal: PageSize âm → service chỉnh về default 10
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_NegativePageSize_UsesDefault10()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object, new Mock<ISparePartCategoryRepository>().Object, new Mock<ISparePartBrandRepository>().Object);
            var query = new ParamQuery { Page = 1, PageSize = -5 };

            var data = BuildMixedInventories().AsQueryable();
            repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<Inventory>(data));

            var result = await service.GetPagedAsync(query);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(10, result.Data.PageSize);
        }

        private static List<Inventory> BuildMixedInventories() => new()
        {
            new Inventory { SparePartId = 1, BranchId = 1, PartCode = "BG-001", PartName = "Bugi NGK CR7HSA", Quantity = 20, IsActive = true },
            new Inventory { SparePartId = 2, BranchId = 1, PartCode = "BG-002", PartName = "Bugi Denso Iridium", Quantity = 15, IsActive = true },
            new Inventory { SparePartId = 3, BranchId = 1, PartCode = "LOC-001", PartName = "Lọc gió Honda Wave", Quantity = 10, IsActive = true },
            new Inventory { SparePartId = 4, BranchId = 1, PartCode = "NHT-001", PartName = "Nhớt Motul", Quantity = 5, IsActive = false },
            new Inventory { SparePartId = 5, BranchId = 1, PartCode = "PHANH-001", PartName = "Phanh đĩa Sirius", Quantity = 8, IsActive = false }
        };
    }
}
