using Garage_Management.Application.DTOs.Iventories.StockTransactions;
using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Entities.JobCards;
using Moq;
using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;

namespace Garage_Management.UnitTest.JobCards.JobCardSpareParts
{
    [TestClass]
    public class JobCardSparePartServiceTests
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
        public async Task AddSparePartsAsync_Throws_WhenJobCardIdIsInvalid()
        {
            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts = new List<AddSparePartToJobCardDto>
                {
                    new AddSparePartToJobCardDto { SparePartId = 1, Quantity = 1 }
                }
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.AddSparePartsAsync(0, dto, CancellationToken.None));
        }

        [TestMethod]
        public async Task AddSparePartsAsync_ReturnsNull_WhenJobCardDoesNotExist()
        {
            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts = new List<AddSparePartToJobCardDto>
                {
                    new AddSparePartToJobCardDto { SparePartId = 1, Quantity = 1 }
                }
            };

            _jobCardRepo
                .Setup(x => x.GetByIdAsync(2))
                .ReturnsAsync((JobCardEntity?)null);

            var result = await _service.AddSparePartsAsync(2, dto, CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AddSparePartsAsync_Throws_WhenRequestContainsDuplicateSparePartIds()
        {
            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts = new List<AddSparePartToJobCardDto>
                {
                    new AddSparePartToJobCardDto { SparePartId = 5, Quantity = 1 },
                    new AddSparePartToJobCardDto { SparePartId = 5, Quantity = 2 }
                }
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.AddSparePartsAsync(1, dto, CancellationToken.None));
        }

        [TestMethod]
        public async Task AddSparePartsAsync_Throws_WhenJobCardIsNotWaitingCustomerApproval()
        {
            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts = new List<AddSparePartToJobCardDto>
                {
                    new AddSparePartToJobCardDto { SparePartId = 6, Quantity = 1 }
                }
            };

            _jobCardRepo
                .Setup(x => x.GetByIdAsync(3))
                .ReturnsAsync(new JobCardEntity
                {
                    JobCardId = 3,
                    Status = JobCardStatus.WaitingSupervisorApproval
                });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.AddSparePartsAsync(3, dto, CancellationToken.None));
        }

        [TestMethod]
        public async Task AddSparePartsAsync_Throws_WhenStockIsInsufficient()
        {
            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts = new List<AddSparePartToJobCardDto>
                {
                    new AddSparePartToJobCardDto { SparePartId = 7, Quantity = 5 }
                }
            };

            _jobCardRepo
                .Setup(x => x.GetByIdAsync(4))
                .ReturnsAsync(new JobCardEntity
                {
                    JobCardId = 4,
                    Status = JobCardStatus.WaitingCustomerApproval
                });

            _jobCardSparePartRepo
                .Setup(x => x.GetByIdAsync(4, 7, It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobCardSparePart?)null);

            _inventoryRepo
                .Setup(x => x.GetByIdAsync(7))
                .ReturnsAsync(new Inventory
                {
                    SparePartId = 7,
                    Quantity = 1,
                    SellingPrice = 50,
                    IsActive = true
                });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.AddSparePartsAsync(4, dto, CancellationToken.None));
        }

        [TestMethod]
        public async Task AddSparePartsAsync_AddsEntitiesAndCreatesStockTransactions_WhenRequestIsValid()
        {
            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts = new List<AddSparePartToJobCardDto>
                {
                    new AddSparePartToJobCardDto
                    {
                        SparePartId = 10,
                        Quantity = 2,
                        IsUnderWarranty = true,
                        Note = "install both"
                    },
                    new AddSparePartToJobCardDto
                    {
                        SparePartId = 11,
                        Quantity = 1,
                        IsUnderWarranty = false
                    }
                }
            };

            var addedEntities = new List<JobCardSparePart>();
            var stockRequests = new List<StockTransactionCreateRequest>();

            _jobCardRepo
                .Setup(x => x.GetByIdAsync(5))
                .ReturnsAsync(new JobCardEntity
                {
                    JobCardId = 5,
                    Status = JobCardStatus.WaitingCustomerApproval
                });

            _jobCardSparePartRepo
                .Setup(x => x.GetByIdAsync(5, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobCardSparePart?)null);

            _inventoryRepo
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(new Inventory
                {
                    SparePartId = 10,
                    Quantity = 10,
                    SellingPrice = 200,
                    IsActive = true
                });

            _inventoryRepo
                .Setup(x => x.GetByIdAsync(11))
                .ReturnsAsync(new Inventory
                {
                    SparePartId = 11,
                    Quantity = 5,
                    SellingPrice = 75,
                    IsActive = true
                });

            _jobCardSparePartRepo
                .Setup(x => x.AddAsync(It.IsAny<JobCardSparePart>(), It.IsAny<CancellationToken>()))
                .Callback<JobCardSparePart, CancellationToken>((entity, _) => addedEntities.Add(entity))
                .Returns(Task.CompletedTask);

            _stockTransactionService
                .Setup(x => x.CreateAsync(It.IsAny<StockTransactionCreateRequest>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransactionCreateRequest, CancellationToken>((request, _) => stockRequests.Add(request))
                .ReturnsAsync(new StockTransactionResponse());

            var result = await _service.AddSparePartsAsync(5, dto, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, addedEntities.Count);
            Assert.AreEqual(400, addedEntities.Single(x => x.SparePartId == 10).TotalAmount);
            Assert.AreEqual(75, addedEntities.Single(x => x.SparePartId == 11).TotalAmount);
            Assert.AreEqual(TransactionType.ExportToJobCard, stockRequests[0].TransactionType);
            Assert.AreEqual(5, stockRequests[0].JobCardId);
            _jobCardSparePartRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task RemoveSparePartAsync_ReturnsFalse_WhenSparePartDoesNotExistInJobCard()
        {
            _jobCardSparePartRepo
                .Setup(x => x.GetByIdAsync(9, 99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobCardSparePart?)null);

            var result = await _service.RemoveSparePartAsync(9, 99, CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task RemoveSparePartAsync_DeletesEntityAndCreatesReturnTransaction()
        {
            var entity = new JobCardSparePart
            {
                JobCardId = 7,
                SparePartId = 13,
                Quantity = 3,
                UnitPrice = 90
            };

            StockTransactionCreateRequest? capturedRequest = null;

            _jobCardSparePartRepo
                .Setup(x => x.GetByIdAsync(7, 13, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            _jobCardRepo
                .Setup(x => x.GetByIdAsync(7))
                .ReturnsAsync(new JobCardEntity { JobCardId = 7 });

            _inventoryRepo
                .Setup(x => x.GetByIdAsync(13))
                .ReturnsAsync(new Inventory { SparePartId = 13, Quantity = 20, SellingPrice = 90, IsActive = true });

            _stockTransactionService
                .Setup(x => x.CreateAsync(It.IsAny<StockTransactionCreateRequest>(), It.IsAny<CancellationToken>()))
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
