using Garage_Management.Application.DTOs.Iventories.StockTransactions;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Inventories;
using Moq;
using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;
using JobCardSparePartEntity = Garage_Management.Base.Entities.JobCards.JobCardSparePart;

namespace Garage_Management.UnitTest.JobCardSpareParts
{
    [TestClass]
    public class JobCardSparePartServiceRemoveTests
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
        public async Task RemoveSparePartAsync_ReturnsFalse_WhenSparePartDoesNotExistInJobCard()
        {
            _jobCardSparePartRepo.Setup(x => x.GetByIdAsync(9, 99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobCardSparePartEntity?)null);

            var result = await _service.RemoveSparePartAsync(9, 99, CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task RemoveSparePartAsync_DeletesEntityAndCreatesReturnTransaction()
        {
            var entity = new JobCardSparePartEntity
            {
                JobCardId = 7,
                SparePartId = 13,
                Quantity = 3,
                UnitPrice = 90
            };

            StockTransactionCreateRequest? capturedRequest = null;

            _jobCardSparePartRepo.Setup(x => x.GetByIdAsync(7, 13, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);
            _jobCardRepo.Setup(x => x.GetByIdAsync(7))
                .ReturnsAsync(new JobCardEntity { JobCardId = 7 });
            _inventoryRepo.Setup(x => x.GetByIdAsync(13))
                .ReturnsAsync(new Inventory { SparePartId = 13, Quantity = 20, SellingPrice = 90, IsActive = true });
            _stockTransactionService.Setup(x => x.CreateAsync(It.IsAny<StockTransactionCreateRequest>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransactionCreateRequest, CancellationToken>((request, _) => capturedRequest = request)
                .ReturnsAsync(new StockTransactionResponse());

            var result = await _service.RemoveSparePartAsync(7, 13, CancellationToken.None);

            Assert.IsTrue(result);
            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(TransactionType.ReturnFromJobCard, capturedRequest.TransactionType);
            Assert.AreEqual(3, capturedRequest.QuantityChange);
            _jobCardSparePartRepo.Verify(x => x.Delete(entity), Times.Once);
            _jobCardSparePartRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
