using Garage_Management.Application.Interfaces.Repositories.RepairEstimaties;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.RepairEstimaties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepairEstimateServiceApp = Garage_Management.Application.Services.RepairEstimaties.RepairEstimateServiceService;

namespace Garage_Management.UnitTest.RepairEstimateServices
{
    [TestClass]
    public class RepairEstimateServiceServiceGetTests
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
        public async Task GetByIdAsync_WhenEntityExists_ReturnsMappedResponse()
        {
            _repo.Setup(x => x.GetByIdAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RepairEstimateService
                {
                    RepairEstimateId = 1,
                    ServiceId = 2,
                    Status = RepairEstimateApprovalStatus.Approve,
                    UnitPrice = 150,
                    Quantity = 2,
                    TotalAmount = 300
                });

            var result = await _service.GetByIdAsync(1, 2, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.RepairEstimateId);
            Assert.AreEqual(2, result.ServiceId);
            Assert.AreEqual(RepairEstimateApprovalStatus.Approve, result.Status);
            Assert.AreEqual(150m, result.UnitPrice);
            Assert.AreEqual(2, result.Quantity);
            Assert.AreEqual(300m, result.TotalAmount);
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenEntityDoesNotExist_ReturnsNull()
        {
            _repo.Setup(x => x.GetByIdAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RepairEstimateService?)null);

            var result = await _service.GetByIdAsync(1, 2, CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_WithInvalidRepairEstimateId_Throws()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.GetByIdAsync(0, 2, CancellationToken.None));
        }

        [TestMethod]
        public async Task GetPagedAsync_WhenCalled_ReturnsMappedPagedResult()
        {
            _repo.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<RepairEstimateService>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 2,
                    PageData =
                    [
                        new RepairEstimateService
                        {
                            RepairEstimateId = 1,
                            ServiceId = 2,
                            Status = RepairEstimateApprovalStatus.WaitingApproval,
                            UnitPrice = 100,
                            Quantity = 1,
                            TotalAmount = 100
                        },
                        new RepairEstimateService
                        {
                            RepairEstimateId = 1,
                            ServiceId = 3,
                            Status = RepairEstimateApprovalStatus.Reject,
                            UnitPrice = 200,
                            Quantity = 2,
                            TotalAmount = 400
                        }
                    ]
                });

            var result = await _service.GetPagedAsync(1, 10, CancellationToken.None);

            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(10, result.PageSize);
            Assert.AreEqual(2, result.Total);
            Assert.AreEqual(2, result.PageData.Count());
            Assert.AreEqual(3, result.PageData.Last().ServiceId);
            Assert.AreEqual(400m, result.PageData.Last().TotalAmount);
        }

        [TestMethod]
        public async Task GetPagedAsync_WithInvalidPage_Throws()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.GetPagedAsync(0, 10, CancellationToken.None));
        }
    }
}
