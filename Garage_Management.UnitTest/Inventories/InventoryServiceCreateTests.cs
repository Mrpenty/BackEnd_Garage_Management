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
    public class InventoryServiceCreateTests
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

            // Mặc định Query() trả về list rỗng (không có PartCode trùng)
            var emptyInventories = new List<Inventory>().AsQueryable();
            _repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<Inventory>(emptyInventories));

            _service = new InventoryService(_repo.Object, _categoryRepo.Object, _brandRepo.Object);
        }

        private void SetupValidCategoryAndBrand()
        {
            _categoryRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new SparePartCategory { CategoryId = 1, CategoryName = "Phụ tùng động cơ" });
            _brandRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new SparePartBrand { SparePartBrandId = 1, BrandName = "NGK" });
        }

        /// <summary>
        /// UTCID01 - Normal: Tạo phụ tùng với đầy đủ thông tin hợp lệ
        /// </summary>
        [TestMethod]
        public async Task UTCID01_CreateAsync_WithValidFullData_ReturnsResponse()
        {
            SetupValidCategoryAndBrand();
            _repo.Setup(x => x.AddAsync(It.IsAny<Inventory>(), It.IsAny<CancellationToken>()))
                .Callback<Inventory, CancellationToken>((e, _) => e.SparePartId = 1)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _repo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Inventory?)null);

            var request = new InventoryCreateRequest
            {
                PartCode = "BG-001",
                PartName = "Bugi NGK CR7HSA",
                Unit = "Cái",
                CategoryId = 1,
                SparePartBrandId = 2,
                Quantity = 20,
                MinQuantity = 5,
                LastPurchasePrice = 25000,
                SellingPrice = 35000,
                IsActive = true
            };

            var result = await _service.CreateAsync(request);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.SparePartId);
            Assert.AreEqual("BG-001", result.PartCode);
            Assert.AreEqual("Bugi NGK CR7HSA", result.PartName);
            Assert.AreEqual("Cái", result.Unit);
            Assert.AreEqual(20, result.Quantity);
            Assert.AreEqual(5, result.MinQuantity);
            Assert.AreEqual(25000m, result.LastPurchasePrice);
            Assert.AreEqual(35000m, result.SellingPrice);
            Assert.IsTrue(result.IsActive);
        }

        /// <summary>
        /// UTCID02 - Abnormal: PartName toàn khoảng trắng
        /// </summary>
        [TestMethod]
        public async Task UTCID02_CreateAsync_WithWhitespacePartName_Throws()
        {
            var request = new InventoryCreateRequest
            {
                PartCode = "LOC-001",
                PartName = "     ",
                Quantity = 10,
                IsActive = true
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request));
            Assert.AreEqual("PartName không hợp lệ", ex.Message);
        }

        /// <summary>
        /// UTCID03 - Abnormal: PartName là chuỗi rỗng
        /// </summary>
        [TestMethod]
        public async Task UTCID03_CreateAsync_WithEmptyPartName_Throws()
        {
            var request = new InventoryCreateRequest
            {
                PartCode = "DAY-001",
                PartName = "",
                Quantity = 10,
                IsActive = true
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request));
            Assert.AreEqual("PartName không hợp lệ", ex.Message);
        }

        /// <summary>
        /// UTCID04 - Abnormal: Quantity âm
        /// </summary>
        [TestMethod]
        public async Task UTCID04_CreateAsync_WithNegativeQuantity_Throws()
        {
            var request = new InventoryCreateRequest
            {
                PartCode = "PHANH-001",
                PartName = "Phanh đĩa Sirius 110",
                Unit = "Cái",
                Quantity = -1,
                IsActive = true
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request));
            Assert.AreEqual("Quantity không hợp lệ", ex.Message);
        }

        /// <summary>
        /// UTCID05 - Boundary: Quantity = 0 (biên hợp lệ)
        /// </summary>
        [TestMethod]
        public async Task UTCID05_CreateAsync_WithZeroQuantity_ReturnsResponse()
        {
            SetupValidCategoryAndBrand();
            _repo.Setup(x => x.AddAsync(It.IsAny<Inventory>(), It.IsAny<CancellationToken>()))
                .Callback<Inventory, CancellationToken>((e, _) => e.SparePartId = 3)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _repo.Setup(x => x.GetByIdWithDetailsAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync((Inventory?)null);

            var request = new InventoryCreateRequest
            {
                PartCode = "NHT-001",
                PartName = "Nhớt Motul 5100 15W-50",
                Unit = "Chai",
                CategoryId = 3,
                SparePartBrandId = 3,
                Quantity = 0,
                LastPurchasePrice = 18000,
                SellingPrice = 28000,
                IsActive = true
            };

            var result = await _service.CreateAsync(request);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.SparePartId);
            Assert.AreEqual(0, result.Quantity);
        }

        /// <summary>
        /// UTCID06 - Normal: Tạo chỉ với các field bắt buộc (optional null)
        /// </summary>
        [TestMethod]
        public async Task UTCID06_CreateAsync_WithMinimalFields_ReturnsResponse()
        {
            _repo.Setup(x => x.AddAsync(It.IsAny<Inventory>(), It.IsAny<CancellationToken>()))
                .Callback<Inventory, CancellationToken>((e, _) => e.SparePartId = 4)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _repo.Setup(x => x.GetByIdWithDetailsAsync(4, It.IsAny<CancellationToken>())).ReturnsAsync((Inventory?)null);

            var request = new InventoryCreateRequest
            {
                PartCode = null,
                PartName = "Dây curoa Yamaha Exciter 150",
                Unit = null,
                CategoryId = null,
                SparePartBrandId = null,
                Quantity = 10,
                MinQuantity = null,
                LastPurchasePrice = null,
                SellingPrice = null,
                IsActive = true
            };

            var result = await _service.CreateAsync(request);

            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.SparePartId);
            Assert.IsNull(result.PartCode);
            Assert.AreEqual("Dây curoa Yamaha Exciter 150", result.PartName);
            Assert.IsNull(result.Unit);
            Assert.IsNull(result.CategoryId);
            Assert.IsNull(result.SparePartBrandId);
            Assert.AreEqual(10, result.Quantity);
        }

        /// <summary>
        /// UTCID07 - Abnormal: MinQuantity âm
        /// </summary>
        [TestMethod]
        public async Task UTCID07_CreateAsync_WithNegativeMinQuantity_Throws()
        {
            var request = new InventoryCreateRequest
            {
                PartCode = "LOC-002",
                PartName = "Lọc gió Honda Wave",
                Quantity = 10,
                MinQuantity = -1,
                IsActive = true
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request));
            Assert.AreEqual("MinQuantity không hợp lệ", ex.Message);
        }

        /// <summary>
        /// UTCID08 - Abnormal: LastPurchasePrice âm
        /// </summary>
        [TestMethod]
        public async Task UTCID08_CreateAsync_WithNegativeLastPurchasePrice_Throws()
        {
            var request = new InventoryCreateRequest
            {
                PartCode = "NHT-002",
                PartName = "Nhớt Castrol Power1",
                Quantity = 10,
                LastPurchasePrice = -100,
                IsActive = true
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request));
            Assert.AreEqual("LastPurchasePrice không hợp lệ", ex.Message);
        }

        /// <summary>
        /// UTCID09 - Abnormal: SellingPrice âm
        /// </summary>
        [TestMethod]
        public async Task UTCID09_CreateAsync_WithNegativeSellingPrice_Throws()
        {
            var request = new InventoryCreateRequest
            {
                PartCode = "NHT-003",
                PartName = "Nhớt Shell Advance",
                Quantity = 10,
                SellingPrice = -50,
                IsActive = true
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request));
            Assert.AreEqual("SellingPrice không hợp lệ", ex.Message);
        }

        /// <summary>
        /// UTCID10 - Abnormal: CategoryId không tồn tại
        /// </summary>
        [TestMethod]
        public async Task UTCID10_CreateAsync_WithInvalidCategoryId_Throws()
        {
            _categoryRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((SparePartCategory?)null);

            var request = new InventoryCreateRequest
            {
                PartCode = "BUG-002",
                PartName = "Bugi Denso Iridium",
                Quantity = 10,
                CategoryId = 999,
                IsActive = true
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request));
            Assert.AreEqual("CategoryId không tồn tại", ex.Message);
        }

        /// <summary>
        /// UTCID11 - Abnormal: SparePartBrandId không tồn tại
        /// </summary>
        [TestMethod]
        public async Task UTCID11_CreateAsync_WithInvalidBrandId_Throws()
        {
            _categoryRepo.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new SparePartCategory { CategoryId = 1, CategoryName = "Phụ tùng động cơ" });
            _brandRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((SparePartBrand?)null);

            var request = new InventoryCreateRequest
            {
                PartCode = "BUG-003",
                PartName = "Bugi Bosch Platinum",
                Quantity = 10,
                CategoryId = 1,
                SparePartBrandId = 999,
                IsActive = true
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request));
            Assert.AreEqual("SparePartBrandId không tồn tại", ex.Message);
        }

        /// <summary>
        /// UTCID12 - Abnormal: PartCode đã tồn tại (trùng lặp)
        /// </summary>
        [TestMethod]
        public async Task UTCID12_CreateAsync_WithDuplicatePartCode_Throws()
        {
            SetupValidCategoryAndBrand();
            var existing = new List<Inventory>
            {
                new Inventory { SparePartId = 99, PartCode = "BG-001", PartName = "Bugi cũ" }
            }.AsQueryable();
            _repo.Setup(x => x.Query()).Returns(new TestAsyncEnumerable<Inventory>(existing));

            var request = new InventoryCreateRequest
            {
                PartCode = "BG-001",
                PartName = "Bugi NGK mới",
                Quantity = 10,
                CategoryId = 1,
                SparePartBrandId = 1,
                IsActive = true
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request));
            Assert.AreEqual("PartCode đã tồn tại", ex.Message);
        }
    }
}
