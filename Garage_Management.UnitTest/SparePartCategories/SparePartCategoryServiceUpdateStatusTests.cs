using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.SparePartCategories
{
    [TestClass]
    public class SparePartCategoryServiceUpdateStatusTests
    {
        [TestMethod]
        public async Task UpdateStatusAsync_NotFound_ReturnsNull()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((SparePartCategory?)null);

            var result = await service.UpdateStatusAsync(99, false);

            Assert.IsNull(result);
        }
    }
}
