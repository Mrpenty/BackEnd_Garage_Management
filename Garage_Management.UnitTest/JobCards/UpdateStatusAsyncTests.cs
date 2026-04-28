using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Format;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;

namespace Garage_Management.UnitTest.JobCards
{
    [TestClass]
    public class UpdateStatusAsyncTests
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
        public async Task UpdateStatusAsync_ReturnsFalse_WhenJobCardNotFound()
        {
            _jobCardRepo
                .Setup(x => x.GetByIdAsync(99))
                .ReturnsAsync((JobCardEntity?)null);

            var result = await _service.UpdateStatusAsync(99, JobCardStatus.InProgress, CancellationToken.None);

            Assert.IsFalse(result);
            _jobCardRepo.Verify(x => x.Update(It.IsAny<JobCardEntity>()), Times.Never);
            _jobCardRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_UpdatesStatus_AndUsesProvidedCancellationToken()
        {
            var entity = new JobCardEntity
            {
                JobCardId = 10,
                Status = JobCardStatus.Created
            };
            using var cts = new CancellationTokenSource();

            _jobCardRepo
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(entity);
            _jobCardRepo
                .Setup(x => x.SaveAsync(cts.Token))
                .ReturnsAsync(1);

            var result = await _service.UpdateStatusAsync(10, JobCardStatus.InProgress, cts.Token);

            Assert.IsTrue(result);
            Assert.AreEqual(JobCardStatus.InProgress, entity.Status);
            Assert.IsNull(entity.EndDate);
            _jobCardRepo.Verify(x => x.Update(entity), Times.Once);
            _jobCardRepo.Verify(x => x.SaveAsync(cts.Token), Times.Once);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_SetsEndDateInSeAsiaTimeZone_WhenCompleted()
        {
            var entity = new JobCardEntity
            {
                JobCardId = 11,
                Status = JobCardStatus.InProgress
            };
            var expectedTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var beforeUpdate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, expectedTimeZone);

            _jobCardRepo
                .Setup(x => x.GetByIdAsync(11))
                .ReturnsAsync(entity);
            _jobCardRepo
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _service.UpdateStatusAsync(11, JobCardStatus.Completed, CancellationToken.None);

            var afterUpdate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, expectedTimeZone);

            Assert.IsTrue(result);
            Assert.AreEqual(JobCardStatus.Completed, entity.Status);
            Assert.IsNotNull(entity.EndDate);
            Assert.IsTrue(entity.EndDate >= beforeUpdate);
            Assert.IsTrue(entity.EndDate <= afterUpdate);
            _jobCardRepo.Verify(x => x.Update(entity), Times.Once);
            _jobCardRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_OverwritesExistingEndDate_WhenCompleted()
        {
            var originalEndDate = new DateTime(2025, 1, 1, 8, 0, 0);
            var entity = new JobCardEntity
            {
                JobCardId = 12,
                Status = JobCardStatus.InProgress,
                EndDate = originalEndDate
            };

            _jobCardRepo
                .Setup(x => x.GetByIdAsync(12))
                .ReturnsAsync(entity);
            _jobCardRepo
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _service.UpdateStatusAsync(12, JobCardStatus.Completed, CancellationToken.None);

            Assert.IsTrue(result);
            Assert.AreEqual(JobCardStatus.Completed, entity.Status);
            Assert.AreNotEqual(originalEndDate, entity.EndDate);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_DoesNotChangeExistingEndDate_WhenStatusIsNotCompleted()
        {
            var originalEndDate = new DateTime(2025, 1, 1, 8, 0, 0);
            var entity = new JobCardEntity
            {
                JobCardId = 13,
                Status = JobCardStatus.Completed,
                EndDate = originalEndDate
            };

            _jobCardRepo
                .Setup(x => x.GetByIdAsync(13))
                .ReturnsAsync(entity);
            _jobCardRepo
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _service.UpdateStatusAsync(13, JobCardStatus.WaitingPickup, CancellationToken.None);

            Assert.IsTrue(result);
            Assert.AreEqual(JobCardStatus.WaitingPickup, entity.Status);
            Assert.AreEqual(originalEndDate, entity.EndDate);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_ReturnsTrue_WhenSaveReturnsZero()
        {
            var entity = new JobCardEntity
            {
                JobCardId = 14,
                Status = JobCardStatus.Created
            };

            _jobCardRepo
                .Setup(x => x.GetByIdAsync(14))
                .ReturnsAsync(entity);
            _jobCardRepo
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            var result = await _service.UpdateStatusAsync(14, JobCardStatus.InProgress, CancellationToken.None);

            Assert.IsTrue(result);
            Assert.AreEqual(JobCardStatus.InProgress, entity.Status);
            _jobCardRepo.Verify(x => x.Update(entity), Times.Once);
            _jobCardRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
