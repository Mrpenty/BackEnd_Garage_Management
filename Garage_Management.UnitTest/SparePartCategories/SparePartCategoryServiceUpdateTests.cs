using Garage_Management.Application.DTOs.Inventories.SparePartCategories;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services.Auth;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
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

        /// <summary>
        /// UTCID07 - Abnormal: NoChildren + CategoryName=null → throw "Phải nhập tên cho nhóm phụ tùng"
        /// </summary>
        [TestMethod]
        public async Task UTCID07_UpdateAsync_NoChildren_NullName_Throws()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            var entity = new SparePartCategory { CategoryId = 1, CategoryName = "Phanh" };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.UpdateAsync(1, new SparePartCategoryUpdateRequest { CategoryName = null! }));
            Assert.AreEqual("Phải nhập tên cho nhóm phụ tùng", ex.Message);
        }

        /// <summary>
        /// UTCID08 - Abnormal: NoChildren + CategoryName toàn whitespace → throw same
        /// </summary>
        [TestMethod]
        public async Task UTCID08_UpdateAsync_NoChildren_WhitespaceName_Throws()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            var entity = new SparePartCategory { CategoryId = 1, CategoryName = "Phanh" };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.UpdateAsync(1, new SparePartCategoryUpdateRequest { CategoryName = "   " }));
            Assert.AreEqual("Phải nhập tên cho nhóm phụ tùng", ex.Message);
        }

        /// <summary>
        /// UTCID09 - Boundary: NoChildren + CategoryName 101 ký tự → throw MaxLength
        /// </summary>
        [TestMethod]
        public async Task UTCID09_UpdateAsync_NoChildren_NameTooLong_Throws()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            var entity = new SparePartCategory { CategoryId = 1, CategoryName = "Phanh" };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            var longName = new string('A', 101);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.UpdateAsync(1, new SparePartCategoryUpdateRequest { CategoryName = longName }));
            Assert.AreEqual("Tên nhóm phụ tùng không được vượt quá 100 ký tự", ex.Message);
        }

        /// <summary>
        /// UTCID10 - Boundary: Description 256 ký tự → throw MaxLength
        /// </summary>
        [TestMethod]
        public async Task UTCID10_UpdateAsync_DescriptionTooLong_Throws()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            var entity = new SparePartCategory { CategoryId = 1, CategoryName = "Phanh" };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            var longDesc = new string('B', 256);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.UpdateAsync(1, new SparePartCategoryUpdateRequest { CategoryName = "Phanh", Description = longDesc }));
            Assert.AreEqual("Mô tả không được vượt quá 255 ký tự", ex.Message);
        }

        /// <summary>
        /// UTCID11 - Abnormal: User không có quyền (không phải Admin/Supervisor) → throw UnauthorizedAccessException
        /// </summary>
        [TestMethod]
        public async Task UTCID11_UpdateAsync_UnauthorizedRole_Throws()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var currentUser = new Mock<ICurrentUserService>();
            currentUser.Setup(x => x.IsAdmin()).Returns(false);
            currentUser.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            currentUser.Setup(x => x.GetCurrentUserId()).Returns(7);
            var service = new SparePartCategoryService(repo.Object, currentUser.Object);

            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                service.UpdateAsync(1, new SparePartCategoryUpdateRequest { CategoryName = "Lọc" }));
            Assert.AreEqual("Chỉ Supervisor được cập nhật nhóm phụ tùng", ex.Message);
        }

        /// <summary>
        /// UTCID12 - Normal: Update thành công bởi Supervisor → entity.UpdatedAt set + UpdatedBy = userId
        /// </summary>
        [TestMethod]
        public async Task UTCID12_UpdateAsync_SupervisorUpdate_SetsAuditFields()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var currentUser = new Mock<ICurrentUserService>();
            currentUser.Setup(x => x.IsAdmin()).Returns(false);
            currentUser.Setup(x => x.IsInRole("Supervisor")).Returns(true);
            currentUser.Setup(x => x.GetCurrentUserId()).Returns(42);
            var service = new SparePartCategoryService(repo.Object, currentUser.Object);

            var entity = new SparePartCategory { CategoryId = 1, CategoryName = "Phanh", Description = "Cũ" };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            var beforeUpdate = DateTime.UtcNow;

            var result = await service.UpdateAsync(1, new SparePartCategoryUpdateRequest { CategoryName = "Phanh", Description = "Mới" });

            Assert.IsNotNull(result);
            Assert.IsNotNull(entity.UpdatedAt);
            Assert.IsTrue(entity.UpdatedAt >= beforeUpdate);
            Assert.AreEqual(42, entity.UpdatedBy);
        }
    }
}
