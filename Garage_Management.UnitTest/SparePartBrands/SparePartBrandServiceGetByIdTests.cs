using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.SparePartBrands
{
    [TestClass]
    public class SparePartBrandServiceGetByIdTests
    {
        [TestMethod]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((SparePartBrand?)null);

            var result = await service.GetByIdAsync(99);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_Found_ReturnsMappedResponse()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new SparePartBrand
            {
                SparePartBrandId = 1,
                BrandName = "NGK",
                Description = "Bugi",
                IsActive = true
            });

            var result = await service.GetByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.SparePartBrandId);
            Assert.AreEqual("NGK", result.BrandName);
            Assert.IsTrue(result.IsActive);
        }

        /// <summary>
        /// UTCID03 - Normal: Brand inactive → trả response với IsActive=false
        /// </summary>
        [TestMethod]
        public async Task GetByIdAsync_FoundInactive_ReturnsResponseWithIsActiveFalse()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new SparePartBrand
            {
                SparePartBrandId = 2,
                BrandName = "Yamaha",
                Description = "Đã ngưng kinh doanh",
                IsActive = false
            });

            var result = await service.GetByIdAsync(2);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.SparePartBrandId);
            Assert.AreEqual("Yamaha", result.BrandName);
            Assert.IsFalse(result.IsActive);
        }

        /// <summary>
        /// UTCID04 - Boundary: Id = 0 → repo trả null
        /// </summary>
        [TestMethod]
        public async Task GetByIdAsync_ZeroId_ReturnsNull()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(0)).ReturnsAsync((SparePartBrand?)null);

            var result = await service.GetByIdAsync(0);

            Assert.IsNull(result);
        }

        /// <summary>
        /// UTCID05 - Abnormal: Id âm → repo trả null
        /// </summary>
        [TestMethod]
        public async Task GetByIdAsync_NegativeId_ReturnsNull()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(-1)).ReturnsAsync((SparePartBrand?)null);

            var result = await service.GetByIdAsync(-1);

            Assert.IsNull(result);
        }
    }
}
