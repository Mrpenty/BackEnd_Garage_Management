using Garage_Management.Application.DTOs.RepairEstimates;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.RepairEstimaties;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.RepairEstimaties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepairEstimateServiceApp = Garage_Management.Application.Services.RepairEstimaties.RepairEstimateService;

namespace Garage_Management.UnitTest.RepairEstimates
{
    [TestClass]
    public class RepairEstimateServiceStatusTests
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
        public async Task UpdateStatusAsync_WhenEntityExists_UpdatesAndReturnsDetail()
        {
            var entity = new RepairEstimate
            {
                RepairEstimateId = 1,
                JobCardId = 10,
                Status = RepairEstimateApprovalStatus.WaitingApproval,
                ServiceTotal = 100,
                SparePartTotal = 50,
                GrandTotal = 150
            };

            _repo.Setup(x => x.GetTrackedByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var before = DateTime.UtcNow.AddSeconds(-1);
            var result = await _service.UpdateStatusAsync(
                1,
                new RepairEstimateStatusUpdateRequest { Status = RepairEstimateApprovalStatus.Approve },
                CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(RepairEstimateApprovalStatus.Approve, entity.Status);
            Assert.AreEqual(RepairEstimateApprovalStatus.Approve, result.Status);
            Assert.IsTrue(entity.UpdatedAt >= before);
            _repo.Verify(x => x.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_WhenEntityDoesNotExist_ReturnsNull()
        {
            _repo.Setup(x => x.GetTrackedByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RepairEstimate?)null);

            var result = await _service.UpdateStatusAsync(
                1,
                new RepairEstimateStatusUpdateRequest { Status = RepairEstimateApprovalStatus.Reject },
                CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_WithInvalidRepairEstimateId_Throws()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateStatusAsync(
                    0,
                    new RepairEstimateStatusUpdateRequest { Status = RepairEstimateApprovalStatus.Approve },
                    CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateStatusAsync_WithInvalidStatus_Throws()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateStatusAsync(
                    1,
                    new RepairEstimateStatusUpdateRequest { Status = (RepairEstimateApprovalStatus)999 },
                    CancellationToken.None));
        }
    }
}
