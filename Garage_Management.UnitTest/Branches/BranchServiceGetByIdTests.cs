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
    public class BranchServiceGetByIdTests
    {
        private Mock<IBranchRepository> _repo = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IBranchRepository>();
            _repo.Setup(x => x.CountEmployeesAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(3);
            _repo.Setup(x => x.CountActiveJobCardsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(5);
        }

        /// <summary>
        /// UTCID01 - Normal: Admin xem được bất kỳ chi nhánh
        /// </summary>
        [TestMethod]
        public async Task GetByIdAsync_Admin_ReturnsResponse()
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
            _repo.Setup(x => x.GetDetailByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await service.GetByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.BranchId);
            Assert.AreEqual(3, result.EmployeeCount);
            Assert.AreEqual(5, result.ActiveJobCardCount);
        }

        /// <summary>
        /// UTCID02 - Normal: Staff xem chi nhánh của mình
        /// </summary>
        [TestMethod]
        public async Task GetByIdAsync_Staff_OwnBranch_ReturnsResponse()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsStaff(branchId: 1));
            var entity = new Branch
            {
                BranchId = 1,
                BranchCode = "HN-01",
                Name = "Hà Nội",
                Address = "123",
                IsActive = true
            };
            _repo.Setup(x => x.GetDetailByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await service.GetByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.BranchId);
        }

        /// <summary>
        /// UTCID03 - Abnormal: Staff cố xem chi nhánh khác → throw UnauthorizedAccessException
        /// </summary>
        [TestMethod]
        public async Task GetByIdAsync_Staff_OtherBranch_Throws()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsStaff(branchId: 1));

            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                service.GetByIdAsync(2));
            Assert.AreEqual("Không có quyền truy cập chi nhánh khác", ex.Message);
        }

        /// <summary>
        /// UTCID04 - Abnormal: Admin query id không tồn tại → trả null
        /// </summary>
        [TestMethod]
        public async Task GetByIdAsync_Admin_NotFound_ReturnsNull()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            _repo.Setup(x => x.GetDetailByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Branch?)null);

            var result = await service.GetByIdAsync(99);

            Assert.IsNull(result);
        }

        /// <summary>
        /// UTCID05 - Boundary: Id = 0 (Admin, repo trả null)
        /// </summary>
        [TestMethod]
        public async Task GetByIdAsync_Admin_ZeroId_ReturnsNull()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            _repo.Setup(x => x.GetDetailByIdAsync(0, It.IsAny<CancellationToken>())).ReturnsAsync((Branch?)null);

            var result = await service.GetByIdAsync(0);

            Assert.IsNull(result);
        }
    }
}
