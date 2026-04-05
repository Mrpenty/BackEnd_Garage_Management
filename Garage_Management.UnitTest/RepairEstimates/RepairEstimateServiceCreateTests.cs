using Garage_Management.Application.DTOs.RepairEstimates;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.RepairEstimaties;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Entities.RepairEstimaties;
using Garage_Management.Base.Entities.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;
using RepairEstimateServiceApp = Garage_Management.Application.Services.RepairEstimaties.RepairEstimateService;

namespace Garage_Management.UnitTest.RepairEstimates
{
    [TestClass]
    public class RepairEstimateServiceCreateTests
    {
        private Mock<IRepairEstimateRepository> _repo = null!;
        private Mock<IJobCardRepository> _jobCardRepo = null!;
        private Mock<IServiceRepository> _serviceRepo = null!;
        private Mock<IInventoryRepository> _inventoryRepo = null!;
        private RepairEstimateServiceApp _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IRepairEstimateRepository>();
            _jobCardRepo = new Mock<IJobCardRepository>();
            _serviceRepo = new Mock<IServiceRepository>();
            _inventoryRepo = new Mock<IInventoryRepository>();
            _service = new RepairEstimateServiceApp(_repo.Object, _jobCardRepo.Object, _serviceRepo.Object, _inventoryRepo.Object);
        }

        [TestMethod]
        public async Task CreateAsync_WithValidRequest_ReturnsCreatedEstimate()
        {
            var request = new RepairEstimateCreateRequest
            {
                JobCardId = 10,
                Note = "Customer approved inspection",
                Services =
                [
                    new RepairEstimateCreateServiceItemRequest { ServiceId = 2, Quantity = 2 }
                ],
                SpareParts =
                [
                    new RepairEstimateCreateSparePartItemRequest { SparePartId = 5, Quantity = 3 }
                ]
            };

            RepairEstimate? captured = null;

            _jobCardRepo.Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(new JobCardEntity { JobCardId = 10 });
            _serviceRepo.Setup(x => x.GetByIdAsync(2))
                .ReturnsAsync(new Service
                {
                    ServiceId = 2,
                    ServiceName = "Oil change",
                    BasePrice = 150,
                    IsActive = true
                });
            _inventoryRepo.Setup(x => x.GetByIdAsync(5))
                .ReturnsAsync(new Inventory
                {
                    SparePartId = 5,
                    PartName = "Brake Pad",
                    SellingPrice = 80,
                    IsActive = true
                });
            _repo.Setup(x => x.AddAsync(It.IsAny<RepairEstimate>(), It.IsAny<CancellationToken>()))
                .Callback<RepairEstimate, CancellationToken>((entity, _) =>
                {
                    entity.RepairEstimateId = 99;
                    captured = entity;
                })
                .Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsNotNull(captured);
            Assert.AreEqual(10, captured.JobCardId);
            Assert.AreEqual(300m, captured.ServiceTotal);
            Assert.AreEqual(240m, captured.SparePartTotal);
            Assert.AreEqual(540m, captured.GrandTotal);
            Assert.AreEqual(1, captured.Services.Count);
            Assert.AreEqual(1, captured.SpareParts.Count);

            Assert.AreEqual(99, result.RepairEstimateId);
            Assert.AreEqual(540m, result.GrandTotal);
            Assert.AreEqual("Oil change", result.Services[0].ServiceName);
            Assert.AreEqual("Brake Pad", result.SpareParts[0].SparePartName);
        }

        [TestMethod]
        public async Task CreateAsync_WhenJobCardDoesNotExist_Throws()
        {
            var request = new RepairEstimateCreateRequest
            {
                JobCardId = 10,
                Services = [new RepairEstimateCreateServiceItemRequest { ServiceId = 2, Quantity = 1 }]
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((JobCardEntity?)null);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_WhenServiceIsDuplicated_Throws()
        {
            var request = new RepairEstimateCreateRequest
            {
                JobCardId = 10,
                Services =
                [
                    new RepairEstimateCreateServiceItemRequest { ServiceId = 2, Quantity = 1 },
                    new RepairEstimateCreateServiceItemRequest { ServiceId = 2, Quantity = 2 }
                ]
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(new JobCardEntity { JobCardId = 10 });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_WhenSparePartIsDuplicated_Throws()
        {
            var request = new RepairEstimateCreateRequest
            {
                JobCardId = 10,
                SpareParts =
                [
                    new RepairEstimateCreateSparePartItemRequest { SparePartId = 5, Quantity = 1 },
                    new RepairEstimateCreateSparePartItemRequest { SparePartId = 5, Quantity = 2 }
                ]
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(new JobCardEntity { JobCardId = 10 });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_WhenServiceBasePriceMissing_Throws()
        {
            var request = new RepairEstimateCreateRequest
            {
                JobCardId = 10,
                Services = [new RepairEstimateCreateServiceItemRequest { ServiceId = 2, Quantity = 1 }]
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(new JobCardEntity { JobCardId = 10 });
            _serviceRepo.Setup(x => x.GetByIdAsync(2))
                .ReturnsAsync(new Service { ServiceId = 2, IsActive = true, BasePrice = null });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_WhenSparePartSellingPriceMissing_Throws()
        {
            var request = new RepairEstimateCreateRequest
            {
                JobCardId = 10,
                SpareParts = [new RepairEstimateCreateSparePartItemRequest { SparePartId = 5, Quantity = 1 }]
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(new JobCardEntity { JobCardId = 10 });
            _inventoryRepo.Setup(x => x.GetByIdAsync(5))
                .ReturnsAsync(new Inventory { SparePartId = 5, IsActive = true, SellingPrice = null });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_WhenRequestHasNoItems_Throws()
        {
            var request = new RepairEstimateCreateRequest
            {
                JobCardId = 10
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_WhenNoteIsWhitespace_Throws()
        {
            var request = new RepairEstimateCreateRequest
            {
                JobCardId = 10,
                Note = "   ",
                Services = [new RepairEstimateCreateServiceItemRequest { ServiceId = 2, Quantity = 1 }]
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }
    }
}
