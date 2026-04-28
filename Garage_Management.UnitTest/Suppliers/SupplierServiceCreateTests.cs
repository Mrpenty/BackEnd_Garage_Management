using Garage_Management.Application.DTOs.Inventories.Suppliers;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Suppliers
{
    [TestClass]
    public class SupplierServiceCreateTests
    {
        private Mock<ISupplierRepository> _repo = null!;
        private SupplierService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<ISupplierRepository>();
            _service = new SupplierService(_repo.Object);
        }

        /// <summary>
        /// UTCID01 - Normal: Tạo nhà cung cấp cá nhân (Individual) với đầy đủ thông tin hợp lệ
        /// </summary>
        [TestMethod]
        public async Task UTCID01_CreateAsync_WithValidIndividual_ReturnsResponse()
        {
            var request = new SupplierCreateRequest
            {
                SupplierName = "Yamaha Parts VN",
                SupplierType = SupplierType.Individual,
                Phone = "0987654321",
                Address = "123 Đường Nguyễn Trãi, Quận 1, TP.HCM",
                TaxCode = "0123456789",
                IsActive = true
            };

            _repo.Setup(x => x.HasExistAsync("Yamaha Parts VN", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.AddAsync(It.IsAny<Supplier>(), It.IsAny<CancellationToken>()))
                .Callback<Supplier, CancellationToken>((e, _) => e.SupplierId = 1)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.SupplierId);
            Assert.AreEqual("Yamaha Parts VN", result.SupplierName);
            Assert.AreEqual(SupplierType.Individual, result.SupplierType);
            Assert.AreEqual("0987654321", result.Phone);
            Assert.AreEqual("123 Đường Nguyễn Trãi, Quận 1, TP.HCM", result.Address);
            Assert.AreEqual("0123456789", result.TaxCode);
            Assert.IsTrue(result.IsActive);
        }

        /// <summary>
        /// UTCID02 - Abnormal: SupplierName đã tồn tại trong hệ thống
        /// </summary>
        [TestMethod]
        public async Task UTCID02_CreateAsync_WithDuplicateName_Throws()
        {
            var request = new SupplierCreateRequest
            {
                SupplierName = "Yamaha Parts VN",
                SupplierType = SupplierType.Individual,
                Phone = "0987654321",
                Address = "123 Đường Nguyễn Trãi, Quận 1, TP.HCM",
                TaxCode = "0123456789",
                IsActive = true
            };

            _repo.Setup(x => x.HasExistAsync("Yamaha Parts VN", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
            Assert.AreEqual("Nhà cung cấp đã tồn tại", ex.Message);
        }

        /// <summary>
        /// UTCID03 - Abnormal: SupplierName toàn khoảng trắng
        /// </summary>
        [TestMethod]
        public async Task UTCID03_CreateAsync_WithWhitespaceName_Throws()
        {
            var request = new SupplierCreateRequest
            {
                SupplierName = "     ",
                SupplierType = SupplierType.Individual,
                Phone = "0987654321",
                Address = "123 Đường Nguyễn Trãi, Quận 1, TP.HCM",
                TaxCode = "0123456789",
                IsActive = true
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
            Assert.AreEqual("Phải nhập tên cho nhà cung cấp", ex.Message);
        }

        /// <summary>
        /// UTCID04 - Normal: Tạo nhà cung cấp doanh nghiệp (Business) thành công
        /// </summary>
        [TestMethod]
        public async Task UTCID04_CreateAsync_WithValidBusiness_ReturnsResponse()
        {
            var request = new SupplierCreateRequest
            {
                SupplierName = "Công ty TNHH Phụ tùng Honda Việt Nam",
                SupplierType = SupplierType.Business,
                Phone = "02838123456",
                Address = "456 Đường Lê Lợi, Quận 3, TP.HCM",
                TaxCode = "0312345678",
                IsActive = true
            };

            _repo.Setup(x => x.HasExistAsync("Công ty TNHH Phụ tùng Honda Việt Nam", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.AddAsync(It.IsAny<Supplier>(), It.IsAny<CancellationToken>()))
                .Callback<Supplier, CancellationToken>((e, _) => e.SupplierId = 2)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.SupplierId);
            Assert.AreEqual("Công ty TNHH Phụ tùng Honda Việt Nam", result.SupplierName);
            Assert.AreEqual(SupplierType.Business, result.SupplierType);
            Assert.AreEqual("02838123456", result.Phone);
            Assert.AreEqual("456 Đường Lê Lợi, Quận 3, TP.HCM", result.Address);
            Assert.AreEqual("0312345678", result.TaxCode);
            Assert.IsTrue(result.IsActive);
        }

        /// <summary>
        /// UTCID05 - Abnormal: SupplierType không hợp lệ (giá trị 0 không có trong enum)
        /// </summary>
        [TestMethod]
        public async Task UTCID05_CreateAsync_WithInvalidSupplierType_Throws()
        {
            var request = new SupplierCreateRequest
            {
                SupplierName = "Yamaha Parts VN",
                SupplierType = (SupplierType)0,
                Phone = "0987654321",
                Address = "123 Đường Nguyễn Trãi, Quận 1, TP.HCM",
                TaxCode = "0123456789",
                IsActive = true
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
            Assert.AreEqual("SupplierType không hợp lệ", ex.Message);
        }

        /// <summary>
        /// UTCID06 - Abnormal: SupplierName là chuỗi rỗng ""
        /// </summary>
        [TestMethod]
        public async Task UTCID06_CreateAsync_WithEmptyName_Throws()
        {
            var request = new SupplierCreateRequest
            {
                SupplierName = "",
                SupplierType = SupplierType.Business,
                Phone = "02838123456",
                Address = "456 Đường Lê Lợi, Quận 3, TP.HCM",
                TaxCode = "0312345678",
                IsActive = true
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
            Assert.AreEqual("Phải nhập tên cho nhà cung cấp", ex.Message);
        }
    }
}
