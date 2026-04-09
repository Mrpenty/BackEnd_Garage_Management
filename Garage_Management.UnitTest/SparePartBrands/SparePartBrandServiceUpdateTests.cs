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
    public class SparePartBrandServiceUpdateTests
    {
        [TestMethod]
        public async Task UpdateAsync_NotFound_ReturnsNull()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((SparePartBrand?)null);

            var result = await service.UpdateAsync(99, new SparePartBrandUpdateRequest { BrandName = "X" });

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateAsync_HasChildren_OnlyDescriptionUpdated()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var entity = new SparePartBrand { SparePartBrandId = 1, BrandName = "Honda", Description = "old" };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.UpdateAsync(1, new SparePartBrandUpdateRequest { BrandName = "Yamaha", Description = "new" });

            Assert.IsNotNull(result);
            Assert.AreEqual("Honda", entity.BrandName);
            Assert.AreEqual("new", entity.Description);
        }
    }
}
