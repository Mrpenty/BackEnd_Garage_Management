using Garage_Management.Application.DTOs.Inventories.SparePartCategories;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.SparePartCategories
{
    [TestClass]
    public class SparePartCategoryServiceCreateTests
    {
        [TestMethod]
        public async Task CreateAsync_EmptyName_Throws()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new SparePartCategoryCreateRequest { CategoryName = " " }));
        }

        [TestMethod]
        public async Task CreateAsync_Duplicate_Throws()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            repo.Setup(x => x.HasExistAsync("Phanh", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new SparePartCategoryCreateRequest { CategoryName = "Phanh" }));
        }

        [TestMethod]
        public async Task CreateAsync_Valid_ReturnsResponse()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            repo.Setup(x => x.HasExistAsync("Phanh", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.AddAsync(It.IsAny<SparePartCategory>(), It.IsAny<CancellationToken>()))
                .Callback<SparePartCategory, CancellationToken>((e, _) => e.CategoryId = 5)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.CreateAsync(new SparePartCategoryCreateRequest
            {
                CategoryName = "Phanh",
                Description = "Phụ tùng phanh",
                IsActive = true
            });

            Assert.AreEqual(5, result.CategoryId);
            Assert.AreEqual("Phanh", result.CategoryName);
            Assert.IsTrue(result.IsActive);
        }
    }
}
