using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Format;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
// fix trùng namespace JobCard
using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;
using JobCardServiceApp = Garage_Management.Application.Services.JobCards.JobCardService;

namespace Garage_Management.UnitTest.JobCards
{
    [TestClass]
    public class JobCardServiceCrudTests
    {
        private Mock<IJobCardRepository> _jobCardRepo;
        private Mock<IServiceRepository> _serviceRepo;
        private Mock<IInventoryRepository> _inventoryRepo;
        private Mock<IJobCardServiceRepository> _jobCardServiceRepo;
        private Mock<IJobCardSparePartRepository> _jobCardSparePartRepo;
        private Mock<IWorkBayRepository> _workBayRepo;
        private Mock<IAppointmentRepository> _appointmentRepo;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Mock<ProgressCalculator> _progressCalculator;
        private JobCardServiceApp _service;

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
                _progressCalculator.Object

            );
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _jobCardRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((JobCardEntity)null);

            var result = await _service.GetByIdAsync(1);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsDto_WhenExists()
        {
            var jobCard = new JobCardEntity
            {
                JobCardId = 10,
                CustomerId = 1,
                VehicleId = 1,
                StartDate = DateTime.UtcNow,
                Status = JobCardStatus.Created
            };

            _jobCardRepo
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(jobCard);

            var result = await _service.GetByIdAsync(10);

            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.JobCardId);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_ReturnsFalse_WhenJobCardNotFound()
        {
            _jobCardRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((JobCardEntity)null);

            var result = await _service.UpdateStatusAsync(
                99,
                JobCardStatus.Completed,
                CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_CompletesJobCard()
        {
            var jobCard = new JobCardEntity
            {
                JobCardId = 20,
                Status = JobCardStatus.InProgress,
                StartDate = DateTime.UtcNow
            };

            _jobCardRepo
                .Setup(x => x.GetByIdAsync(20))
                .ReturnsAsync(jobCard);

            _jobCardRepo
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var result = await _service.UpdateStatusAsync(
                20,
                JobCardStatus.Completed,
                CancellationToken.None);

            Assert.IsTrue(result);
            Assert.AreEqual(JobCardStatus.Completed, jobCard.Status);
            Assert.IsNotNull(jobCard.EndDate);
        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsFalse_WhenJobCardNotFound()
        {
            _jobCardRepo
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((JobCardEntity)null);

            var result = await _service.UpdateAsync(
                1,
                new UpdateJobCardDto { Note = "test" },
                CancellationToken.None);

            Assert.IsFalse(result);
        }
    }
}