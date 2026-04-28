using Garage_Management.Application.DTOs.Iventories.StockTransactions;
using Garage_Management.Application.DTOs.JobCards;
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
    public class JobCardSparePartServiceAddTests
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
        public async Task AddSparePartsAsync_Throws_WhenJobCardIdInvalid()
        {
            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts = [new AddSparePartToJobCardDto { SparePartId = 1, Quantity = 1 }]
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.AddSparePartsAsync(0, dto, CancellationToken.None));
        }

        [TestMethod]
        public async Task AddSparePartsAsync_ReturnsNull_WhenJobCardNotFound()
        {
            _jobCardRepo.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((JobCardEntity?)null);

            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts = [new AddSparePartToJobCardDto { SparePartId = 1, Quantity = 1 }]
            };

            var result = await _service.AddSparePartsAsync(1, dto, CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AddSparePartsAsync_Throws_WhenDuplicateSparePartIds()
        {
            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts =
                [
                    new AddSparePartToJobCardDto { SparePartId = 5, Quantity = 1 },
                    new AddSparePartToJobCardDto { SparePartId = 5, Quantity = 2 }
                ]
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.AddSparePartsAsync(1, dto, CancellationToken.None));
        }

        [TestMethod]
        public async Task AddSparePartsAsync_Throws_WhenJobCardStatusInvalid()
        {
            _jobCardRepo.Setup(x => x.GetByIdAsync(2))
                .ReturnsAsync(new JobCardEntity
                {
                    JobCardId = 2,
                    Status = JobCardStatus.WaitingSupervisorApproval
                });

            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts = [new AddSparePartToJobCardDto { SparePartId = 1, Quantity = 1 }]
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.AddSparePartsAsync(2, dto, CancellationToken.None));
        }

        [TestMethod]
        public async Task AddSparePartsAsync_Throws_WhenQuantityNonPositive()
        {
            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts = [new AddSparePartToJobCardDto { SparePartId = 1, Quantity = 0 }]
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.AddSparePartsAsync(1, dto, CancellationToken.None));
        }

        [TestMethod]
        public async Task AddSparePartsAsync_Throws_WhenInventoryNotFound()
        {
            _jobCardRepo.Setup(x => x.GetByIdAsync(3))
                .ReturnsAsync(new JobCardEntity
                {
                    JobCardId = 3,
                    Status = JobCardStatus.WaitingCustomerApproval
                });

            _inventoryRepo.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Inventory?)null);

            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts = [new AddSparePartToJobCardDto { SparePartId = 999, Quantity = 1 }]
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.AddSparePartsAsync(3, dto, CancellationToken.None));
        }

        [TestMethod]
        public async Task AddSparePartsAsync_Throws_WhenStockInsufficient()
        {
            _jobCardRepo.Setup(x => x.GetByIdAsync(4))
                .ReturnsAsync(new JobCardEntity
                {
                    JobCardId = 4,
                    Status = JobCardStatus.WaitingCustomerApproval
                });

            _jobCardSparePartRepo.Setup(x => x.GetByIdAsync(4, 7, It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobCardSparePartEntity?)null);

            _inventoryRepo.Setup(x => x.GetByIdAsync(7))
                .ReturnsAsync(new Inventory
                {
                    SparePartId = 7,
                    Quantity = 1,
                    SellingPrice = 50,
                    IsActive = true
                });

            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts = [new AddSparePartToJobCardDto { SparePartId = 7, Quantity = 5 }]
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.AddSparePartsAsync(4, dto, CancellationToken.None));
        }

        [TestMethod]
        public async Task AddSparePartsAsync_ReturnsResponse_WhenValid()
        {
            var addedEntities = new List<JobCardSparePartEntity>();

            _jobCardRepo.Setup(x => x.GetByIdAsync(5))
                .ReturnsAsync(new JobCardEntity
                {
                    JobCardId = 5,
                    Status = JobCardStatus.WaitingCustomerApproval
                });

            _jobCardSparePartRepo.Setup(x => x.GetByIdAsync(5, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobCardSparePartEntity?)null);

            _inventoryRepo.Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(new Inventory { SparePartId = 10, Quantity = 10, SellingPrice = 200, IsActive = true });

            _inventoryRepo.Setup(x => x.GetByIdAsync(11))
                .ReturnsAsync(new Inventory { SparePartId = 11, Quantity = 5, SellingPrice = 75, IsActive = true });

            _jobCardSparePartRepo.Setup(x => x.AddAsync(It.IsAny<JobCardSparePartEntity>(), It.IsAny<CancellationToken>()))
                .Callback<JobCardSparePartEntity, CancellationToken>((e, _) => addedEntities.Add(e))
                .Returns(Task.CompletedTask);

            _stockTransactionService.Setup(x => x.CreateAsync(It.IsAny<StockTransactionCreateRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StockTransactionResponse());

            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts =
                [
                    new AddSparePartToJobCardDto { SparePartId = 10, Quantity = 2 },
                    new AddSparePartToJobCardDto { SparePartId = 11, Quantity = 1 }
                ]
            };

            var result = await _service.AddSparePartsAsync(5, dto, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(400m, addedEntities.Single(x => x.SparePartId == 10).TotalAmount);
            Assert.AreEqual(75m, addedEntities.Single(x => x.SparePartId == 11).TotalAmount);

            _jobCardSparePartRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}