using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Inventories
{
    [TestClass]
    public class InventoryServiceDeleteTests
    {
        [TestMethod]
        public async Task DeleteAsync_HasDependencies_Throws()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object, new Mock<ISparePartCategoryRepository>().Object, new Mock<ISparePartBrandRepository>().Object);
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Inventory { SparePartId = 1, PartName = "Bugi" });
            repo.Setup(x => x.HasDependenciesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() => service.DeleteAsync(1));
        }

        [TestMethod]
        public async Task DeleteAsync_NoDependencies_ReturnsTrue()
        {
            var repo = new Mock<IInventoryRepository>();
            var service = new InventoryService(repo.Object, new Mock<ISparePartCategoryRepository>().Object, new Mock<ISparePartBrandRepository>().Object);
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Inventory { SparePartId = 1, PartName = "Bugi" });
            repo.Setup(x => x.HasDependenciesAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.DeleteAsync(1);
            Assert.IsTrue(result);
        }
    }
}
