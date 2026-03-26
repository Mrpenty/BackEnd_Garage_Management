using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;
using WorkBayEntity = Garage_Management.Base.Entities.JobCards.WorkBay;

namespace Garage_Management.UnitTest.JobCards
{
    [TestClass]
    public class WorkBayAssignmentTests
    {
        private Mock<IJobCardRepository> _jobCardRepo;
        private Mock<IServiceRepository> _serviceRepo;
        private Mock<IInventoryRepository> _inventoryRepo;
        private Mock<IJobCardServiceRepository> _jobCardServiceRepo;
        private Mock<IJobCardSparePartRepository> _jobCardSparePartRepo;
        private Mock<IWorkBayRepository> _workBayRepo;
        private Mock<IAppointmentRepository> _appointmentRepo;

        private JobCardService _service;

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

            _service = new JobCardService(
                _jobCardRepo.Object,
                _serviceRepo.Object,
                _inventoryRepo.Object,
                _jobCardServiceRepo.Object,
                _jobCardSparePartRepo.Object,
                _workBayRepo.Object,
                _appointmentRepo.Object
            );
        }

        [TestMethod]
        public async Task AssignWorkBayAsync_ReturnFalse_WhenJobCardNotFound()
        {
            _jobCardRepo.Setup(x => x.GetByIdAsync(999))
                        .ReturnsAsync((JobCardEntity)null);

            var result = await _service.AssignWorkBayAsync(
                new AssignWorkBayRequestDto { JobCardId = 999, WorkBayId = 1 },
                CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AssignWorkBayAsync_ReturnFalse_WhenWorkBayNotFound()
        {
            var jobCard = new JobCardEntity { JobCardId = 2 };

            _jobCardRepo.Setup(x => x.GetByIdAsync(2))
                        .ReturnsAsync(jobCard);

            _workBayRepo.Setup(x => x.GetByIdAsync(99))
                        .ReturnsAsync((WorkBayEntity)null);

            var result = await _service.AssignWorkBayAsync(
                new AssignWorkBayRequestDto { JobCardId = 2, WorkBayId = 99 },
                CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AssignWorkBayAsync_ReturnFalse_WhenWorkBayOccupied()
        {
            var jobCard = new JobCardEntity { JobCardId = 3 };

            var bay = new WorkBayEntity
            {
                Id = 5,
                Status = WorkBayStatus.Occupied,
                JobcardId = 123
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(3))
                        .ReturnsAsync(jobCard);

            _workBayRepo.Setup(x => x.GetByIdAsync(5))
                        .ReturnsAsync(bay);

            var result = await _service.AssignWorkBayAsync(
                new AssignWorkBayRequestDto { JobCardId = 3, WorkBayId = 5 },
                CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AssignWorkBayAsync_ReturnTrue_WhenValid()
        {
            var jobCard = new JobCardEntity { JobCardId = 4 };

            var bay = new WorkBayEntity
            {
                Id = 6,
                Status = WorkBayStatus.Available
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(4))
                        .ReturnsAsync(jobCard);

            _workBayRepo.Setup(x => x.GetByIdAsync(6))
                        .ReturnsAsync(bay);



            _workBayRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                        .Returns(Task.FromResult(1));

            var result = await _service.AssignWorkBayAsync(
                new AssignWorkBayRequestDto { JobCardId = 4, WorkBayId = 6 },
                CancellationToken.None);

            Assert.IsTrue(result);
            Assert.AreEqual(WorkBayStatus.Occupied, bay.Status);
            Assert.AreEqual(4, bay.JobcardId);
        }

        [TestMethod]
        public async Task ReleaseWorkBayAsync_ReturnFalse_WhenWorkBayNotFound()
        {
            _workBayRepo.Setup(x => x.GetByIdAsync(999))
                        .ReturnsAsync((WorkBayEntity)null);

            var result = await _service.ReleaseWorkBayAsync(
                new ReleaseWorkBayDto { WorkBayId = 999 },
                CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ReleaseWorkBayAsync_ReturnTrue_WhenValid()
        {
            var bay = new WorkBayEntity
            {
                Id = 7,
                Status = WorkBayStatus.Occupied,
                JobcardId = 55
            };

            _workBayRepo.Setup(x => x.GetByIdAsync(7))
                        .ReturnsAsync(bay);

            _workBayRepo
    .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
    .Returns(Task.CompletedTask);

            var result = await _service.ReleaseWorkBayAsync(
                new ReleaseWorkBayDto { WorkBayId = 7 },
                CancellationToken.None);

            Assert.IsTrue(result);
            Assert.AreEqual(WorkBayStatus.Available, bay.Status);
            Assert.IsNull(bay.JobcardId);
        }
    }
}