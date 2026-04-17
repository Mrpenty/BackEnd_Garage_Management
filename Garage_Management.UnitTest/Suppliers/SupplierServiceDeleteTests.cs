using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Suppliers
{
    [TestClass]
    public class SupplierServiceDeleteTests
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
        public async Task DeleteAsync_NotFound_ReturnsFalse()
        {
            _repo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Supplier?)null);

            var result = await _service.DeleteAsync(999, CancellationToken.None);

            Assert.IsFalse(result);
            _repo.Verify(x => x.HasStockTransactionsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _repo.Verify(x => x.Delete(It.IsAny<Supplier>()), Times.Never);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task DeleteAsync_HasStockTransactions_Throws()
        {
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Supplier { SupplierId = 1, SupplierName = "ABC" });
            _repo.Setup(x => x.HasStockTransactionsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var ex = await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                _service.DeleteAsync(1, CancellationToken.None));

            Assert.AreEqual("Không thể xóa nhà cung cấp vì đã phát sinh giao dịch kho", ex.Message);
            _repo.Verify(x => x.Delete(It.IsAny<Supplier>()), Times.Never);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task DeleteAsync_NoDependencies_ReturnsTrue()
        {
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Supplier { SupplierId = 1, SupplierName = "ABC" });
            _repo.Setup(x => x.HasStockTransactionsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.DeleteAsync(1, CancellationToken.None);

            Assert.IsTrue(result);
            _repo.Verify(x => x.Delete(It.IsAny<Supplier>()), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
