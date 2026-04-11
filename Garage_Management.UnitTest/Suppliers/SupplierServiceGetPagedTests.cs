using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Suppliers
{
    [TestClass]
    public class SupplierServiceGetPagedTests
    {
        [TestMethod]
        public async Task GetPagedAsync_ReturnsMappedPagedResult()
        {
            var repo = new Mock<ISupplierRepository>();
            var service = new SupplierService(repo.Object);
            var query = new ParamQuery();
            repo.Setup(x => x.GetPagedAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<Supplier>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 2,
                    PageData = new List<Supplier>
                    {
                        new Supplier { SupplierId = 1, SupplierName = "ABC", SupplierType = SupplierType.Business, IsActive = true },
                        new Supplier { SupplierId = 2, SupplierName = "XYZ", SupplierType = SupplierType.Individual, IsActive = false }
                    }
                });

            var result = await service.GetPagedAsync(query);

            Assert.AreEqual(2, result.Total);
            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(10, result.PageSize);
        }
    }
}
