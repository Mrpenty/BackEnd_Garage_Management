using Garage_Management.Application.DTOs.RepairEstimateSpareParts;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.RepairEstimaties;
using Garage_Management.Application.Services.RepairEstimaties;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Entities.RepairEstimaties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Garage_Management.UnitTest.RepairEstimateSpareParts
{
    [TestClass]
    public class RepairEstimateSparePartServiceCreateTests
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
        public async Task CreateAsync_Throws_WhenRequestIsNull()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => _service.CreateAsync(null!, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenRepairEstimateIdIsInvalid()
        {
            var request = new RepairEstimateSparePartCreateRequest
            {
                RepairEstimateId = 0,
                SparePartId = 5,
                Quantity = 1
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));

            Assert.AreEqual("RepairEstimateId phải lớn hơn 0", ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenSparePartIdIsInvalid()
        {
            var request = new RepairEstimateSparePartCreateRequest
            {
                RepairEstimateId = 10,
                SparePartId = 0,
                Quantity = 1
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));

            Assert.AreEqual("SparePartId phải lớn hơn 0", ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenQuantityIsInvalid()
        {
            var request = new RepairEstimateSparePartCreateRequest
            {
                RepairEstimateId = 10,
                SparePartId = 5,
                Quantity = 0
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));

            Assert.AreEqual("Số lượng phải lớn hơn 0", ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenRepairEstimateDoesNotExist()
        {
            var request = BuildRequest();

            _repo.Setup(x => x.RepairEstimateExistsAsync(request.RepairEstimateId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));

            Assert.AreEqual("Không tìm thấy báo giá sửa chữa", ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenSparePartDoesNotExist()
        {
            var request = BuildRequest();

            _repo.Setup(x => x.RepairEstimateExistsAsync(request.RepairEstimateId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _inventoryRepository.Setup(x => x.GetByIdAsync(request.SparePartId))
                .ReturnsAsync((Inventory?)null);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));

            Assert.AreEqual("Không tìm thấy phụ tùng", ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenSparePartIsInactive()
        {
            var request = BuildRequest();

            _repo.Setup(x => x.RepairEstimateExistsAsync(request.RepairEstimateId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _inventoryRepository.Setup(x => x.GetByIdAsync(request.SparePartId))
                .ReturnsAsync(new Inventory
                {
                    SparePartId = request.SparePartId,
                    IsActive = false
                });

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));

            Assert.AreEqual($"Phụ tùng {request.SparePartId} đã ngừng hoạt động", ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenRepairEstimateSparePartAlreadyExists()
        {
            var request = BuildRequest();

            _repo.Setup(x => x.RepairEstimateExistsAsync(request.RepairEstimateId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _inventoryRepository.Setup(x => x.GetByIdAsync(request.SparePartId))
                .ReturnsAsync(new Inventory
                {
                    SparePartId = request.SparePartId,
                    IsActive = true,
                    SellingPrice = 15m
                });
            _repo.Setup(x => x.GetByIdAsync(request.RepairEstimateId, request.SparePartId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RepairEstimateSparePart
                {
                    RepairEstimateId = request.RepairEstimateId,
                    SparePartId = request.SparePartId
                });

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));

            Assert.AreEqual("Phụ tùng báo giá sửa chữa này đã tồn tại", ex.Message);
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenSellingPriceIsMissing()
        {
            var request = BuildRequest();

            _repo.Setup(x => x.RepairEstimateExistsAsync(request.RepairEstimateId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _inventoryRepository.Setup(x => x.GetByIdAsync(request.SparePartId))
                .ReturnsAsync(new Inventory
                {
                    SparePartId = request.SparePartId,
                    IsActive = true,
                    SellingPrice = null
                });
            _repo.Setup(x => x.GetByIdAsync(request.RepairEstimateId, request.SparePartId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RepairEstimateSparePart?)null);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));

            Assert.AreEqual("Phụ tùng chưa có giá bán trong tồn kho", ex.Message);
        }

       

        [TestMethod]
        public async Task CreateAsync_ReturnsResponse_WhenRequestIsValid()
        {
            var request = BuildRequest();
            RepairEstimateSparePart? captured = null;

            _repo.Setup(x => x.RepairEstimateExistsAsync(request.RepairEstimateId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _inventoryRepository.Setup(x => x.GetByIdAsync(request.SparePartId))
                .ReturnsAsync(new Inventory
                {
                    SparePartId = request.SparePartId,
                    IsActive = true,
                    SellingPrice = 12.5m
                });
            _repo.Setup(x => x.GetByIdAsync(request.RepairEstimateId, request.SparePartId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RepairEstimateSparePart?)null);
            _repo.Setup(x => x.AddAsync(It.IsAny<RepairEstimateSparePart>(), It.IsAny<CancellationToken>()))
                .Callback<RepairEstimateSparePart, CancellationToken>((entity, _) => captured = entity)
                .Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsNotNull(captured);
            Assert.AreEqual(request.RepairEstimateId, captured.RepairEstimateId);
            Assert.AreEqual(request.SparePartId, captured.SparePartId);
            Assert.AreEqual(RepairEstimateApprovalStatus.WaitingApproval, captured.Status);
            Assert.AreEqual(request.Quantity, captured.Quantity);
            Assert.AreEqual(12.5m, captured.UnitPrice);
            Assert.AreEqual(25m, captured.TotalAmount);

            Assert.AreEqual(request.RepairEstimateId, result.RepairEstimateId);
            Assert.AreEqual(request.SparePartId, result.SparePartId);
            Assert.AreEqual(RepairEstimateApprovalStatus.WaitingApproval, result.Status);
            Assert.AreEqual(request.Quantity, result.Quantity);
            Assert.AreEqual(12.5m, result.UnitPrice);
            Assert.AreEqual(25m, result.TotalAmount);

            _repo.Verify(x => x.AddAsync(It.IsAny<RepairEstimateSparePart>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        private static RepairEstimateSparePartCreateRequest BuildRequest()
        {
            return new RepairEstimateSparePartCreateRequest
            {
                RepairEstimateId = 10,
                SparePartId = 5,
                Quantity = 2
            };
        }
    }
}
