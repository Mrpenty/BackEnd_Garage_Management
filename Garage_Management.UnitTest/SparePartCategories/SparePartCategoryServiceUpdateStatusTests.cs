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
    public class SparePartCategoryServiceUpdateStatusTests
    {
        /// <summary>
        /// UTCID01 - Normal: Activate category đang inactive (false → true)
        /// </summary>
        [TestMethod]
        public async Task UpdateStatusAsync_ActivateFromInactive_UpdatesAndSaves()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            var entity = new SparePartCategory
            {
                CategoryId = 1,
                CategoryName = "Phanh",
                Description = "Phụ tùng phanh",
                IsActive = false
            };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.UpdateStatusAsync(1, true);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsActive);
            Assert.IsTrue(entity.IsActive);
            repo.Verify(x => x.Update(It.IsAny<SparePartCategory>()), Times.Once);
            repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// UTCID02 - Normal: Deactivate category đang active (true → false)
        /// </summary>
        [TestMethod]
        public async Task UpdateStatusAsync_DeactivateFromActive_UpdatesAndSaves()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            var entity = new SparePartCategory
            {
                CategoryId = 1,
                CategoryName = "Phanh",
                Description = "Phụ tùng phanh",
                IsActive = true
            };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.UpdateStatusAsync(1, false);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsActive);
            Assert.IsFalse(entity.IsActive);
            repo.Verify(x => x.Update(It.IsAny<SparePartCategory>()), Times.Once);
            repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// UTCID03 - Boundary: Idempotent - trạng thái không đổi (true → true) → KHÔNG gọi Update/Save
        /// </summary>
        [TestMethod]
        public async Task UpdateStatusAsync_SameStatus_NoOp_ReturnsResponse()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            var entity = new SparePartCategory
            {
                CategoryId = 1,
                CategoryName = "Phanh",
                Description = "Phụ tùng phanh",
                IsActive = true
            };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            var result = await service.UpdateStatusAsync(1, true);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsActive);
            repo.Verify(x => x.Update(It.IsAny<SparePartCategory>()), Times.Never);
            repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// UTCID04 - Abnormal: Id không tồn tại → trả null
        /// </summary>
        [TestMethod]
        public async Task UpdateStatusAsync_NotFound_ReturnsNull()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((SparePartCategory?)null);

            var result = await service.UpdateStatusAsync(99, false);

            Assert.IsNull(result);
        }

        /// <summary>
        /// UTCID05 - Abnormal: User không có quyền (không phải Admin/Supervisor) → throw UnauthorizedAccessException
        /// </summary>
        [TestMethod]
        public async Task UTCID05_UpdateStatusAsync_UnauthorizedRole_Throws()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var currentUser = new Mock<ICurrentUserService>();
            currentUser.Setup(x => x.IsAdmin()).Returns(false);
            currentUser.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            currentUser.Setup(x => x.GetCurrentUserId()).Returns(7);
            var service = new SparePartCategoryService(repo.Object, currentUser.Object);

            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                service.UpdateStatusAsync(1, false));
            Assert.AreEqual("Chỉ Supervisor được đổi trạng thái nhóm phụ tùng", ex.Message);
        }
    }
}
