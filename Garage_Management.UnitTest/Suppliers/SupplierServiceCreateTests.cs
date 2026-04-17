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

        [TestMethod]
        public async Task CreateAsync_NameNull_Throws()
        {
            var request = new SupplierCreateRequest
            {
                SupplierName = null!,
                SupplierType = SupplierType.Business
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.CreateAsync(request, CancellationToken.None));

            _repo.Verify(x => x.HasExistAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()), Times.Never);
            _repo.Verify(x => x.AddAsync(It.IsAny<Supplier>(), It.IsAny<CancellationToken>()), Times.Never);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateAsync_NameWhitespace_Throws()
        {
            var request = new SupplierCreateRequest
            {
                SupplierName = "   ",
                SupplierType = SupplierType.Business
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.CreateAsync(request, CancellationToken.None));

            _repo.Verify(x => x.HasExistAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()), Times.Never);
            _repo.Verify(x => x.AddAsync(It.IsAny<Supplier>(), It.IsAny<CancellationToken>()), Times.Never);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateAsync_DuplicateName_Throws()
        {
            _repo.Setup(x => x.HasExistAsync("ABC", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.CreateAsync(new SupplierCreateRequest { SupplierName = "ABC", SupplierType = SupplierType.Business }, CancellationToken.None));

            _repo.Verify(x => x.AddAsync(It.IsAny<Supplier>(), It.IsAny<CancellationToken>()), Times.Never);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task CreateAsync_ValidBusiness_ReturnsResponse()
        {
            _repo.Setup(x => x.HasExistAsync("ABC", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.AddAsync(It.IsAny<Supplier>(), It.IsAny<CancellationToken>()))
                .Callback<Supplier, CancellationToken>((e, _) => e.SupplierId = 9)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(new SupplierCreateRequest
            {
                SupplierName = "ABC",
                SupplierType = SupplierType.Business,
                IsActive = true
            }, CancellationToken.None);

            Assert.AreEqual(9, result.SupplierId);
            Assert.AreEqual("ABC", result.SupplierName);
            Assert.AreEqual(SupplierType.Business, result.SupplierType);
            Assert.IsTrue(result.IsActive);

            _repo.Verify(x => x.AddAsync(It.IsAny<Supplier>(), It.IsAny<CancellationToken>()), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateAsync_NameHasSpaces_TrimBeforeDuplicateCheck()
        {
            _repo.Setup(x => x.HasExistAsync("ABC", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            Supplier? captured = null;
            _repo.Setup(x => x.AddAsync(It.IsAny<Supplier>(), It.IsAny<CancellationToken>()))
                .Callback<Supplier, CancellationToken>((e, _) =>
                {
                    captured = e;
                    e.SupplierId = 10;
                })
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(new SupplierCreateRequest
            {
                SupplierName = "  ABC  ",
                SupplierType = SupplierType.Business,
                IsActive = false
            }, CancellationToken.None);

            Assert.IsNotNull(captured);
            Assert.AreEqual("ABC", captured!.SupplierName);
            Assert.AreEqual("ABC", result.SupplierName);
            Assert.IsFalse(result.IsActive);
            _repo.Verify(x => x.HasExistAsync("ABC", null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateAsync_OptionalFieldsNull_MapsNull()
        {
            _repo.Setup(x => x.HasExistAsync("Supplier A", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            Supplier? captured = null;
            _repo.Setup(x => x.AddAsync(It.IsAny<Supplier>(), It.IsAny<CancellationToken>()))
                .Callback<Supplier, CancellationToken>((e, _) =>
                {
                    captured = e;
                    e.SupplierId = 11;
                })
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(new SupplierCreateRequest
            {
                SupplierName = "Supplier A",
                SupplierType = SupplierType.Individual,
                Phone = null,
                Address = null,
                TaxCode = null,
                IsActive = true
            }, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsNotNull(captured);
            Assert.IsNull(captured!.Phone);
            Assert.IsNull(captured.Address);
            Assert.IsNull(captured.TaxCode);
            Assert.AreEqual(SupplierType.Individual, result.SupplierType);
        }
    }
}
