using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Common.Format;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;

namespace Garage_Management.UnitTest.JobCards
{
    [TestClass]
    public class ReorderWorkBayQueueTests
    {
        private Mock<IJobCardRepository> _jobCardRepo = null!;
        private Mock<IServiceRepository> _serviceRepo = null!;
        private Mock<IInventoryRepository> _inventoryRepo = null!;
        private Mock<IJobCardServiceRepository> _jobCardServiceRepo = null!;
        private Mock<IJobCardSparePartRepository> _jobCardSparePartRepo = null!;
        private Mock<IWorkBayRepository> _workBayRepo = null!;
        private Mock<IAppointmentRepository> _appointmentRepo = null!;
        private Mock<IHttpContextAccessor> _httpContextAccessor = null!;
        private Mock<ProgressCalculator> _progressCalculator = null!;
        private JobCardService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _jobCardRepo = new Mock<IJobCardRepository>();
            _serviceRepo = new Mock<IServiceRepository>();
            _inventoryRepo = new Mock<IInventoryRepository>();
            _jobCardServiceRepo = new Mock<IJobCardServiceRepository>();
            _jobCardSparePartRepo = new Mock<IJobCardSparePartRepository>();
            _workBayRepo = new Mock<IWorkBayRepository>();
            _appointmentRepo = new Mock<IAppointmentRepository>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _progressCalculator = new Mock<ProgressCalculator>();

            _service = new JobCardService(
                _jobCardRepo.Object,
                _serviceRepo.Object,
                _inventoryRepo.Object,
                _jobCardServiceRepo.Object,
                _jobCardSparePartRepo.Object,
                _workBayRepo.Object,
                _appointmentRepo.Object,
                _httpContextAccessor.Object,
                _progressCalculator.Object);
        }

        [TestMethod]
        public async Task ReorderWorkBayQueueAsync_MovesJobToTop_UsingMinMinus1000()
        {
            var target = BuildJobCard(3, 3000m, 1);
            var jobs = new List<JobCardEntity>
            {
                BuildJobCard(1, 1000m, 1),
                BuildJobCard(2, 2000m, 1),
                target
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(3)).ReturnsAsync(target);
            _jobCardRepo.Setup(x => x.GetTrackedByWorkBayIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobs);
            _jobCardRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _service.ReorderWorkBayQueueAsync(
                new ReorderJobCardQueueDto
                {
                    JobCardId = 3,
                    WorkBayId = 1,
                    NextJobCardId = 1
                },
                CancellationToken.None);

            Assert.IsTrue(result);
            Assert.AreEqual(0m, target.QueueOrder);
            Assert.AreEqual(1000m, jobs[0].QueueOrder);
            Assert.AreEqual(2000m, jobs[1].QueueOrder);
            _jobCardRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task ReorderWorkBayQueueAsync_MovesJobBetweenTwoJobs_UsingAverage()
        {
            var target = BuildJobCard(4, 4000m, 2);
            var previous = BuildJobCard(1, 1000m, 2);
            var next = BuildJobCard(2, 2000m, 2);
            var jobs = new List<JobCardEntity>
            {
                previous,
                next,
                BuildJobCard(3, 3000m, 2),
                target
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(4)).ReturnsAsync(target);
            _jobCardRepo.Setup(x => x.GetTrackedByWorkBayIdAsync(2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobs);
            _jobCardRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _service.ReorderWorkBayQueueAsync(
                new ReorderJobCardQueueDto
                {
                    JobCardId = 4,
                    WorkBayId = 2,
                    PreviousJobCardId = 1,
                    NextJobCardId = 2
                },
                CancellationToken.None);

            Assert.IsTrue(result);
            Assert.AreEqual(1500m, target.QueueOrder);
            Assert.AreEqual(1000m, previous.QueueOrder);
            Assert.AreEqual(2000m, next.QueueOrder);
            _jobCardRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task ReorderWorkBayQueueAsync_MovesJobToBottom_UsingMaxPlus1000()
        {
            var target = BuildJobCard(1, 1000m, 3);
            var jobs = new List<JobCardEntity>
            {
                target,
                BuildJobCard(2, 2000m, 3),
                BuildJobCard(3, 3000m, 3)
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(target);
            _jobCardRepo.Setup(x => x.GetTrackedByWorkBayIdAsync(3, It.IsAny<CancellationToken>()))
                .ReturnsAsync(jobs);
            _jobCardRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _service.ReorderWorkBayQueueAsync(
                new ReorderJobCardQueueDto
                {
                    JobCardId = 1,
                    WorkBayId = 3,
                    PreviousJobCardId = 3
                },
                CancellationToken.None);

            Assert.IsTrue(result);
            Assert.AreEqual(4000m, target.QueueOrder);
            _jobCardRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task ReorderWorkBayQueueAsync_Throws_WhenJobCardNotInWorkBay()
        {
            var target = BuildJobCard(10, 1000m, 3);

            _jobCardRepo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(target);

            var ex = await Assert.ThrowsExceptionAsync<Exception>(() =>
                _service.ReorderWorkBayQueueAsync(
                    new ReorderJobCardQueueDto
                    {
                        JobCardId = 10,
                        WorkBayId = 4,
                        NextJobCardId = 11
                    },
                    CancellationToken.None));

            Assert.AreEqual("JobCard không thuộc WorkBay này.", ex.Message);
            _jobCardRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        private static JobCardEntity BuildJobCard(int id, decimal queueOrder, int workBayId)
        {
            return new JobCardEntity
            {
                JobCardId = id,
                QueueOrder = queueOrder,
                WorkBayId = workBayId
            };
        }
    }
}
