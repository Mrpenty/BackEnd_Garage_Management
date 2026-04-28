using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Application.Services.JobCards;
using Moq;
using System.Collections.Generic;

namespace Garage_Management.UnitTest.JobCardSpareParts
{
    [TestClass]
    public class JobCardSparePartServiceGetTests
    {
        private Mock<IJobCardRepository> _jobCardRepo = null!;
        private Mock<IJobCardSparePartRepository> _jobCardSparePartRepo = null!;
        private Mock<IInventoryRepository> _inventoryRepo = null!;
        private Mock<IStockTransactionService> _stockTransactionService = null!;
        private JobCardSparePartService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _jobCardRepo = new Mock<IJobCardRepository>();
            _jobCardSparePartRepo = new Mock<IJobCardSparePartRepository>();
            _inventoryRepo = new Mock<IInventoryRepository>();
            _stockTransactionService = new Mock<IStockTransactionService>();
            _service = new JobCardSparePartService(
                _jobCardRepo.Object,
                _jobCardSparePartRepo.Object,
                _inventoryRepo.Object,
                _stockTransactionService.Object);
        }

        [TestMethod]
        public async Task GetAllAsync_ReturnsMappedItems()
        {
            _jobCardSparePartRepo.Setup(x => x.GetAllWithDetailsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                [
                    new Base.Entities.JobCards.JobCardSparePart
                    {
                        JobCardId = 1,
                        SparePartId = 2,
                        Quantity = 3,
                        UnitPrice = 100,
                        TotalAmount = 300
                    }
                ]);

            var result = await _service.GetAllAsync(CancellationToken.None);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(300m, result[0].TotalAmount);
        }

        [TestMethod]
        public async Task GetByJobCardIdAsync_ReturnsMappedItems()
        {
            _jobCardRepo.Setup(x => x.GetByIdAsync(5))
                .ReturnsAsync(new Base.Entities.JobCards.JobCard { JobCardId = 5 });

            _jobCardSparePartRepo.Setup(x => x.GetByJobCardIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                [
                    new Base.Entities.JobCards.JobCardSparePart
                    {
                        JobCardId = 5,
                        SparePartId = 10,
                        Quantity = 1,
                        UnitPrice = 50,
                        TotalAmount = 50
                    }
                ]);

            var result = await _service.GetByJobCardIdAsync(5, CancellationToken.None);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(10, result[0].SparePartId);
        }

        [TestMethod]
        public async Task GetByJobCardIdAsync_ThrowsKeyNotFoundException_WhenJobCardDoesNotExist()
        {
            _jobCardRepo.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Base.Entities.JobCards.JobCard?)null);

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                () => _service.GetByJobCardIdAsync(999, CancellationToken.None));

            _jobCardSparePartRepo.Verify(
                x => x.GetByJobCardIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
