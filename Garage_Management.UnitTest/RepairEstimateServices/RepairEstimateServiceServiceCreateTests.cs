using Garage_Management.Application.DTOs.RepairEstimateServices;
using Garage_Management.Application.Interfaces.Repositories.RepairEstimaties;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.RepairEstimaties;
using Garage_Management.Base.Entities.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepairEstimateServiceApp = Garage_Management.Application.Services.RepairEstimaties.RepairEstimateServiceService;
using RepairEstimateServiceEntity = Garage_Management.Base.Entities.RepairEstimaties.RepairEstimateService;

namespace Garage_Management.UnitTest.RepairEstimateServices
{
    [TestClass]
    public class RepairEstimateServiceServiceCreateTests
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
        public async Task CreateAsync_WithValidRequest_ReturnsCreatedResponse()
        {
            var request = new RepairEstimateServiceCreateRequest
            {
                RepairEstimateId = 1,
                ServiceId = 2,
                Quantity = 2
            };

            RepairEstimateServiceEntity? captured = null;

            _repairEstimateRepo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RepairEstimate { RepairEstimateId = 1 });
            _serviceRepo.Setup(x => x.GetByIdAsync(2))
                .ReturnsAsync(new Service { ServiceId = 2, IsActive = true, BasePrice = 250 });
            _repo.Setup(x => x.GetByIdAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RepairEstimateServiceEntity?)null);
            _repo.Setup(x => x.AddAsync(It.IsAny<RepairEstimateServiceEntity>(), It.IsAny<CancellationToken>()))
                .Callback<RepairEstimateServiceEntity, CancellationToken>((entity, _) => captured = entity)
                .Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsNotNull(captured);
            Assert.AreEqual(RepairEstimateApprovalStatus.WaitingApproval, captured.Status);
            Assert.AreEqual(250m, captured.UnitPrice);
            Assert.AreEqual(500m, captured.TotalAmount);
            Assert.AreEqual(1, result.RepairEstimateId);
            Assert.AreEqual(2, result.ServiceId);
            Assert.AreEqual(250m, result.UnitPrice);
            Assert.AreEqual(2, result.Quantity);
            Assert.AreEqual(500m, result.TotalAmount);
        }

        [TestMethod]
        public async Task CreateAsync_WhenRepairEstimateDoesNotExist_Throws()
        {
            var request = new RepairEstimateServiceCreateRequest
            {
                RepairEstimateId = 1,
                ServiceId = 2,
                Quantity = 1
            };

            _repairEstimateRepo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RepairEstimate?)null);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_WhenServiceIsInactive_Throws()
        {
            var request = new RepairEstimateServiceCreateRequest
            {
                RepairEstimateId = 1,
                ServiceId = 2,
                Quantity = 1
            };

            _repairEstimateRepo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RepairEstimate { RepairEstimateId = 1 });
            _serviceRepo.Setup(x => x.GetByIdAsync(2))
                .ReturnsAsync(new Service { ServiceId = 2, IsActive = false, BasePrice = 100 });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_WhenEntityAlreadyExists_Throws()
        {
            var request = new RepairEstimateServiceCreateRequest
            {
                RepairEstimateId = 1,
                ServiceId = 2,
                Quantity = 1
            };

            _repairEstimateRepo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RepairEstimate { RepairEstimateId = 1 });
            _serviceRepo.Setup(x => x.GetByIdAsync(2))
                .ReturnsAsync(new Service { ServiceId = 2, IsActive = true, BasePrice = 100 });
            _repo.Setup(x => x.GetByIdAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RepairEstimateServiceEntity
                {
                    RepairEstimateId = 1,
                    ServiceId = 2
                });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_WhenServiceBasePriceMissing_Throws()
        {
            var request = new RepairEstimateServiceCreateRequest
            {
                RepairEstimateId = 1,
                ServiceId = 2,
                Quantity = 2
            };

            _repairEstimateRepo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RepairEstimate { RepairEstimateId = 1 });
            _serviceRepo.Setup(x => x.GetByIdAsync(2))
                .ReturnsAsync(new Service { ServiceId = 2, IsActive = true, BasePrice = null });

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }
    }
}
