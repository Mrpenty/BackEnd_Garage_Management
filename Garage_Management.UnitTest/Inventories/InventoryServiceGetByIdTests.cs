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
    public class InventoryServiceGetByIdTests
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

        /// <summary>
        /// UTCID01 - Normal: Tìm thấy phụ tùng với đầy đủ thông tin Brand + Category
        /// </summary>
        [TestMethod]
        public async Task UTCID01_GetByIdAsync_FoundWithFullDetails_ReturnsMappedResponse()
        {
            var entity = new Inventory
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
                IsActive = true,
                CreatedAt = new DateTime(2026, 4, 18, 10, 0, 0, DateTimeKind.Utc),
                SparePartBrand = new SparePartBrand { SparePartBrandId = 2, BrandName = "NGK" },
                SparePartCategory = new SparePartCategory { CategoryId = 1, CategoryName = "Phụ tùng động cơ" }
            };
            _repo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(1);

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
            Assert.AreEqual("NGK", result.SparePartBrandName);
            Assert.AreEqual("Phụ tùng động cơ", result.CategoryName);
        }

        /// <summary>
        /// UTCID02 - Normal: Tìm thấy phụ tùng nhưng không có Brand/Category (các navigation null)
        /// </summary>
        [TestMethod]
        public async Task UTCID02_GetByIdAsync_FoundWithoutBrandCategory_ReturnsMappedResponse()
        {
            var entity = new Inventory
            {
                SparePartId = 2,
                BranchId = 1,
                PartCode = null,
                PartName = "Dây curoa Yamaha Exciter 150",
                Unit = null,
                CategoryId = null,
                SparePartBrandId = null,
                Quantity = 10,
                MinQuantity = null,
                LastPurchasePrice = null,
                SellingPrice = null,
                IsActive = true,
                SparePartBrand = null,
                SparePartCategory = null
            };
            _repo.Setup(x => x.GetByIdWithDetailsAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(2);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.SparePartId);
            Assert.AreEqual("Dây curoa Yamaha Exciter 150", result.PartName);
            Assert.IsNull(result.PartCode);
            Assert.IsNull(result.Unit);
            Assert.IsNull(result.CategoryId);
            Assert.IsNull(result.CategoryName);
            Assert.IsNull(result.SparePartBrandId);
            Assert.IsNull(result.SparePartBrandName);
            Assert.IsNull(result.MinQuantity);
            Assert.IsNull(result.LastPurchasePrice);
            Assert.IsNull(result.SellingPrice);
        }

        /// <summary>
        /// UTCID03 - Abnormal: Id không tồn tại → trả null (HTTP 404 chuẩn REST)
        /// </summary>
        [TestMethod]
        public async Task UTCID03_GetByIdAsync_NotFound_ReturnsNull()
        {
            _repo.Setup(x => x.GetByIdWithDetailsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Inventory?)null);

            var result = await _service.GetByIdAsync(999);

            Assert.IsNull(result);
        }
    }
}
