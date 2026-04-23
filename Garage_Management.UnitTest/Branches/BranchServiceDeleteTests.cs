using Garage_Management.Application.Interfaces.Repositories.Branches;
using Garage_Management.Application.Services.Branches;
using Garage_Management.Base.Entities.Branches;
using Garage_Management.UnitTest.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Branches
{
    [TestClass]
    public class BranchServiceDeleteTests
    {
        private Mock<IBranchRepository> _repo = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IBranchRepository>();
        }

        /// <summary>
        /// UTCID01 - Normal: Admin xóa chi nhánh không có dependencies → soft delete thành công
        /// </summary>
        [TestMethod]
        public async Task DeleteAsync_Admin_NoDependencies_SoftDeletes()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            var entity = new Branch { BranchId = 1, BranchCode = "HN-01", Name = "HN", Address = "123" };
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _repo.Setup(x => x.HasDependenciesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.DeleteAsync(1);

            Assert.IsTrue(result);
            Assert.IsNotNull(entity.DeletedAt);
            _repo.Verify(x => x.Update(It.IsAny<Branch>()), Times.Once);
        }

        /// <summary>
        /// UTCID02 - Abnormal: Non-admin không được xóa
        /// </summary>
        [TestMethod]
        public async Task DeleteAsync_NonAdmin_Throws()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsStaff());

            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                service.DeleteAsync(1));
            Assert.AreEqual("Chỉ Admin được xóa chi nhánh", ex.Message);
        }

        /// <summary>
        /// UTCID03 - Abnormal: Chi nhánh có dependencies (employee/jobcard/inventory) → throw
        /// </summary>
        [TestMethod]
        public async Task DeleteAsync_HasDependencies_Throws()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            var entity = new Branch { BranchId = 1, BranchCode = "HN-01", Name = "HN", Address = "123" };
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _repo.Setup(x => x.HasDependenciesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.DeleteAsync(1));
            Assert.AreEqual("Không thể xóa chi nhánh vì còn nhân viên / phụ tùng / phiếu sửa chữa", ex.Message);
        }

        /// <summary>
        /// UTCID04 - Abnormal: Id không tồn tại → trả false
        /// </summary>
        [TestMethod]
        public async Task DeleteAsync_NotFound_ReturnsFalse()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            _repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Branch?)null);

            var result = await service.DeleteAsync(99);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// UTCID05 - Abnormal: Chi nhánh đã soft-deleted → trả false
        /// </summary>
        [TestMethod]
        public async Task DeleteAsync_AlreadyDeleted_ReturnsFalse()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            var entity = new Branch
            {
                BranchId = 1,
                BranchCode = "HN-01",
                Name = "HN",
                Address = "123",
                DeletedAt = DateTime.UtcNow
            };
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            var result = await service.DeleteAsync(1);

            Assert.IsFalse(result);
        }
    }
}
