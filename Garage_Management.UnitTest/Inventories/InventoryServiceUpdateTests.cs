using Garage_Management.Application.DTOs.Iventories;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.UnitTest.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Inventories
{
    [TestClass]
    public class InventoryServiceUpdateTests
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

            // Mặc định Query() trả về list rỗng
            _repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<Inventory>(new List<Inventory>().AsQueryable()));

            _service = new InventoryService(_repo.Object, _categoryRepo.Object, _brandRepo.Object, MockCurrentUser.AsStaff());
        }

        private Inventory MakeEntity() => new Inventory
        {
            SparePartId = 1,
            BranchId = 1,
            PartCode = "BG-001",
            PartName = "Bugi NGK CR7HSA",
            Unit = "Cái",
            CategoryId = 1,
            SparePartBrandId = 2,
            Quantity = 20,
            MinQuantity = 5,
            LastPurchasePrice = 25000m,
            SellingPrice = 35000m,
            IsActive = true
        };

        private void SetupValidCategoryAndBrand()
        {
            _categoryRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new SparePartCategory { CategoryId = 1, CategoryName = "Phụ tùng động cơ" });
            _brandRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new SparePartBrand { SparePartBrandId = 1, BrandName = "NGK" });
        }

        /// <summary>
        /// UTCID01 - Normal: Cập nhật đầy đủ các field hợp lệ
        /// </summary>
        [TestMethod]
        public async Task UTCID01_UpdateAsync_WithValidFullData_ReturnsResponse()
        {
            SetupValidCategoryAndBrand();
            var entity = MakeEntity();
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _repo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var request = new InventoryUpdateRequest
            {
                PartCode = "BG-002",
                PartName = "Bugi NGK CR8HSA",
                Unit = "Cái",
                CategoryId = 1,
                SparePartBrandId = 2,
                MinQuantity = 10,
                LastPurchasePrice = 28000m,
                SellingPrice = 40000m
            };

            var result = await _service.UpdateAsync(1, request);

            Assert.IsNotNull(result);
            Assert.AreEqual("BG-002", entity.PartCode);
            Assert.AreEqual("Bugi NGK CR8HSA", entity.PartName);
            Assert.AreEqual(10, entity.MinQuantity);
            Assert.AreEqual(28000m, entity.LastPurchasePrice);
            Assert.AreEqual(40000m, entity.SellingPrice);
        }

        /// <summary>
        /// UTCID02 - Abnormal: Không tìm thấy Inventory theo id
        /// </summary>
        [TestMethod]
        public async Task UTCID02_UpdateAsync_NotFound_Throws()
        {
            _repo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Inventory?)null);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateAsync(999, new InventoryUpdateRequest { PartName = "Test" }));
            Assert.AreEqual("Id không tồn tại", ex.Message);
        }

        /// <summary>
        /// UTCID03 - Abnormal: MinQuantity âm
        /// </summary>
        [TestMethod]
        public async Task UTCID03_UpdateAsync_WithNegativeMinQuantity_Throws()
        {
            var entity = MakeEntity();
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateAsync(1, new InventoryUpdateRequest { MinQuantity = -1 }));
            Assert.AreEqual("Số lượng phụ tùng tối thiểu không hợp lệ", ex.Message);
        }

        /// <summary>
        /// UTCID04 - Abnormal: LastPurchasePrice âm
        /// </summary>
        [TestMethod]
        public async Task UTCID04_UpdateAsync_WithNegativeLastPurchasePrice_Throws()
        {
            var entity = MakeEntity();
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateAsync(1, new InventoryUpdateRequest { LastPurchasePrice = -100m }));
            Assert.AreEqual("Giá mua cuối cùng không hợp lệ", ex.Message);
        }

        /// <summary>
        /// UTCID05 - Abnormal: SellingPrice âm
        /// </summary>
        [TestMethod]
        public async Task UTCID05_UpdateAsync_WithNegativeSellingPrice_Throws()
        {
            var entity = MakeEntity();
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateAsync(1, new InventoryUpdateRequest { SellingPrice = -50m }));
            Assert.AreEqual("Giá bán hiện tại không hợp lệ", ex.Message);
        }

        /// <summary>
        /// UTCID06 - Abnormal: CategoryId không tồn tại
        /// </summary>
        [TestMethod]
        public async Task UTCID06_UpdateAsync_WithInvalidCategoryId_Throws()
        {
            var entity = MakeEntity();
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _categoryRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((SparePartCategory?)null);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateAsync(1, new InventoryUpdateRequest { CategoryId = 999 }));
            Assert.AreEqual("CategoryId không tồn tại", ex.Message);
        }

        /// <summary>
        /// UTCID07 - Abnormal: SparePartBrandId không tồn tại
        /// </summary>
        [TestMethod]
        public async Task UTCID07_UpdateAsync_WithInvalidBrandId_Throws()
        {
            var entity = MakeEntity();
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _categoryRepo.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new SparePartCategory { CategoryId = 1, CategoryName = "Phụ tùng động cơ" });
            _brandRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((SparePartBrand?)null);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateAsync(1, new InventoryUpdateRequest { CategoryId = 1, SparePartBrandId = 999 }));
            Assert.AreEqual("SparePartBrandId không tồn tại", ex.Message);
        }

        /// <summary>
        /// UTCID08 - Abnormal: PartCode trùng với phụ tùng khác trong CÙNG chi nhánh (per-branch unique)
        /// </summary>
        [TestMethod]
        public async Task UTCID08_UpdateAsync_WithDuplicatePartCodeSameBranch_Throws()
        {
            var entity = MakeEntity();
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            // Có 1 phụ tùng khác (id=99) đã dùng PartCode "DUP-001" trong cùng Branch 1
            var existing = new List<Inventory>
            {
                new Inventory { SparePartId = 99, BranchId = 1, PartCode = "DUP-001", PartName = "Phụ tùng khác" }
            }.AsQueryable();
            _repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<Inventory>(existing));

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateAsync(1, new InventoryUpdateRequest { PartCode = "DUP-001" }));
            Assert.AreEqual("PartCode đã tồn tại", ex.Message);
        }

        /// <summary>
        /// UTCID11 - Normal: PartCode trùng nhưng KHÁC chi nhánh → cho phép update (per-branch unique scope)
        /// </summary>
        [TestMethod]
        public async Task UTCID11_UpdateAsync_WithSamePartCodeOtherBranch_Succeeds()
        {
            var entity = MakeEntity(); // BranchId = 1, PartCode = "BG-001"
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _repo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            // Phụ tùng (id=99) đã dùng PartCode "DUP-001" nhưng ở Branch 2
            var existing = new List<Inventory>
            {
                new Inventory { SparePartId = 99, BranchId = 2, PartCode = "DUP-001", PartName = "Phụ tùng chi nhánh 2" }
            }.AsQueryable();
            _repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<Inventory>(existing));

            var result = await _service.UpdateAsync(1, new InventoryUpdateRequest { PartCode = "DUP-001" });

            Assert.IsNotNull(result);
            Assert.AreEqual("DUP-001", entity.PartCode);
        }

        /// <summary>
        /// UTCID09 - Normal: Partial update (chỉ cập nhật 1 field PartName)
        /// </summary>
        [TestMethod]
        public async Task UTCID09_UpdateAsync_PartialUpdate_OnlyPartName_ReturnsResponse()
        {
            var entity = MakeEntity();
            var originalPrice = entity.SellingPrice;
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _repo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.UpdateAsync(1, new InventoryUpdateRequest { PartName = "Bugi NGK mới" });

            Assert.IsNotNull(result);
            Assert.AreEqual("Bugi NGK mới", entity.PartName);
            // Các field khác không đổi
            Assert.AreEqual("BG-001", entity.PartCode);
            Assert.AreEqual(originalPrice, entity.SellingPrice);
        }

        /// <summary>
        /// UTCID10 - Boundary: MinQuantity = 0 (biên hợp lệ)
        /// </summary>
        [TestMethod]
        public async Task UTCID10_UpdateAsync_WithZeroMinQuantity_ReturnsResponse()
        {
            var entity = MakeEntity();
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _repo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.UpdateAsync(1, new InventoryUpdateRequest { MinQuantity = 0 });

            Assert.IsNotNull(result);
            Assert.AreEqual(0, entity.MinQuantity);
        }
    }
}
