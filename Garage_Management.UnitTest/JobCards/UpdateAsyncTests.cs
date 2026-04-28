using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Format;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using JobCardServiceApp = Garage_Management.Application.Services.JobCards.JobCardService;

namespace Garage_Management.UnitTest.JobCards
{
    [TestClass]
    public class UpdateAsyncTests
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
        private JobCardServiceApp _service = null!;

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

            _service = new JobCardServiceApp(
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
        public async Task UpdateAsync_ReturnsFalse_WhenJobCardNotFound()
        {
            _jobCardRepo.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((JobCard?)null);

            var result = await _service.UpdateAsync(999, new UpdateJobCardDto(), CancellationToken.None);

            Assert.IsFalse(result);
            _jobCardRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsFalse_WhenJobCardIsSoftDeleted()
        {
            _jobCardRepo.Setup(x => x.GetByIdAsync(5))
                .ReturnsAsync(new JobCard
                {
                    JobCardId = 5,
                    DeletedAt = new DateTime(2026, 4, 1)
                });

            var result = await _service.UpdateAsync(5, new UpdateJobCardDto { Note = "ignored" }, CancellationToken.None);

            Assert.IsFalse(result);
            _jobCardRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateAsync_UpdatesAllProvidedFields_AndUsesProvidedCancellationToken()
        {
            using var cts = new CancellationTokenSource();
            var entity = new JobCard
            {
                JobCardId = 10,
                Note = "old note",
                SupervisorId = 2,
                EndDate = new DateTime(2026, 4, 25, 8, 0, 0)
            };
            var newEndDate = new DateTime(2026, 4, 26, 18, 0, 0);

            _jobCardRepo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(entity);
            _jobCardRepo.Setup(x => x.SaveAsync(cts.Token)).ReturnsAsync(1);

            var result = await _service.UpdateAsync(
                10,
                new UpdateJobCardDto
                {
                    Note = "updated note",
                    SupervisorId = 7,
                    EndDate = newEndDate
                },
                cts.Token);

            Assert.IsTrue(result);
            Assert.AreEqual("updated note", entity.Note);
            Assert.AreEqual(7, entity.SupervisorId);
            Assert.AreEqual(newEndDate, entity.EndDate);
            _jobCardRepo.Verify(x => x.SaveAsync(cts.Token), Times.Once);
        }

        [TestMethod]
        public async Task UpdateAsync_OnlyUpdatesProvidedFields_AndKeepsOtherValues()
        {
            var originalEndDate = new DateTime(2026, 4, 20, 9, 0, 0);
            var entity = new JobCard
            {
                JobCardId = 11,
                Note = "original note",
                SupervisorId = 3,
                EndDate = originalEndDate
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(11)).ReturnsAsync(entity);
            _jobCardRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.UpdateAsync(
                11,
                new UpdateJobCardDto
                {
                    SupervisorId = 9
                },
                CancellationToken.None);

            Assert.IsTrue(result);
            Assert.AreEqual("original note", entity.Note);
            Assert.AreEqual(9, entity.SupervisorId);
            Assert.AreEqual(originalEndDate, entity.EndDate);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenNoFieldsProvided_ReturnsTrue_AndStillSaves()
        {
            var entity = new JobCard
            {
                JobCardId = 12,
                Note = "keep me",
                SupervisorId = 4,
                EndDate = new DateTime(2026, 4, 21, 11, 0, 0)
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(12)).ReturnsAsync(entity);
            _jobCardRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.UpdateAsync(12, new UpdateJobCardDto(), CancellationToken.None);

            Assert.IsTrue(result);
            Assert.AreEqual("keep me", entity.Note);
            Assert.AreEqual(4, entity.SupervisorId);
            Assert.AreEqual(new DateTime(2026, 4, 21, 11, 0, 0), entity.EndDate);
            _jobCardRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
