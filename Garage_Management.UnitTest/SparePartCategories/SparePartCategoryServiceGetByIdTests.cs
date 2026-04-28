using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Inventories;
using Garage_Management.Base.Entities.Inventories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.SparePartCategories
{
    [TestClass]
    public class SparePartCategoryServiceGetByIdTests
    {
        [TestMethod]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((SparePartCategory?)null);

            var result = await service.GetByIdAsync(99);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_Found_ReturnsMappedResponse()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new SparePartCategory
            {
                CategoryId = 1,
                CategoryName = "Phanh",
                Description = "Phụ tùng phanh",
                IsActive = true
            });

            var result = await service.GetByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CategoryId);
            Assert.AreEqual("Phanh", result.CategoryName);
            Assert.AreEqual("Phụ tùng phanh", result.Description);
            Assert.IsTrue(result.IsActive);
        }

        /// <summary>
        /// UTCID03 - Normal: Category inactive → trả response với IsActive=false
        /// </summary>
        [TestMethod]
        public async Task GetByIdAsync_FoundInactive_ReturnsResponseWithIsActiveFalse()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new SparePartCategory
            {
                CategoryId = 2,
                CategoryName = "Lọc gió",
                Description = "Đã ngưng",
                IsActive = false
            });

            var result = await service.GetByIdAsync(2);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.CategoryId);
            Assert.AreEqual("Lọc gió", result.CategoryName);
            Assert.IsFalse(result.IsActive);
        }

        /// <summary>
        /// UTCID04 - Boundary: Id = 0 → service short-circuit (fail-fast), KHÔNG gọi repo
        /// </summary>
        [TestMethod]
        public async Task GetByIdAsync_ZeroId_ReturnsNullWithoutCallingRepo()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);

            var result = await service.GetByIdAsync(0);

            Assert.IsNull(result);
            repo.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        /// <summary>
        /// UTCID05 - Abnormal: Id âm → service short-circuit (fail-fast), KHÔNG gọi repo
        /// </summary>
        [TestMethod]
        public async Task GetByIdAsync_NegativeId_ReturnsNullWithoutCallingRepo()
        {
            var repo = new Mock<ISparePartCategoryRepository>();
            var service = new SparePartCategoryService(repo.Object);

            var result = await service.GetByIdAsync(-1);

            Assert.IsNull(result);
            repo.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
