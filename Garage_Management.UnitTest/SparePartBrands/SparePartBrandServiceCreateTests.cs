using Garage_Management.Application.DTOs.Inventories.SparePartBrands;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.SparePartBrands
{
    [TestClass]
    public class SparePartBrandServiceCreateTests
    {
        [TestMethod]
        public async Task CreateAsync_Duplicate_Throws()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            repo.Setup(x => x.HasExistAsync("Honda", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new SparePartBrandCreateRequest { BrandName = "Honda" }));
        }

        [TestMethod]
        public async Task CreateAsync_Valid_ReturnsResponse()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            repo.Setup(x => x.HasExistAsync("NGK", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.AddAsync(It.IsAny<SparePartBrand>(), It.IsAny<CancellationToken>()))
                .Callback<SparePartBrand, CancellationToken>((e, _) => e.SparePartBrandId = 12)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.CreateAsync(new SparePartBrandCreateRequest { BrandName = "NGK", Description = "Bugi" });

            Assert.AreEqual(12, result.SparePartBrandId);
        }
    }
}
