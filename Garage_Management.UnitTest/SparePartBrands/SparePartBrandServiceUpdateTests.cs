using Garage_Management.Application.DTOs.Inventories.SparePartBrands;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services.Auth;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
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

        /// <summary>
        /// UTCID07 - Abnormal: Brand đã soft-deleted (DeletedAt != null) → return null
        /// </summary>
        [TestMethod]
        public async Task UpdateAsync_SoftDeleted_ReturnsNull()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var entity = new SparePartBrand
            {
                SparePartBrandId = 1,
                BrandName = "Honda",
                Description = "old",
                DeletedAt = DateTime.UtcNow
            };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            var result = await service.UpdateAsync(1, new SparePartBrandUpdateRequest { BrandName = "Yamaha", Description = "new" });

            Assert.IsNull(result);
            // Không gọi Update / SaveAsync vì entity đã bị xóa mềm
            repo.Verify(x => x.Update(It.IsAny<SparePartBrand>()), Times.Never);
            repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// UTCID08 - Abnormal: NoChildren + BrandName=null → throw "Tên hãng phụ tùng không được để trống"
        /// </summary>
        [TestMethod]
        public async Task UTCID08_UpdateAsync_NoChildren_NullBrandName_Throws()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var entity = new SparePartBrand { SparePartBrandId = 1, BrandName = "Honda", Description = "old" };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.UpdateAsync(1, new SparePartBrandUpdateRequest { BrandName = null!, Description = "new" }));
            Assert.AreEqual("Tên hãng phụ tùng không được để trống", ex.Message);
        }

        /// <summary>
        /// UTCID09 - Abnormal: NoChildren + BrandName toàn whitespace → throw same
        /// </summary>
        [TestMethod]
        public async Task UTCID09_UpdateAsync_NoChildren_WhitespaceBrandName_Throws()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var entity = new SparePartBrand { SparePartBrandId = 1, BrandName = "Honda", Description = "old" };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.UpdateAsync(1, new SparePartBrandUpdateRequest { BrandName = "   ", Description = "new" }));
            Assert.AreEqual("Tên hãng phụ tùng không được để trống", ex.Message);
        }

        /// <summary>
        /// UTCID10 - Boundary: NoChildren + BrandName 101 ký tự → throw "Tên hãng phụ tùng không được vượt quá 100 ký tự"
        /// </summary>
        [TestMethod]
        public async Task UTCID10_UpdateAsync_NoChildren_BrandNameTooLong_Throws()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var entity = new SparePartBrand { SparePartBrandId = 1, BrandName = "Honda", Description = "old" };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            var longName = new string('A', 101);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.UpdateAsync(1, new SparePartBrandUpdateRequest { BrandName = longName, Description = "new" }));
            Assert.AreEqual("Tên hãng phụ tùng không được vượt quá 100 ký tự", ex.Message);
        }

        /// <summary>
        /// UTCID11 - Boundary: Description 501 ký tự → throw "Mô tả không được vượt quá 500 ký tự"
        /// </summary>
        [TestMethod]
        public async Task UTCID11_UpdateAsync_DescriptionTooLong_Throws()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var entity = new SparePartBrand { SparePartBrandId = 1, BrandName = "Honda", Description = "old" };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            var longDesc = new string('B', 501);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.UpdateAsync(1, new SparePartBrandUpdateRequest { BrandName = "Honda", Description = longDesc }));
            Assert.AreEqual("Mô tả không được vượt quá 500 ký tự", ex.Message);
        }

        /// <summary>
        /// UTCID12 - Abnormal: User không phải Admin/Supervisor → throw UnauthorizedAccessException
        /// </summary>
        [TestMethod]
        public async Task UTCID12_UpdateAsync_UnauthorizedRole_Throws()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var currentUser = new Mock<ICurrentUserService>();
            currentUser.Setup(x => x.IsAdmin()).Returns(false);
            currentUser.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            currentUser.Setup(x => x.GetCurrentUserId()).Returns(7);
            var service = new SparePartBrandService(repo.Object, currentUser.Object);

            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                service.UpdateAsync(1, new SparePartBrandUpdateRequest { BrandName = "Yamaha", Description = "new" }));
            Assert.AreEqual("Chỉ Supervisor được cập nhật hãng phụ tùng", ex.Message);
        }

        /// <summary>
        /// UTCID13 - Normal: Update thành công bởi Supervisor → entity.UpdatedAt set + UpdatedBy = userId
        /// </summary>
        [TestMethod]
        public async Task UTCID13_UpdateAsync_SupervisorUpdate_SetsAuditFields()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var currentUser = new Mock<ICurrentUserService>();
            currentUser.Setup(x => x.IsAdmin()).Returns(false);
            currentUser.Setup(x => x.IsInRole("Supervisor")).Returns(true);
            currentUser.Setup(x => x.GetCurrentUserId()).Returns(42);
            var service = new SparePartBrandService(repo.Object, currentUser.Object);

            var entity = new SparePartBrand { SparePartBrandId = 1, BrandName = "Honda", Description = "old" };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            var beforeUpdate = DateTime.UtcNow;

            var result = await service.UpdateAsync(1, new SparePartBrandUpdateRequest { BrandName = "Honda", Description = "new" });

            Assert.IsNotNull(result);
            Assert.IsNotNull(entity.UpdatedAt);
            Assert.IsTrue(entity.UpdatedAt >= beforeUpdate);
            Assert.AreEqual(42, entity.UpdatedBy);
        }

        /// <summary>
        /// UTCID14 - Boundary: NoChildren + BrandName="  Yamaha  " (có space) → trim, check duplicate đúng tên đã trim
        /// </summary>
        [TestMethod]
        public async Task UTCID14_UpdateAsync_NoChildren_BrandNameWithSpaces_TrimsBeforeDuplicateCheck()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var entity = new SparePartBrand { SparePartBrandId = 1, BrandName = "Honda", Description = "old" };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasSparePartsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.HasExistAsync("Yamaha", 1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.UpdateAsync(1, new SparePartBrandUpdateRequest { BrandName = "  Yamaha  ", Description = "new" });

            Assert.IsNotNull(result);
            Assert.AreEqual("Yamaha", entity.BrandName);
            repo.Verify(x => x.HasExistAsync("Yamaha", 1, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
