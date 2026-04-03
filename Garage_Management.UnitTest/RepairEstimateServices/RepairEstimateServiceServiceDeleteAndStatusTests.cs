using Garage_Management.Application.DTOs.RepairEstimateServices;
using Garage_Management.Application.Interfaces.Repositories.RepairEstimaties;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepairEstimateServiceApp = Garage_Management.Application.Services.RepairEstimaties.RepairEstimateServiceService;
using RepairEstimateServiceEntity = Garage_Management.Base.Entities.RepairEstimaties.RepairEstimateService;

namespace Garage_Management.UnitTest.RepairEstimateServices
{
    [TestClass]
    public class RepairEstimateServiceServiceDeleteAndStatusTests
    {
        private Mock<IRepairEstimateServiceRepository> _repo = null!;
        private Mock<IRepairEstimateRepository> _repairEstimateRepo = null!;
        private Mock<IServiceRepository> _serviceRepo = null!;
        private RepairEstimateServiceApp _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IRepairEstimateServiceRepository>();
            _repairEstimateRepo = new Mock<IRepairEstimateRepository>();
            _serviceRepo = new Mock<IServiceRepository>();
            _service = new RepairEstimateServiceApp(_repo.Object, _repairEstimateRepo.Object, _serviceRepo.Object);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenEntityExists_ReturnsTrue()
        {
            var entity = new RepairEstimateServiceEntity
            {
                RepairEstimateId = 1,
                ServiceId = 2
            };

            _repo.Setup(x => x.GetByIdAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var result = await _service.DeleteAsync(1, 2, CancellationToken.None);

            Assert.IsTrue(result);
            _repo.Verify(x => x.DeleteAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenEntityDoesNotExist_ReturnsFalse()
        {
            _repo.Setup(x => x.GetByIdAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RepairEstimateServiceEntity?)null);

            var result = await _service.DeleteAsync(1, 2, CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_WhenEntityExists_ReturnsUpdatedResponse()
        {
            var entity = new RepairEstimateServiceEntity
            {
                RepairEstimateId = 1,
                ServiceId = 2,
                Status = RepairEstimateApprovalStatus.WaitingApproval,
                UnitPrice = 100,
                Quantity = 1,
                TotalAmount = 100
            };

            _repo.Setup(x => x.GetTrackedByIdAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var result = await _service.UpdateStatusAsync(
                1,
                2,
                new RepairEstimateServiceStatusUpdateRequest
                {
                    Status = RepairEstimateApprovalStatus.Approve
                },
                CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(RepairEstimateApprovalStatus.Approve, entity.Status);
            Assert.AreEqual(RepairEstimateApprovalStatus.Approve, result.Status);
            _repo.Verify(x => x.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_WhenEntityDoesNotExist_ReturnsNull()
        {
            _repo.Setup(x => x.GetTrackedByIdAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RepairEstimateServiceEntity?)null);

            var result = await _service.UpdateStatusAsync(
                1,
                2,
                new RepairEstimateServiceStatusUpdateRequest
                {
                    Status = RepairEstimateApprovalStatus.Reject
                },
                CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_WithInvalidStatus_Throws()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateStatusAsync(
                    1,
                    2,
                    new RepairEstimateServiceStatusUpdateRequest
                    {
                        Status = (RepairEstimateApprovalStatus)999
                    },
                    CancellationToken.None));
        }
    }
}
