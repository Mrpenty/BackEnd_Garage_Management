using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.RepairEstimaties;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Enums;
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
    public class RepairEstimateServiceGetTests
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
        public async Task GetAllAsync_WhenDataExists_ReturnsMappedList()
        {
            _repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                [
                    new RepairEstimate
                    {
                        RepairEstimateId = 1,
                        JobCardId = 10,
                        Status = RepairEstimateApprovalStatus.WaitingApproval,
                        ServiceTotal = 100,
                        SparePartTotal = 50,
                        GrandTotal = 150,
                        Note = "Estimate 1"
                    },
                    new RepairEstimate
                    {
                        RepairEstimateId = 2,
                        JobCardId = 11,
                        Status = RepairEstimateApprovalStatus.Approve,
                        ServiceTotal = 200,
                        SparePartTotal = 70,
                        GrandTotal = 270,
                        Note = "Estimate 2"
                    }
                ]);

            var result = await _service.GetAllAsync(CancellationToken.None);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].RepairEstimateId);
            Assert.AreEqual(270m, result[1].GrandTotal);
            Assert.AreEqual(RepairEstimateApprovalStatus.Approve, result[1].Status);
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenEntityExists_ReturnsDetailResponse()
        {
            _repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(BuildEstimate());

            var result = await _service.GetByIdAsync(1, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RepairEstimateId);
            Assert.AreEqual(2, result.Services.Count);
            Assert.AreEqual(1, result.SpareParts.Count);
            Assert.AreEqual("Oil change", result.Services[0].ServiceName);
            Assert.AreEqual("Brake Pad", result.SpareParts[0].SparePartName);
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenEntityDoesNotExist_ReturnsNull()
        {
            _repo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RepairEstimate?)null);

            var result = await _service.GetByIdAsync(1, CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_WithInvalidRepairEstimateId_Throws()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.GetByIdAsync(0, CancellationToken.None));
        }

        [TestMethod]
        public async Task GetByJobCardIdAsync_WhenJobCardExists_ReturnsMappedList()
        {
            _jobCardRepo.Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(new JobCardEntity { JobCardId = 10 });
            _repo.Setup(x => x.GetByJobCardIdAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync([BuildEstimate()]);

            var result = await _service.GetByJobCardIdAsync(10, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(10, result[0].JobCardId);
            Assert.AreEqual(250m, result[0].GrandTotal);
        }

        [TestMethod]
        public async Task GetByJobCardIdAsync_WhenJobCardDoesNotExist_ReturnsNull()
        {
            _jobCardRepo.Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync((JobCardEntity?)null);

            var result = await _service.GetByJobCardIdAsync(10, CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByJobCardIdAsync_WithInvalidJobCardId_Throws()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.GetByJobCardIdAsync(0, CancellationToken.None));
        }

        private static RepairEstimate BuildEstimate()
        {
            return new RepairEstimate
            {
                RepairEstimateId = 1,
                JobCardId = 10,
                Status = RepairEstimateApprovalStatus.WaitingApproval,
                ServiceTotal = 200,
                SparePartTotal = 50,
                GrandTotal = 250,
                Note = "Need confirmation",
                Services =
                [
                    new RepairEstimateService
                    {
                        RepairEstimateId = 1,
                        ServiceId = 2,
                        Status = RepairEstimateApprovalStatus.WaitingApproval,
                        UnitPrice = 100,
                        Quantity = 1,
                        TotalAmount = 100,
                        Service = new Service { ServiceId = 2, ServiceName = "Oil change" }
                    },
                    new RepairEstimateService
                    {
                        RepairEstimateId = 1,
                        ServiceId = 3,
                        Status = RepairEstimateApprovalStatus.Approve,
                        UnitPrice = 100,
                        Quantity = 1,
                        TotalAmount = 100,
                        Service = new Service { ServiceId = 3, ServiceName = "Inspection" }
                    }
                ],
                SpareParts =
                [
                    new RepairEstimateSparePart
                    {
                        RepairEstimateId = 1,
                        SparePartId = 5,
                        Status = RepairEstimateApprovalStatus.WaitingApproval,
                        UnitPrice = 50,
                        Quantity = 1,
                        TotalAmount = 50,
                        Inventory = new Inventory { SparePartId = 5, PartName = "Brake Pad" }
                    }
                ]
            };
        }
    }
}
