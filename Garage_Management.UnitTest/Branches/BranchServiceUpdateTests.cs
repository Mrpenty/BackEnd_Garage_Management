using Garage_Management.Application.DTOs.Branches;
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
    public class BranchServiceUpdateTests
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
        /// UTCID01 - Normal: Admin cập nhật chi nhánh
        /// </summary>
        [TestMethod]
        public async Task UpdateAsync_Admin_Valid_UpdatesEntity()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            var entity = new Branch
            {
                BranchId = 1,
                BranchCode = "HN-01",
                Name = "Cũ",
                Address = "Địa chỉ cũ"
            };
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.UpdateAsync(1, new BranchUpdateRequest
            {
                Name = "Mới",
                Address = "Địa chỉ mới",
                Phone = "0912345678"
            });

            Assert.IsNotNull(result);
            Assert.AreEqual("Mới", entity.Name);
            Assert.AreEqual("Địa chỉ mới", entity.Address);
            Assert.IsNotNull(entity.UpdatedAt);
        }

        /// <summary>
        /// UTCID02 - Abnormal: Non-admin không được update
        /// </summary>
        [TestMethod]
        public async Task UpdateAsync_NonAdmin_Throws()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsStaff());

            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                service.UpdateAsync(1, new BranchUpdateRequest { Name = "X", Address = "Y" }));
            Assert.AreEqual("Chỉ Admin được cập nhật chi nhánh", ex.Message);
        }

        /// <summary>
        /// UTCID03 - Abnormal: Id không tồn tại → trả null
        /// </summary>
        [TestMethod]
        public async Task UpdateAsync_NotFound_ReturnsNull()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            _repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Branch?)null);

            var result = await service.UpdateAsync(99, new BranchUpdateRequest { Name = "X", Address = "Y" });

            Assert.IsNull(result);
        }

        /// <summary>
        /// UTCID04 - Abnormal: Chi nhánh đã soft-deleted → trả null
        /// </summary>
        [TestMethod]
        public async Task UpdateAsync_SoftDeleted_ReturnsNull()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            var entity = new Branch
            {
                BranchId = 1,
                BranchCode = "HN-01",
                Name = "Cũ",
                Address = "Địa chỉ",
                DeletedAt = DateTime.UtcNow
            };
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            var result = await service.UpdateAsync(1, new BranchUpdateRequest { Name = "X", Address = "Y" });

            Assert.IsNull(result);
        }
    }
}
