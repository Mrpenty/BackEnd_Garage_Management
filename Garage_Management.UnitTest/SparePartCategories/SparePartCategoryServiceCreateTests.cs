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
    public class SparePartCategoryServiceCreateTests
    {
        [TestMethod]
        public async Task CreateAsync_EmptyName_Throws()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new SparePartCategoryCreateRequest { CategoryName = " " }));
        }

        [TestMethod]
        public async Task CreateAsync_Duplicate_Throws()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            repo.Setup(x => x.HasExistAsync("Phanh", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new SparePartCategoryCreateRequest { CategoryName = "Phanh" }));
        }

        [TestMethod]
        public async Task CreateAsync_Valid_ReturnsResponse()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            repo.Setup(x => x.HasExistAsync("Phanh", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.AddAsync(It.IsAny<SparePartCategory>(), It.IsAny<CancellationToken>()))
                .Callback<SparePartCategory, CancellationToken>((e, _) => e.CategoryId = 5)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.CreateAsync(new SparePartCategoryCreateRequest
            {
                CategoryName = "Phanh",
                Description = "Phụ tùng phanh",
                IsActive = true
            });

            Assert.AreEqual(5, result.CategoryId);
            Assert.AreEqual("Phanh", result.CategoryName);
            Assert.IsTrue(result.IsActive);
        }

        /// <summary>
        /// UTCID04 - Normal: Tạo category "Lọc" chưa tồn tại → success, CategoryId auto-generated
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Valid_AnotherCategory_ReturnsResponse()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            repo.Setup(x => x.HasExistAsync("Lọc", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.AddAsync(It.IsAny<SparePartCategory>(), It.IsAny<CancellationToken>()))
                .Callback<SparePartCategory, CancellationToken>((e, _) => e.CategoryId = 6)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.CreateAsync(new SparePartCategoryCreateRequest
            {
                CategoryName = "Lọc",
                Description = "Lọc gió",
                IsActive = true
            });

            Assert.AreEqual(6, result.CategoryId);
            Assert.AreEqual("Lọc", result.CategoryName);
            Assert.AreEqual("Lọc gió", result.Description);
            Assert.IsTrue(result.IsActive);
        }

        /// <summary>
        /// UTCID05 - Abnormal: CategoryName = null → throw "Phải nhập tên cho nhóm phụ tùng"
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_NullName_Throws()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.CreateAsync(new SparePartCategoryCreateRequest
                {
                    CategoryName = null!,
                    Description = null,
                    IsActive = false
                }));
            Assert.AreEqual("Phải nhập tên cho nhóm phụ tùng", ex.Message);
        }

        /// <summary>
        /// UTCID06 - Boundary: CategoryName 101 ký tự → throw "Tên nhóm phụ tùng không được vượt quá 100 ký tự"
        /// </summary>
        [TestMethod]
        public async Task UTCID06_CreateAsync_NameTooLong_Throws()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            var longName = new string('A', 101);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.CreateAsync(new SparePartCategoryCreateRequest { CategoryName = longName }));
            Assert.AreEqual("Tên nhóm phụ tùng không được vượt quá 100 ký tự", ex.Message);
        }

        /// <summary>
        /// UTCID07 - Boundary: Description 256 ký tự → throw "Mô tả không được vượt quá 255 ký tự"
        /// </summary>
        [TestMethod]
        public async Task UTCID07_CreateAsync_DescriptionTooLong_Throws()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            var longDesc = new string('B', 256);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.CreateAsync(new SparePartCategoryCreateRequest { CategoryName = "Phanh", Description = longDesc }));
            Assert.AreEqual("Mô tả không được vượt quá 255 ký tự", ex.Message);
        }

        /// <summary>
        /// UTCID08 - Boundary: CategoryName có khoảng trắng đầu/cuối → trim trước khi check duplicate
        /// </summary>
        [TestMethod]
        public async Task UTCID08_CreateAsync_NameWithSpaces_TrimsBeforeDuplicateCheck()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            repo.Setup(x => x.HasExistAsync("Phanh", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.AddAsync(It.IsAny<SparePartCategory>(), It.IsAny<CancellationToken>()))
                .Callback<SparePartCategory, CancellationToken>((e, _) => e.CategoryId = 7)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.CreateAsync(new SparePartCategoryCreateRequest { CategoryName = "  Phanh  " });

            Assert.AreEqual(7, result.CategoryId);
            Assert.AreEqual("Phanh", result.CategoryName);
            repo.Verify(x => x.HasExistAsync("Phanh", null, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// UTCID09 - Abnormal: User không có quyền (không phải Admin/Supervisor) → throw UnauthorizedAccessException
        /// </summary>
        [TestMethod]
        public async Task UTCID09_CreateAsync_UnauthorizedRole_Throws()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var currentUser = new Mock<ICurrentUserService>();
            currentUser.Setup(x => x.IsAdmin()).Returns(false);
            currentUser.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            currentUser.Setup(x => x.GetCurrentUserId()).Returns(7);
            var service = new SparePartCategoryService(repo.Object, currentUser.Object);

            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                service.CreateAsync(new SparePartCategoryCreateRequest { CategoryName = "Phanh" }));
            Assert.AreEqual("Chỉ Supervisor được tạo nhóm phụ tùng", ex.Message);
        }

        /// <summary>
        /// UTCID10 - Normal: Supervisor tạo → success, CreatedBy = userId
        /// </summary>
        [TestMethod]
        public async Task UTCID10_CreateAsync_SupervisorRole_SetsCreatedBy()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var currentUser = new Mock<ICurrentUserService>();
            currentUser.Setup(x => x.IsAdmin()).Returns(false);
            currentUser.Setup(x => x.IsInRole("Supervisor")).Returns(true);
            currentUser.Setup(x => x.GetCurrentUserId()).Returns(42);
            var service = new SparePartCategoryService(repo.Object, currentUser.Object);

            SparePartCategory? captured = null;
            repo.Setup(x => x.HasExistAsync("Phanh", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.AddAsync(It.IsAny<SparePartCategory>(), It.IsAny<CancellationToken>()))
                .Callback<SparePartCategory, CancellationToken>((e, _) => { e.CategoryId = 8; captured = e; })
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.CreateAsync(new SparePartCategoryCreateRequest { CategoryName = "Phanh" });

            Assert.AreEqual(8, result.CategoryId);
            Assert.IsNotNull(captured);
            Assert.AreEqual(42, captured!.CreatedBy);
        }
    }
}
