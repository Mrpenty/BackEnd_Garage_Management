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
    public class JobCardSparePartServiceUpdateTests
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
        public async Task UpdateAsync_Throws_WhenJobCardIdInvalid()
        {
            var dto = new UpdateJobCardSparePartDto { Quantity = 2 };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.UpdateAsync(0, 1, dto, CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateAsync_Throws_WhenSparePartIdInvalid()
        {
            var dto = new UpdateJobCardSparePartDto { Quantity = 2 };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.UpdateAsync(1, 0, dto, CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsNull_WhenEntityDoesNotExist()
        {
            _jobCardSparePartRepo.Setup(x => x.GetByIdAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobCardSparePartEntity?)null);

            var dto = new UpdateJobCardSparePartDto
            {
                Quantity = 3,
                Note = "test"
            };

            var result = await _service.UpdateAsync(1, 10, dto, CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateAsync_Throws_WhenJobCardDoesNotExist()
        {
            _jobCardSparePartRepo.Setup(x => x.GetByIdAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new JobCardSparePartEntity
                {
                    JobCardId = 1,
                    SparePartId = 10,
                    Quantity = 2,
                    UnitPrice = 100
                });

            _jobCardRepo.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((JobCardEntity?)null);

            var dto = new UpdateJobCardSparePartDto
            {
                Quantity = 3
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.UpdateAsync(1, 10, dto, CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateAsync_Throws_WhenNoFieldsProvided()
        {
            _jobCardSparePartRepo.Setup(x => x.GetByIdAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new JobCardSparePartEntity
                {
                    JobCardId = 1,
                    SparePartId = 10,
                    Quantity = 2,
                    UnitPrice = 100
                });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.UpdateAsync(1, 10, new UpdateJobCardSparePartDto(), CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateAsync_Throws_WhenQuantityInvalid()
        {
            _jobCardSparePartRepo.Setup(x => x.GetByIdAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new JobCardSparePartEntity
                {
                    JobCardId = 1,
                    SparePartId = 10,
                    Quantity = 2,
                    UnitPrice = 100
                });

            var dto = new UpdateJobCardSparePartDto
            {
                Quantity = 0
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.UpdateAsync(1, 10, dto, CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateAsync_IncreasesQuantity_AndCreatesExportTransaction()
        {
            var entity = new JobCardSparePartEntity
            {
                JobCardId = 1,
                SparePartId = 10,
                Quantity = 2,
                UnitPrice = 100,
                TotalAmount = 200,
                IsUnderWarranty = false,
                Note = "old note"
            };

            StockTransactionCreateRequest? capturedRequest = null;

            _jobCardSparePartRepo.Setup(x => x.GetByIdAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);
            _jobCardRepo.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new JobCardEntity { JobCardId = 1 });
            _stockTransactionService.Setup(x => x.CreateAsync(It.IsAny<StockTransactionCreateRequest>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransactionCreateRequest, CancellationToken>((request, _) => capturedRequest = request)
                .ReturnsAsync(new StockTransactionResponse());
            _jobCardSparePartRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(1, 10, new UpdateJobCardSparePartDto
            {
                Quantity = 5
            }, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result!.Quantity);
            Assert.AreEqual(500m, result.TotalAmount);
            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(TransactionType.ExportToJobCard, capturedRequest!.TransactionType);
            Assert.AreEqual(3, capturedRequest.QuantityChange);
            Assert.AreEqual(1, capturedRequest.JobCardId);
            _jobCardSparePartRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
            _stockTransactionService.Verify(x => x.CreateAsync(It.IsAny<StockTransactionCreateRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateAsync_DecreasesQuantity_AndCreatesReturnTransaction()
        {
            var entity = new JobCardSparePartEntity
            {
                JobCardId = 2,
                SparePartId = 20,
                Quantity = 5,
                UnitPrice = 80,
                TotalAmount = 400,
                IsUnderWarranty = true,
                Note = "note"
            };

            StockTransactionCreateRequest? capturedRequest = null;

            _jobCardSparePartRepo.Setup(x => x.GetByIdAsync(2, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);
            _jobCardRepo.Setup(x => x.GetByIdAsync(2))
                .ReturnsAsync(new JobCardEntity { JobCardId = 2 });
            _stockTransactionService.Setup(x => x.CreateAsync(It.IsAny<StockTransactionCreateRequest>(), It.IsAny<CancellationToken>()))
                .Callback<StockTransactionCreateRequest, CancellationToken>((request, _) => capturedRequest = request)
                .ReturnsAsync(new StockTransactionResponse());
            _jobCardSparePartRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(2, 20, new UpdateJobCardSparePartDto
            {
                Quantity = 2
            }, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result!.Quantity);
            Assert.AreEqual(160m, result.TotalAmount);
            Assert.IsNotNull(capturedRequest);
            Assert.AreEqual(TransactionType.ReturnFromJobCard, capturedRequest!.TransactionType);
            Assert.AreEqual(3, capturedRequest.QuantityChange);
            Assert.AreEqual(2, capturedRequest.JobCardId);
            _jobCardSparePartRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
            _stockTransactionService.Verify(x => x.CreateAsync(It.IsAny<StockTransactionCreateRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateAsync_UpdatesNoteAndWarranty_WithoutStockTransaction()
        {
            var entity = new JobCardSparePartEntity
            {
                JobCardId = 3,
                SparePartId = 30,
                Quantity = 4,
                UnitPrice = 150,
                TotalAmount = 600,
                IsUnderWarranty = false,
                Note = "old"
            };

            _jobCardSparePartRepo.Setup(x => x.GetByIdAsync(3, 30, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);
            _jobCardRepo.Setup(x => x.GetByIdAsync(3))
                .ReturnsAsync(new JobCardEntity { JobCardId = 3 });
            _jobCardSparePartRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(3, 30, new UpdateJobCardSparePartDto
            {
                IsUnderWarranty = true,
                Note = "updated note"
            }, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(4, result!.Quantity);
            Assert.AreEqual(true, result.IsUnderWarranty);
            Assert.AreEqual("updated note", result.Note);
            Assert.AreEqual(600m, result.TotalAmount);
            _stockTransactionService.Verify(x => x.CreateAsync(It.IsAny<StockTransactionCreateRequest>(), It.IsAny<CancellationToken>()), Times.Never);
            _jobCardSparePartRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
