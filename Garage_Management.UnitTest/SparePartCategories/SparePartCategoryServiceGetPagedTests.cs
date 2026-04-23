using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.SparePartCategories
{
    [TestClass]
    public class SparePartCategoryServiceGetPagedTests
    {
        [TestMethod]
        public async Task GetPagedAsync_ReturnsMappedPagedResult()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            var query = new ParamQuery();
            repo.Setup(x => x.GetPagedAsync(query, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<SparePartCategory>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 2,
                    PageData = new List<SparePartCategory>
                    {
                        new SparePartCategory { CategoryId = 1, CategoryName = "Phanh", IsActive = true },
                        new SparePartCategory { CategoryId = 2, CategoryName = "Lọc gió", IsActive = true }
                    }
                });

            var result = await service.GetPagedAsync(query);

            Assert.AreEqual(2, result.Total);
            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(10, result.PageSize);
        }

        [TestMethod]
        public async Task GetPagedAsync_OnlyActive_PassesParameterToRepo()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            var query = new ParamQuery();
            repo.Setup(x => x.GetPagedAsync(query, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<SparePartCategory>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 1,
                    PageData = new List<SparePartCategory>
                    {
                        new SparePartCategory { CategoryId = 1, CategoryName = "Phanh", IsActive = true }
                    }
                });

            var result = await service.GetPagedAsync(query, onlyActive: true);

            Assert.AreEqual(1, result.Total);
            repo.Verify(x => x.GetPagedAsync(query, true, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// UTCID03 - Normal: DB rỗng → total=0, pageData rỗng
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_Empty_ReturnsEmptyPagedResult()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            var query = new ParamQuery();

            repo.Setup(x => x.GetPagedAsync(query, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<SparePartCategory>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 0,
                    PageData = new List<SparePartCategory>()
                });

            var result = await service.GetPagedAsync(query);

            Assert.AreEqual(0, result.Total);
            Assert.AreEqual(0, result.PageData.Count());
        }

        /// <summary>
        /// UTCID04 - Boundary: Page=2 với 15 records tổng, pageSize=10 → trả 5 items còn lại
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_LastPageWithPartialData_ReturnsRemainingItems()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            var query = new ParamQuery { Page = 2, PageSize = 10 };

            var last5 = new List<SparePartCategory>();
            for (int i = 11; i <= 15; i++)
            {
                last5.Add(new SparePartCategory { CategoryId = i, CategoryName = $"Category{i}", IsActive = true });
            }

            repo.Setup(x => x.GetPagedAsync(query, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<SparePartCategory>
                {
                    Page = 2,
                    PageSize = 10,
                    Total = 15,
                    PageData = last5
                });

            var result = await service.GetPagedAsync(query);

            Assert.AreEqual(15, result.Total);
            Assert.AreEqual(2, result.Page);
            Assert.AreEqual(5, result.PageData.Count());
        }
    }
}
