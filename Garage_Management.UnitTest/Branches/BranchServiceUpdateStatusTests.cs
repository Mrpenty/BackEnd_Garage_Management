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
    public class BranchServiceUpdateStatusTests
    {
        private Mock<IBranchRepository> _repo = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IBranchRepository>();
            _repo.Setup(x => x.CountEmployeesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
            _repo.Setup(x => x.CountActiveJobCardsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
        }

        /// <summary>
        /// UTCID01 - Normal: Admin activate chi nhánh (false → true)
        /// </summary>
        [TestMethod]
        public async Task UpdateStatusAsync_ActivateFromInactive_UpdatesAndSaves()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            var entity = new Branch
            {
                BranchId = 1,
                BranchCode = "HN-01",
                Name = "Hà Nội",
                Address = "123",
                IsActive = false
            };
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.UpdateStatusAsync(1, true);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsActive);
            Assert.IsTrue(entity.IsActive);
            _repo.Verify(x => x.Update(It.IsAny<Branch>()), Times.Once);
        }

        /// <summary>
        /// UTCID02 - Normal: Admin deactivate chi nhánh (true → false)
        /// </summary>
        [TestMethod]
        public async Task UpdateStatusAsync_DeactivateFromActive_UpdatesAndSaves()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            var entity = new Branch
            {
                BranchId = 1,
                BranchCode = "HN-01",
                Name = "Hà Nội",
                Address = "123",
                IsActive = true
            };
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.UpdateStatusAsync(1, false);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsActive);
            _repo.Verify(x => x.Update(It.IsAny<Branch>()), Times.Once);
        }

        /// <summary>
        /// UTCID03 - Boundary: Idempotent - trạng thái không đổi → KHÔNG gọi Update/Save
        /// </summary>
        [TestMethod]
        public async Task UpdateStatusAsync_SameStatus_NoOp()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            var entity = new Branch
            {
                BranchId = 1,
                BranchCode = "HN-01",
                Name = "Hà Nội",
                Address = "123",
                IsActive = true
            };
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            var result = await service.UpdateStatusAsync(1, true);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsActive);
            _repo.Verify(x => x.Update(It.IsAny<Branch>()), Times.Never);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// UTCID04 - Abnormal: Non-admin không được đổi trạng thái
        /// </summary>
        [TestMethod]
        public async Task UpdateStatusAsync_NonAdmin_Throws()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsStaff());

            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                service.UpdateStatusAsync(1, true));
            Assert.AreEqual("Chỉ Admin được đổi trạng thái chi nhánh", ex.Message);
        }

        /// <summary>
        /// UTCID05 - Abnormal: Id không tồn tại → trả null
        /// </summary>
        [TestMethod]
        public async Task UpdateStatusAsync_NotFound_ReturnsNull()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            _repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Branch?)null);

            var result = await service.UpdateStatusAsync(99, true);

            Assert.IsNull(result);
        }
    }
}
