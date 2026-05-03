using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.UnitTest.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Inventories
{
    [TestClass]
    public class InventoryServiceGetByBrandTests
    {
        [TestMethod]
        public async Task GetByBrandIdAsync_ReturnsMappedList()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object, new Mock<ISparePartCategoryRepository>().Object, new Mock<ISparePartBrandRepository>().Object, MockCurrentUser.AsStaff());
            repo.Setup(x => x.GetByBrandIdAsync(2, It.IsAny<int?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Inventory>
                {
                    new Inventory { SparePartId = 1, PartName = "Bugi", SparePartBrandId = 2 },
                    new Inventory { SparePartId = 2, PartName = "Lọc gió", SparePartBrandId = 2 }
                });

            var result = await service.GetByBrandIdAsync(2);

            Assert.AreEqual(2, result.Count);
        }
    }
}
