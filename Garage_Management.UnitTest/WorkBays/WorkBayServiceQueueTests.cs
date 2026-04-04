using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Services.Workbays;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.Vehiclies;
using Moq;
using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;

namespace Garage_Management.UnitTest.WorkBays
{
    [TestClass]
    public class WorkBayServiceQueueTests
    {
        private Mock<IWorkBayRepository> _workBayRepo = null!;
        private Mock<IJobCardRepository> _jobCardRepo = null!;
        private WorkBayService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _workBayRepo = new Mock<IWorkBayRepository>();
            _jobCardRepo = new Mock<IJobCardRepository>();
            _service = new WorkBayService(_workBayRepo.Object, _jobCardRepo.Object);
        }

        [TestMethod]
        public async Task GetByIdAsync_SortsJobCardsByQueueOrder()
        {
            _workBayRepo.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new WorkBay { Id = 1, Name = "Bay 1" });

            _jobCardRepo.Setup(x => x.GetByWorkBayIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                [
                    BuildJobCard(2, 3000m),
                    BuildJobCard(1, 1000m),
                    BuildJobCard(3, 2000m)
                ]);

            var result = await _service.GetByIdAsync(1, CancellationToken.None);

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(
                new[] { 1, 3, 2 },
                result.JobCards.Select(x => x.JobCardId).ToArray());
        }

        [TestMethod]
        public async Task RebalanceQueueAsync_ReassignsQueueOrderByWorkBay()
        {
            var jobs = new List<JobCardEntity>
            {
                BuildJobCard(5, 50m),
                BuildJobCard(6, 51m),
                BuildJobCard(7, 52m)
            };

            _workBayRepo.Setup(x => x.GetByIdAsync(2))
                .ReturnsAsync(new WorkBay { Id = 2, Name = "Bay 2" });

            _jobCardRepo.Setup(x => x.GetTrackedByWorkBayIdAsync(2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobs);

            _jobCardRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(3);

            var result = await _service.RebalanceQueueAsync(2, CancellationToken.None);

            Assert.IsTrue(result.Success);
            CollectionAssert.AreEqual(
                new decimal[] { 1000m, 2000m, 3000m },
                jobs.Select(x => x.QueueOrder).ToArray());
        }

        private static JobCardEntity BuildJobCard(int id, decimal queueOrder)
        {
            return new JobCardEntity
            {
                JobCardId = id,
                QueueOrder = queueOrder,
                CustomerId = id,
                Customer = new Customer { FirstName = $"Cus{id}", LastName = "Test" },
                VehicleId = id,
                Vehicle = new Vehicle
                {
                    VehicleId = id,
                    LicensePlate = $"LP-{id}",
                    Brand = new VehicleBrand { BrandName = "Brand" },
                    Model = new VehicleModel { ModelName = "Model" }
                },
                Mechanics = new List<JobCardMechanic>(),
                Services = new List<JobCardService>()
            };
        }
    }
}
