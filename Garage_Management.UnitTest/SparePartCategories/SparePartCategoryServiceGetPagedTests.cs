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
    }
}
