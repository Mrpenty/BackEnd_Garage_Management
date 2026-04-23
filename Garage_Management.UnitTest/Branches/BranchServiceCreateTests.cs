using Garage_Management.Application.DTOs.Branches;
using Garage_Management.Application.Interfaces.Repositories.Branches;
using Garage_Management.Application.Services.Branches;
using Garage_Management.Base.Entities.Branches;
using Garage_Management.UnitTest.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Branches
{
    [TestClass]
    public class BranchServiceCreateTests
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
        /// UTCID01 - Normal: Admin tạo chi nhánh mới hợp lệ
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_Admin_Valid_ReturnsResponse()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            _repo.Setup(x => x.CodeExistsAsync("HN-01", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.AddAsync(It.IsAny<Branch>(), It.IsAny<CancellationToken>()))
                .Callback<Branch, CancellationToken>((e, _) => e.BranchId = 1)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.CreateAsync(new BranchCreateRequest
            {
                BranchCode = "HN-01",
                Name = "Chi nhánh Hà Nội",
                Address = "123 Đường Láng",
                Phone = "0912345678",
                Email = "hn@garage.vn",
                IsActive = true
            });

            Assert.AreEqual(1, result.BranchId);
            Assert.AreEqual("HN-01", result.BranchCode);
            Assert.AreEqual("Chi nhánh Hà Nội", result.Name);
            Assert.IsTrue(result.IsActive);
        }

        /// <summary>
        /// UTCID02 - Abnormal: Non-admin (staff) không được tạo chi nhánh
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_NonAdmin_Throws()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsStaff());

            var ex = await Assert.ThrowsExceptionAsync<System.UnauthorizedAccessException>(() =>
                service.CreateAsync(new BranchCreateRequest
                {
                    BranchCode = "HN-01",
                    Name = "Chi nhánh Hà Nội",
                    Address = "123 Đường Láng"
                }));
            Assert.AreEqual("Chỉ Admin được tạo chi nhánh", ex.Message);
        }

        /// <summary>
        /// UTCID03 - Abnormal: BranchCode rỗng
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_EmptyBranchCode_Throws()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new BranchCreateRequest
                {
                    BranchCode = " ",
                    Name = "Chi nhánh Hà Nội",
                    Address = "123 Đường Láng"
                }));
            Assert.AreEqual("Phải nhập mã chi nhánh", ex.Message);
        }

        /// <summary>
        /// UTCID04 - Abnormal: BranchCode đã tồn tại
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_DuplicateBranchCode_Throws()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            _repo.Setup(x => x.CodeExistsAsync("HN-01", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new BranchCreateRequest
                {
                    BranchCode = "HN-01",
                    Name = "Chi nhánh Hà Nội",
                    Address = "123 Đường Láng"
                }));
            Assert.AreEqual("Mã chi nhánh đã tồn tại", ex.Message);
        }

        /// <summary>
        /// UTCID05 - Normal: Tạo với ManagerEmployeeId
        /// </summary>
        [TestMethod]
        public async Task CreateAsync_WithManagerEmployeeId_ReturnsResponseWithManager()
        {
            var service = new BranchService(_repo.Object, MockCurrentUser.AsAdmin());
            _repo.Setup(x => x.CodeExistsAsync("SG-01", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.AddAsync(It.IsAny<Branch>(), It.IsAny<CancellationToken>()))
                .Callback<Branch, CancellationToken>((e, _) => e.BranchId = 2)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.CreateAsync(new BranchCreateRequest
            {
                BranchCode = "SG-01",
                Name = "Chi nhánh Sài Gòn",
                Address = "456 Nguyễn Huệ",
                ManagerEmployeeId = 5,
                IsActive = true
            });

            Assert.AreEqual(2, result.BranchId);
            Assert.AreEqual(5, result.ManagerEmployeeId);
        }
    }
}
