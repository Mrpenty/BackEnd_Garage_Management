using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Suppliers
{
    [TestClass]
    public class SupplierServiceGetByIdTests
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
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            _repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Supplier?)null);

            var result = await _service.GetByIdAsync(99, CancellationToken.None);

            Assert.IsNull(result);
            _repo.Verify(x => x.GetByIdAsync(99), Times.Once);
        }

        [TestMethod]
        public async Task GetByIdAsync_Found_ReturnsMappedResponse()
        {
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Supplier
            {
                SupplierId = 1,
                SupplierName = "ABC",
                SupplierType = SupplierType.Business,
                Phone = "0901234567",
                Address = "123 Nguyen Hue",
                TaxCode = "0101234567",
                IsActive = true
            });

            var result = await _service.GetByIdAsync(1, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.SupplierId);
            Assert.AreEqual("ABC", result.SupplierName);
            Assert.AreEqual(SupplierType.Business, result.SupplierType);
            Assert.AreEqual("0901234567", result.Phone);
            Assert.AreEqual("123 Nguyen Hue", result.Address);
            Assert.AreEqual("0101234567", result.TaxCode);
            Assert.IsTrue(result.IsActive);
            _repo.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [TestMethod]
        public async Task GetByIdAsync_FoundInactiveAndOptionalNull_MapsCorrectly()
        {
            _repo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new Supplier
            {
                SupplierId = 2,
                SupplierName = "Supplier B",
                SupplierType = SupplierType.Individual,
                Phone = null,
                Address = null,
                TaxCode = null,
                IsActive = false
            });

            var result = await _service.GetByIdAsync(2, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.SupplierId);
            Assert.AreEqual("Supplier B", result.SupplierName);
            Assert.AreEqual(SupplierType.Individual, result.SupplierType);
            Assert.IsNull(result.Phone);
            Assert.IsNull(result.Address);
            Assert.IsNull(result.TaxCode);
            Assert.IsFalse(result.IsActive);
            _repo.Verify(x => x.GetByIdAsync(2), Times.Once);
        }
    }
}
