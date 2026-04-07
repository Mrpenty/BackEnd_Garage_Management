using Garage_Management.Application.DTOs.Inventories.Suppliers;
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
    public class SupplierServiceUpdateTests
    {
        [TestMethod]
        public async Task UpdateAsync_NotFound_ReturnsNull()
        {
            var repo = new Mock<ISupplierRepository>();
            var service = new SupplierService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Supplier?)null);

            var result = await service.UpdateAsync(99, new SupplierUpdateRequest
            {
                SupplierName = "ABC",
                SupplierType = SupplierType.Business
            });

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateAsync_EmptyName_Throws()
        {
            var repo = new Mock<ISupplierRepository>();
            var service = new SupplierService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Supplier { SupplierId = 1, SupplierName = "ABC" });

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.UpdateAsync(1, new SupplierUpdateRequest
                {
                    SupplierName = " ",
                    SupplierType = SupplierType.Business
                }));
        }

        [TestMethod]
        public async Task UpdateAsync_DuplicateName_Throws()
        {
            var repo = new Mock<ISupplierRepository>();
            var service = new SupplierService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Supplier { SupplierId = 1, SupplierName = "ABC" });
            repo.Setup(x => x.HasExistAsync("XYZ", 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.UpdateAsync(1, new SupplierUpdateRequest
                {
                    SupplierName = "XYZ",
                    SupplierType = SupplierType.Business
                }));
        }

        [TestMethod]
        public async Task UpdateAsync_Valid_UpdatesFieldsAndReturnsResponse()
        {
            var repo = new Mock<ISupplierRepository>();
            var service = new SupplierService(repo.Object);
            var entity = new Supplier
            {
                SupplierId = 1,
                SupplierName = "ABC",
                SupplierType = SupplierType.Business,
                IsActive = true
            };
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            repo.Setup(x => x.HasExistAsync("XYZ", 1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.UpdateAsync(1, new SupplierUpdateRequest
            {
                SupplierName = "XYZ",
                SupplierType = SupplierType.Individual,
                Phone = "0909999999",
                Address = "456 Lê Lợi"
            });

            Assert.IsNotNull(result);
            Assert.AreEqual("XYZ", result.SupplierName);
            Assert.AreEqual(SupplierType.Individual, result.SupplierType);
            Assert.AreEqual("0909999999", result.Phone);
            Assert.AreEqual("456 Lê Lợi", result.Address);
            Assert.IsTrue(result.IsActive);
        }
    }
}
