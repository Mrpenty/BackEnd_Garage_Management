using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.UnitTest.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Inventories
{
    [TestClass]
    public class InventoryServiceUpdateStatusTests
    {
        private Mock<IInventoryRepository> _repo = null!;
        private Mock<ISparePartCategoryRepository> _categoryRepo = null!;
        private Mock<ISparePartBrandRepository> _brandRepo = null!;
        private InventoryService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IInventoryRepository>();
            _categoryRepo = new Mock<ISparePartCategoryRepository>();
            _brandRepo = new Mock<ISparePartBrandRepository>();
            _service = new InventoryService(_repo.Object, _categoryRepo.Object, _brandRepo.Object, MockCurrentUser.AsStaff());
        }

        private Inventory MakeEntity(bool isActive) => new Inventory
        {
            SparePartId = 1,
            BranchId = 1,
            PartCode = "BG-001",
            PartName = "Bugi NGK CR7HSA",
            Unit = "Cái",
            Quantity = 20,
            IsActive = isActive
        };

        /// <summary>
        /// UTCID01 - Normal: Kích hoạt phụ tùng (inactive → active)
        /// </summary>
        [TestMethod]
        public async Task UTCID01_UpdateStatusAsync_ActivateFromInactive_ReturnsResponse()
        {
            var entity = MakeEntity(isActive: false);
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _repo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.UpdateStatusAsync(1, true);

            Assert.IsNotNull(result);
            Assert.IsTrue(entity.IsActive);
            Assert.IsNotNull(entity.UpdatedAt);
            _repo.Verify(x => x.Update(It.IsAny<Inventory>()), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// UTCID02 - Normal: Vô hiệu hóa phụ tùng (active → inactive)
        /// </summary>
        [TestMethod]
        public async Task UTCID02_UpdateStatusAsync_DeactivateFromActive_ReturnsResponse()
        {
            var entity = MakeEntity(isActive: true);
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _repo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.UpdateStatusAsync(1, false);

            Assert.IsNotNull(result);
            Assert.IsFalse(entity.IsActive);
            _repo.Verify(x => x.Update(It.IsAny<Inventory>()), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// UTCID03 - Boundary: Trạng thái không đổi (active → active) - idempotent, không gọi Update/Save
        /// </summary>
        [TestMethod]
        public async Task UTCID03_UpdateStatusAsync_SameStatus_NoOp_ReturnsResponse()
        {
            var entity = MakeEntity(isActive: true);
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _repo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.UpdateStatusAsync(1, true);

            Assert.IsNotNull(result);
            Assert.IsTrue(entity.IsActive);
            // Không gọi Update và Save vì trạng thái không đổi
            _repo.Verify(x => x.Update(It.IsAny<Inventory>()), Times.Never);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// UTCID04 - Abnormal: Id không tồn tại
        /// </summary>
        [TestMethod]
        public async Task UTCID04_UpdateStatusAsync_NotFound_Throws()
        {
            _repo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Inventory?)null);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateStatusAsync(999, true));
            Assert.AreEqual("Id không tồn tại", ex.Message);
        }
    }
}
