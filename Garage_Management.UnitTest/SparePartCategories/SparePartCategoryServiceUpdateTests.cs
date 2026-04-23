using Garage_Management.Application.DTOs.Inventories.SparePartCategories;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.SparePartCategories
{
    [TestClass]
    public class SparePartCategoryServiceUpdateTests
    {
        [TestMethod]
        public async Task UpdateAsync_NotFound_ReturnsNull()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((SparePartCategory?)null);

            var result = await service.UpdateAsync(99, new SparePartCategoryUpdateRequest { CategoryName = "Test" });

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateAsync_HasChildren_NameChanged_Throws()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new SparePartCategory
            {
                CategoryId = 1,
                CategoryName = "Phanh"
            });
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.UpdateAsync(1, new SparePartCategoryUpdateRequest { CategoryName = "Lọc gió" }));
        }

        [TestMethod]
        public async Task UpdateAsync_HasChildren_OnlyDescriptionUpdated()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            var entity = new SparePartCategory
            {
                CategoryId = 1,
                CategoryName = "Phanh",
                Description = "Cũ"
            };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.UpdateAsync(1, new SparePartCategoryUpdateRequest
            {
                CategoryName = "Phanh",
                Description = "Mới"
            });

            Assert.IsNotNull(result);
            Assert.AreEqual("Phanh", result.CategoryName);
            Assert.AreEqual("Mới", result.Description);
        }

        [TestMethod]
        public async Task UpdateAsync_NoChildren_NameChanged_DuplicateName_Throws()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new SparePartCategory
            {
                CategoryId = 1,
                CategoryName = "Phanh"
            });
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.HasExistAsync("Lọc gió", 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.UpdateAsync(1, new SparePartCategoryUpdateRequest { CategoryName = "Lọc gió" }));
        }

        [TestMethod]
        public async Task UpdateAsync_NoChildren_Valid_UpdatesNameAndDescription()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            var entity = new SparePartCategory
            {
                CategoryId = 1,
                CategoryName = "Phanh",
                Description = "Cũ"
            };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.HasExistAsync("Lọc gió", 1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.UpdateAsync(1, new SparePartCategoryUpdateRequest
            {
                CategoryName = "Lọc gió",
                Description = "Mới"
            });

            Assert.IsNotNull(result);
            Assert.AreEqual("Lọc gió", result.CategoryName);
            Assert.AreEqual("Mới", result.Description);
        }

        /// <summary>
        /// UTCID06 - Normal: Không có spareparts + giữ nguyên CategoryName + đổi Description
        /// → service skip duplicate check, chỉ update Description
        /// </summary>
        [TestMethod]
        public async Task UpdateAsync_NoChildren_SameName_UpdatesDescriptionOnly()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            var entity = new SparePartCategory
            {
                CategoryId = 1,
                CategoryName = "Phanh",
                Description = "Cũ"
            };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.UpdateAsync(1, new SparePartCategoryUpdateRequest
            {
                CategoryName = "Phanh",
                Description = "Mới"
            });

            Assert.IsNotNull(result);
            Assert.AreEqual("Phanh", result.CategoryName);
            Assert.AreEqual("Mới", result.Description);
            // Không gọi duplicate check khi name không đổi
            repo.Verify(x => x.HasExistAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
