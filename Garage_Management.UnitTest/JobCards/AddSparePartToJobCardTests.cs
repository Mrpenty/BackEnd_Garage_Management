using Garage_Management.Application.DTOs.Iventories.StockTransactions;
using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;

namespace Garage_Management.UnitTest.JobCards
{
    [TestClass]
    public class AddSparePartToJobCardTests
    {
        private Mock<IJobCardRepository> _jobCardRepo = null!;
        private Mock<IInventoryRepository> _inventoryRepo = null!;
        private Mock<IJobCardSparePartRepository> _jobCardSparePartRepo = null!;
        private Mock<IStockTransactionService> _stockTransactionService = null!;
        private JobCardSparePartService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _jobCardRepo = new Mock<IJobCardRepository>();
            _inventoryRepo = new Mock<IInventoryRepository>();
            _jobCardSparePartRepo = new Mock<IJobCardSparePartRepository>();
            _stockTransactionService = new Mock<IStockTransactionService>();

            _service = new JobCardSparePartService(
                _jobCardRepo.Object,
                _jobCardSparePartRepo.Object,
                _inventoryRepo.Object,
                _stockTransactionService.Object);
        }

        [TestMethod]
        public async Task AddSparePartsAsync_ReturnsNull_WhenJobCardNotFound()
        {
            _jobCardRepo
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((JobCardEntity?)null);

            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts =
                {
                    new AddSparePartToJobCardDto { SparePartId = 1, Quantity = 1 }
                }
            };

            var result = await _service.AddSparePartsAsync(999, dto, CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AddSparePartsAsync_Throws_WhenInventoryNotFound()
        {
            _jobCardRepo
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new JobCardEntity
                {
                    JobCardId = 1,
                    Status = JobCardStatus.WaitingCustomerApproval
                });

            _jobCardSparePartRepo
                .Setup(x => x.GetByIdAsync(1, 999, It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobCardSparePart?)null);

            _inventoryRepo
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Inventory?)null);

            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts =
                {
                    new AddSparePartToJobCardDto { SparePartId = 999, Quantity = 1 }
                }
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.AddSparePartsAsync(1, dto, CancellationToken.None));
        }

        [TestMethod]
        public async Task AddSparePartsAsync_Throws_WhenQuantityNonPositive()
        {
            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts =
                {
                    new AddSparePartToJobCardDto { SparePartId = 5, Quantity = 0 }
                }
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.AddSparePartsAsync(1, dto, CancellationToken.None));
        }

        [TestMethod]
        public async Task AddSparePartsAsync_ReturnsResponse_WhenValid()
        {
            var jobCard = new JobCardEntity
            {
                JobCardId = 7,
                Status = JobCardStatus.WaitingCustomerApproval
            };

            var inventory = new Inventory
            {
                SparePartId = 7,
                Quantity = 10,
                SellingPrice = 20m,
                IsActive = true
            };

            _jobCardRepo
                .Setup(x => x.GetByIdAsync(7))
                .ReturnsAsync(jobCard);

            _jobCardSparePartRepo
                .Setup(x => x.GetByIdAsync(7, 7, It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobCardSparePart?)null);

            _inventoryRepo
                .Setup(x => x.GetByIdAsync(7))
                .ReturnsAsync(inventory);

            _stockTransactionService
                .Setup(x => x.CreateAsync(It.IsAny<StockTransactionCreateRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new StockTransactionResponse());

            _jobCardSparePartRepo
                .Setup(x => x.AddAsync(It.IsAny<JobCardSparePart>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _jobCardSparePartRepo
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var dto = new AddMultipleSparePartsToJobCardDto
            {
                SpareParts =
                {
                    new AddSparePartToJobCardDto
                    {
                        SparePartId = 7,
                        Quantity = 2,
                        IsUnderWarranty = true,
                        Note = "ok"
                    }
                }
            };

            var result = await _service.AddSparePartsAsync(7, dto, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(7, result[0].JobCardId);
            Assert.AreEqual(7, result[0].SparePartId);
            Assert.AreEqual(40m, result[0].TotalAmount);

            _jobCardSparePartRepo.Verify(
                x => x.AddAsync(
                    It.Is<JobCardSparePart>(j =>
                        j.JobCardId == 7 &&
                        j.SparePartId == 7 &&
                        j.Quantity == 2 &&
                        j.TotalAmount == 40m),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
