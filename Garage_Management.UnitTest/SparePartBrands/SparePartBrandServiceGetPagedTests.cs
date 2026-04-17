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
    }
}
