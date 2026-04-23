using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.SparePartBrands
{
    [TestClass]
    public class SparePartBrandServiceGetPagedTests
    {
        [TestMethod]
        public async Task GetPagedAsync_ReturnsMappedPagedResult()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var query = new ParamQuery();

            repo.Setup(x => x.GetPagedAsync(query, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<SparePartBrand>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 2,
                    PageData = new List<SparePartBrand>
                    {
                        new SparePartBrand { SparePartBrandId = 1, BrandName = "NGK", IsActive = true },
                        new SparePartBrand { SparePartBrandId = 2, BrandName = "Bosch", IsActive = false }
                    }
                });

            var result = await service.GetPagedAsync(query);

            Assert.AreEqual(2, result.Total);
            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(10, result.PageSize);
        }

        /// <summary>
        /// UTCID02 - Normal: onlyActive=true → chỉ trả brand active
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_OnlyActive_ReturnsActiveBrandsOnly()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var query = new ParamQuery();

            repo.Setup(x => x.GetPagedAsync(query, true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<SparePartBrand>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 1,
                    PageData = new List<SparePartBrand>
                    {
                        new SparePartBrand { SparePartBrandId = 1, BrandName = "NGK", IsActive = true }
                    }
                });

            var result = await service.GetPagedAsync(query, onlyActive: true);

            Assert.AreEqual(1, result.Total);
            Assert.AreEqual(1, result.PageData.Count());
            Assert.IsTrue(result.PageData.All(x => x.IsActive));
        }

        /// <summary>
        /// UTCID03 - Normal: DB rỗng → total=0, pageData rỗng
        /// </summary>
        [TestMethod]
        public async Task GetPagedAsync_Empty_ReturnsEmptyPagedResult()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var query = new ParamQuery();

            repo.Setup(x => x.GetPagedAsync(query, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<SparePartBrand>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 0,
                    PageData = new List<SparePartBrand>()
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
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var query = new ParamQuery { Page = 2, PageSize = 10 };

            var last5 = new List<SparePartBrand>();
            for (int i = 11; i <= 15; i++)
            {
                last5.Add(new SparePartBrand { SparePartBrandId = i, BrandName = $"Brand{i}", IsActive = true });
            }

            repo.Setup(x => x.GetPagedAsync(query, false, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<SparePartBrand>
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
