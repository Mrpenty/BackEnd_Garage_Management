using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Suppliers
{
    [TestClass]
    public class SupplierServiceGetByIdTests
    {
        [TestMethod]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            var repo = new Mock<ISupplierRepository>();
            var service = new SupplierService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Supplier?)null);

            var result = await service.GetByIdAsync(99);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_Found_ReturnsMappedResponse()
        {
            var repo = new Mock<ISupplierRepository>();
            var service = new SupplierService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Supplier
            {
                SupplierId = 1,
                SupplierName = "ABC",
                SupplierType = SupplierType.Business,
                Phone = "0901234567",
                Address = "123 Nguyễn Huệ",
                IsActive = true
            });

            var result = await service.GetByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.SupplierId);
            Assert.AreEqual("ABC", result.SupplierName);
            Assert.AreEqual(SupplierType.Business, result.SupplierType);
            Assert.AreEqual("0901234567", result.Phone);
            Assert.IsTrue(result.IsActive);
        }
    }
}
