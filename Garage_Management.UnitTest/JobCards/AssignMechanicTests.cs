using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Format;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;
using JobCardServiceApp = Garage_Management.Application.Services.JobCards.JobCardService;
namespace Garage_Management.UnitTest.JobCards
{
    [TestClass]
    public class AssignMechanicTests
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
        public async Task AssignMechanic_ReturnFalse_WhenJobCardNotFound()
        {
            _jobCardRepo
                .Setup(x => x.GetAll())
                .Returns(new List<JobCardEntity>().AsQueryable());

            var dto = new AssignMechanicDto { MechanicId = 1 };

            var result = await _service.AssignMechanicAsync(999, dto, CancellationToken.None);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AssignMechanic_ReturnTrue_WhenAlreadyAssigned()
        {
            var jobCard = new JobCardEntity
            {
                JobCardId = 10,
                Mechanics = new List<JobCardMechanic>
        {
            new JobCardMechanic
            {
                EmployeeId = 2,
                AssignedAt = DateTime.UtcNow,
                Status = MechanicAssignmentStatus.Assigned
            }
        }
            };

            _jobCardRepo
                .Setup(x => x.GetWithMechanicsAsync(10))
                .ReturnsAsync(jobCard);

            var dto = new AssignMechanicDto
            {
                MechanicId = 2
            };

            var result = await _service.AssignMechanicAsync(10, dto, CancellationToken.None);

            Assert.IsTrue(result);
            Assert.AreEqual(1, jobCard.Mechanics.Count);
        }

        [TestMethod]
        public async Task AssignMechanic_AddMechanic_WhenNotAssigned()
        {
            var jobCard = new JobCardEntity
            {
                JobCardId = 10,
                Mechanics = new List<JobCardMechanic>()
            };

            _jobCardRepo
                .Setup(x => x.GetWithMechanicsAsync(10))
                .ReturnsAsync(jobCard);

            _jobCardRepo
            .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
            var dto = new AssignMechanicDto { MechanicId = 5 };

            var result = await _service.AssignMechanicAsync(10, dto, CancellationToken.None);

            Assert.IsTrue(result);
            Assert.AreEqual(1, jobCard.Mechanics.Count);
        }

        [TestMethod]
        public async Task AssignMechanic_SetStatusAssigned()
        {
            var jobCard = new JobCardEntity
            {
                JobCardId = 20,
                Mechanics = new List<JobCardMechanic>()
            };

            _jobCardRepo
                .Setup(x => x.GetWithMechanicsAsync(20))
                .ReturnsAsync(jobCard);

            _jobCardRepo
                .Setup(x => x.SaveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var dto = new AssignMechanicDto
            {
                MechanicId = 3
            };

            await _service.AssignMechanicAsync(20, dto, CancellationToken.None);

            var mechanic = jobCard.Mechanics.First();

            Assert.AreEqual(MechanicAssignmentStatus.Assigned, mechanic.Status);
        }
    }
}