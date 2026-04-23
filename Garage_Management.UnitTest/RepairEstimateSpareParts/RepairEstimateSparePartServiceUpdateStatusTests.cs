using Garage_Management.Application.DTOs.RepairEstimateSpareParts;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.RepairEstimaties;
using Garage_Management.Application.Services.RepairEstimaties;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.RepairEstimaties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Garage_Management.UnitTest.RepairEstimateSpareParts
{
    [TestClass]
    public class RepairEstimateSparePartServiceUpdateStatusTests
    {
        private Mock<IRepairEstimateSparePartRepository> _repo = null!;
        private Mock<IInventoryRepository> _inventoryRepository = null!;
        private RepairEstimateSparePartService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IRepairEstimateSparePartRepository>();
            _inventoryRepository = new Mock<IInventoryRepository>();
            _service = new RepairEstimateSparePartService(_repo.Object, _inventoryRepository.Object);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_Throws_WhenRepairEstimateIdIsInvalid()
        {
            var request = new RepairEstimateSparePartStatusUpdateRequest
            {
                Status = RepairEstimateApprovalStatus.Approve
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateStatusAsync(0, 5, request, CancellationToken.None));

            Assert.AreEqual("RepairEstimateId phải lớn hơn 0", ex.Message);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_Throws_WhenSparePartIdIsInvalid()
        {
            var request = new RepairEstimateSparePartStatusUpdateRequest
            {
                Status = RepairEstimateApprovalStatus.Approve
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateStatusAsync(10, 0, request, CancellationToken.None));

            Assert.AreEqual("SparePartId phải lớn hơn 0", ex.Message);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_Throws_WhenRequestIsNull()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _service.UpdateStatusAsync(10, 5, null!, CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateStatusAsync_Throws_WhenStatusIsInvalid()
        {
            var request = new RepairEstimateSparePartStatusUpdateRequest
            {
                Status = (RepairEstimateApprovalStatus)999
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateStatusAsync(10, 5, request, CancellationToken.None));

            Assert.AreEqual("Trạng thái phụ tùng báo giá sửa chữa không hợp lệ", ex.Message);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_ReturnsNull_WhenEntityDoesNotExist()
        {
            var request = new RepairEstimateSparePartStatusUpdateRequest
            {
                Status = RepairEstimateApprovalStatus.Approve
            };

            _repo.Setup(x => x.GetTrackedByIdAsync(10, 5, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RepairEstimateSparePart?)null);

            var result = await _service.UpdateStatusAsync(10, 5, request, CancellationToken.None);

            Assert.IsNull(result);
            _repo.Verify(x => x.UpdateAsync(It.IsAny<RepairEstimateSparePart>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_ReturnsUpdatedResponse_WhenEntityExists()
        {
            var request = new RepairEstimateSparePartStatusUpdateRequest
            {
                Status = RepairEstimateApprovalStatus.Reject
            };

            var entity = new RepairEstimateSparePart
            {
                RepairEstimateId = 10,
                SparePartId = 5,
                Status = RepairEstimateApprovalStatus.WaitingApproval,
                Quantity = 3,
                UnitPrice = 20m,
                TotalAmount = 60m
            };

            _repo.Setup(x => x.GetTrackedByIdAsync(10, 5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);
            _repo.Setup(x => x.UpdateAsync(entity, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _service.UpdateStatusAsync(10, 5, request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(RepairEstimateApprovalStatus.Reject, entity.Status);
            Assert.AreEqual(10, result.RepairEstimateId);
            Assert.AreEqual(5, result.SparePartId);
            Assert.AreEqual(RepairEstimateApprovalStatus.Reject, result.Status);
            Assert.AreEqual(3, result.Quantity);
            Assert.AreEqual(20m, result.UnitPrice);
            Assert.AreEqual(60m, result.TotalAmount);

            _repo.Verify(x => x.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
