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
    public class SparePartBrandServiceCreateTests
    {
        /// <summary>
        /// UTCID01 - Abnormal: BrandName đã tồn tại
        /// </summary>
        [TestMethod]
        public async Task UTCID01_CreateAsync_Duplicate_Throws()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            repo.Setup(x => x.HasExistAsync("Honda", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.CreateAsync(new SparePartBrandCreateRequest { BrandName = "Honda" }));
            Assert.AreEqual("Hãng phụ tùng đã tồn tại", ex.Message);
        }

        /// <summary>
        /// UTCID02 - Normal: Tạo brand hợp lệ
        /// </summary>
        [TestMethod]
        public async Task UTCID02_CreateAsync_Valid_ReturnsResponse()
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
            Assert.AreEqual("NGK", result.BrandName);
            Assert.AreEqual("Bugi", result.Description);
        }

        /// <summary>
        /// UTCID03 - Abnormal: BrandName = null → throw
        /// </summary>
        [TestMethod]
        public async Task UTCID03_CreateAsync_NullBrandName_Throws()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.CreateAsync(new SparePartBrandCreateRequest { BrandName = null! }));
            Assert.AreEqual("Tên hãng phụ tùng không được để trống", ex.Message);
        }

        /// <summary>
        /// UTCID04 - Abnormal: BrandName rỗng → throw
        /// </summary>
        [TestMethod]
        public async Task UTCID04_CreateAsync_EmptyBrandName_Throws()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.CreateAsync(new SparePartBrandCreateRequest { BrandName = "" }));
            Assert.AreEqual("Tên hãng phụ tùng không được để trống", ex.Message);
        }

        /// <summary>
        /// UTCID05 - Abnormal: BrandName toàn whitespace → throw
        /// </summary>
        [TestMethod]
        public async Task UTCID05_CreateAsync_WhitespaceBrandName_Throws()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.CreateAsync(new SparePartBrandCreateRequest { BrandName = "   " }));
            Assert.AreEqual("Tên hãng phụ tùng không được để trống", ex.Message);
        }

        /// <summary>
        /// UTCID06 - Boundary: BrandName có khoảng trắng đầu/cuối → trim, check duplicate đúng tên đã trim
        /// </summary>
        [TestMethod]
        public async Task UTCID06_CreateAsync_BrandNameWithSpaces_TrimsBeforeDuplicateCheck()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            repo.Setup(x => x.HasExistAsync("Honda", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.AddAsync(It.IsAny<SparePartBrand>(), It.IsAny<CancellationToken>()))
                .Callback<SparePartBrand, CancellationToken>((e, _) => e.SparePartBrandId = 20)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.CreateAsync(new SparePartBrandCreateRequest { BrandName = "  Honda  " });

            Assert.AreEqual(20, result.SparePartBrandId);
            Assert.AreEqual("Honda", result.BrandName);
            repo.Verify(x => x.HasExistAsync("Honda", null, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// UTCID07 - Boundary: BrandName length=101 (vượt 100) → throw
        /// </summary>
        [TestMethod]
        public async Task UTCID07_CreateAsync_BrandNameTooLong_Throws()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var longName = new string('A', 101);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.CreateAsync(new SparePartBrandCreateRequest { BrandName = longName }));
            Assert.AreEqual("Tên hãng phụ tùng không được vượt quá 100 ký tự", ex.Message);
        }

        /// <summary>
        /// UTCID08 - Boundary: Description length=501 (vượt 500) → throw
        /// </summary>
        [TestMethod]
        public async Task UTCID08_CreateAsync_DescriptionTooLong_Throws()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var service = new SparePartBrandService(repo.Object);
            var longDesc = new string('B', 501);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.CreateAsync(new SparePartBrandCreateRequest { BrandName = "Honda", Description = longDesc }));
            Assert.AreEqual("Mô tả không được vượt quá 500 ký tự", ex.Message);
        }

        /// <summary>
        /// UTCID09 - Abnormal: User không có quyền (Mechanic) → throw UnauthorizedAccessException
        /// </summary>
        [TestMethod]
        public async Task UTCID09_CreateAsync_UnauthorizedRole_Throws()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var currentUser = new Mock<ICurrentUserService>();
            currentUser.Setup(x => x.IsAdmin()).Returns(false);
            currentUser.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            currentUser.Setup(x => x.GetCurrentUserId()).Returns(7);
            var service = new SparePartBrandService(repo.Object, currentUser.Object);

            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                service.CreateAsync(new SparePartBrandCreateRequest { BrandName = "Bosch" }));
            Assert.AreEqual("Chỉ Supervisor được tạo hãng phụ tùng", ex.Message);
        }

        /// <summary>
        /// UTCID10 - Normal: Supervisor tạo brand → success, CreatedBy = userId
        /// </summary>
        [TestMethod]
        public async Task UTCID10_CreateAsync_SupervisorRole_SetsCreatedBy()
        {
            var repo = new Mock<ISparePartBrandRepository>();
            var currentUser = new Mock<ICurrentUserService>();
            currentUser.Setup(x => x.IsAdmin()).Returns(false);
            currentUser.Setup(x => x.IsInRole("Supervisor")).Returns(true);
            currentUser.Setup(x => x.GetCurrentUserId()).Returns(42);
            var service = new SparePartBrandService(repo.Object, currentUser.Object);

            SparePartBrand? captured = null;
            repo.Setup(x => x.HasExistAsync("Bosch", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.AddAsync(It.IsAny<SparePartBrand>(), It.IsAny<CancellationToken>()))
                .Callback<SparePartBrand, CancellationToken>((e, _) => { e.SparePartBrandId = 30; captured = e; })
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.CreateAsync(new SparePartBrandCreateRequest { BrandName = "Bosch" });

            Assert.AreEqual(30, result.SparePartBrandId);
            Assert.IsNotNull(captured);
            Assert.AreEqual(42, captured!.CreatedBy);
        }
    }
}
