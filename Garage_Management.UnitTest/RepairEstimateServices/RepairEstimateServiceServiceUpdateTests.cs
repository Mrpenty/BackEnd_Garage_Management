using Garage_Management.Application.DTOs.RepairEstimateServices;
using Garage_Management.Application.Interfaces.Repositories.RepairEstimaties;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Entities.RepairEstimaties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepairEstimateServiceApp = Garage_Management.Application.Services.RepairEstimaties.RepairEstimateServiceService;
using RepairEstimateServiceEntity = Garage_Management.Base.Entities.RepairEstimaties.RepairEstimateService;

namespace Garage_Management.UnitTest.RepairEstimateServices
{
    [TestClass]
    public class RepairEstimateServiceServiceUpdateTests
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
        public async Task UpdateAsync_WhenEntityExists_UpdatesAndReturnsResponse()
        {
            var entity = new RepairEstimateServiceEntity
            {
                RepairEstimateId = 1,
                ServiceId = 2,
                UnitPrice = 100,
                Quantity = 1,
                TotalAmount = 100
            };

            var request = new RepairEstimateServiceUpdateRequest
            {
                UnitPrice = 200,
                Quantity = 3,
                TotalAmount = 600
            };

            _repo.Setup(x => x.GetByIdAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var result = await _service.UpdateAsync(1, 2, request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(200m, entity.UnitPrice);
            Assert.AreEqual(3, entity.Quantity);
            Assert.AreEqual(600m, entity.TotalAmount);
            _repo.Verify(x => x.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenTotalAmountMissing_UsesCalculatedValue()
        {
            var entity = new RepairEstimateServiceEntity
            {
                RepairEstimateId = 1,
                ServiceId = 2,
                UnitPrice = 100,
                Quantity = 2,
                TotalAmount = 200
            };

            var request = new RepairEstimateServiceUpdateRequest
            {
                UnitPrice = 150,
                Quantity = 3
            };

            _repo.Setup(x => x.GetByIdAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var result = await _service.UpdateAsync(1, 2, request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(450m, result.TotalAmount);
            Assert.AreEqual(450m, entity.TotalAmount);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenEntityDoesNotExist_ReturnsNull()
        {
            _repo.Setup(x => x.GetByIdAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RepairEstimateServiceEntity?)null);

            var result = await _service.UpdateAsync(1, 2, new RepairEstimateServiceUpdateRequest(), CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenTotalAmountDoesNotMatch_Throws()
        {
            var entity = new RepairEstimateServiceEntity
            {
                RepairEstimateId = 1,
                ServiceId = 2,
                UnitPrice = 100,
                Quantity = 1,
                TotalAmount = 100
            };

            _repo.Setup(x => x.GetByIdAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateAsync(
                    1,
                    2,
                    new RepairEstimateServiceUpdateRequest
                    {
                        UnitPrice = 150,
                        Quantity = 2,
                        TotalAmount = 100
                    },
                    CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateAsync_WhenQuantityIsInvalid_Throws()
        {
            var entity = new RepairEstimateServiceEntity
            {
                RepairEstimateId = 1,
                ServiceId = 2,
                UnitPrice = 100,
                Quantity = 1,
                TotalAmount = 100
            };

            _repo.Setup(x => x.GetByIdAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateAsync(
                    1,
                    2,
                    new RepairEstimateServiceUpdateRequest { Quantity = 0 },
                    CancellationToken.None));
        }
    }
}
