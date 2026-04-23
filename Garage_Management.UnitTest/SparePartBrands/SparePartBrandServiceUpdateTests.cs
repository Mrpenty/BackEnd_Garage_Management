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

        /// <summary>
        /// UTCID03 - Normal: Brand không có spareparts con, đổi name thành tên chưa trùng → update cả BrandName + Description
        /// </summary>
        [TestMethod]
        public async Task UpdateAsync_NoChildren_NewUniqueName_UpdatesBothNameAndDescription()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var entity = new SparePartBrand { SparePartBrandId = 1, BrandName = "Honda", Description = "old" };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.HasExistAsync("Yamaha", 1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.UpdateAsync(1, new SparePartBrandUpdateRequest { BrandName = "Yamaha", Description = "new" });

            Assert.IsNotNull(result);
            Assert.AreEqual("Yamaha", entity.BrandName);
            Assert.AreEqual("new", entity.Description);
        }

        /// <summary>
        /// UTCID04 - Abnormal: Brand không có spareparts, đổi name thành tên đã tồn tại ở brand khác → throw
        /// </summary>
        [TestMethod]
        public async Task UpdateAsync_NoChildren_DuplicateName_Throws()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var entity = new SparePartBrand { SparePartBrandId = 1, BrandName = "Honda", Description = "old" };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.HasExistAsync("Yamaha", 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.UpdateAsync(1, new SparePartBrandUpdateRequest { BrandName = "Yamaha", Description = "new" }));
            Assert.AreEqual("Hãng phụ tùng đã tồn tại, không thể đổi tên", ex.Message);
        }

        /// <summary>
        /// UTCID05 - Normal: Brand không có spareparts, giữ nguyên BrandName (= current), đổi Description → update Description
        /// </summary>
        [TestMethod]
        public async Task UpdateAsync_NoChildren_SameName_UpdatesDescriptionOnly()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var entity = new SparePartBrand { SparePartBrandId = 1, BrandName = "Honda", Description = "old" };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.UpdateAsync(1, new SparePartBrandUpdateRequest { BrandName = "Honda", Description = "new" });

            Assert.IsNotNull(result);
            Assert.AreEqual("Honda", entity.BrandName);
            Assert.AreEqual("new", entity.Description);
            // Không cần check duplicate khi name không đổi
            repo.Verify(x => x.HasExistAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// UTCID06 - Normal: Brand có spareparts con, chỉ đổi Description (không đổi BrandName) → update Description
        /// </summary>
        [TestMethod]
        public async Task UpdateAsync_HasChildren_SameName_UpdatesDescriptionOnly()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var entity = new SparePartBrand { SparePartBrandId = 1, BrandName = "Honda", Description = "old" };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.UpdateAsync(1, new SparePartBrandUpdateRequest { BrandName = "Honda", Description = "new" });

            Assert.IsNotNull(result);
            Assert.AreEqual("Honda", entity.BrandName);
            Assert.AreEqual("new", entity.Description);
        }
    }
}
