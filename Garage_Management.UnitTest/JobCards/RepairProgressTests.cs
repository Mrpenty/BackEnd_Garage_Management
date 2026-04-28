using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Format;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;
using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;
using JobCardServiceEntity = Garage_Management.Base.Entities.JobCards.JobCardService;
using JobCardServiceTaskEntity = Garage_Management.Base.Entities.JobCards.JobCardServiceTask;

namespace Garage_Management.UnitTest.JobCards
{
    [TestClass]
    public class RepairProgressTests
    {
        private Mock<IJobCardRepository> _jobCardRepo = null!;
        private Mock<IHttpContextAccessor> _httpContextAccessor = null!;
        private Garage_Management.Application.Services.JobCards.JobCardService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _jobCardRepo = new Mock<IJobCardRepository>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _service = new Application.Services.JobCards.JobCardService(
                _jobCardRepo.Object,
                Mock.Of<IServiceRepository>(),
                Mock.Of<IInventoryRepository>(),
                Mock.Of<IJobCardServiceRepository>(),
                Mock.Of<IJobCardSparePartRepository>(),
                Mock.Of<IWorkBayRepository>(),
                Mock.Of<IAppointmentRepository>(),
                _httpContextAccessor.Object,
                new ProgressCalculator());
        }

        [TestMethod]
        public async Task UpdateRepairProgressAsync_NotAuthenticated_ReturnsError()
        {
            _httpContextAccessor.Setup(x => x.HttpContext)
                .Returns(new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) });

            var result = await _service.UpdateRepairProgressAsync(1, new UpdateJobCardProgressDto(), CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Message));
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public async Task UpdateRepairProgressAsync_MechanicNotAssigned_ThrowsUnauthorizedAccessException()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, "3") },
                "Test"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            var jobCard = new JobCardEntity
            {
                JobCardId = 2,
                Status = JobCardStatus.InProgress,
                ProgressPercentage = 50,
                Services = new List<JobCardServiceEntity>()
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(jobCard);

            var result = await _service.UpdateRepairProgressAsync(2, new UpdateJobCardProgressDto(), CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Message));
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public async Task UpdateRepairProgressAsync_UpdatesAndCompletesJobCard_WhenAllServicesCompleted()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, "3") },
                "Test"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            var jobCardService = new JobCardServiceEntity
            {
                JobCardServiceId = 100,
                ServiceId = 1,
                Service = new Service { ServiceName = "Oil change" },
                Status = ServiceStatus.InProgress,
                CreatedAt = DateTime.UtcNow
            };

            var jobCard = new JobCardEntity
            {
                JobCardId = 2,
                Status = JobCardStatus.InProgress,
                ProgressPercentage = 30,
                StartDate = DateTime.UtcNow.AddHours(-1),
                ProgressNotes = "Started",
                Mechanics = new List<JobCardMechanic> { new JobCardMechanic { EmployeeId = 3 } },
                Services = new List<JobCardServiceEntity> { jobCardService }
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(jobCard);
            _jobCardRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var dto = new UpdateJobCardProgressDto
            {
                StatusJobCard = JobCardStatus.InProgress,
                ProgressPercentage = 80,
                ProgressNotes = "Step 1 completed",
                ServiceUpdates = new List<UpdateServiceStatusDto>
                {
                    new UpdateServiceStatusDto { JobCardServiceId = 100, StatusService = ServiceStatus.Completed }
                }
            };

            var result = await _service.UpdateRepairProgressAsync(2, dto, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Message));
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(JobCardStatus.Completed, result.Data.StatusJobCard);
            Assert.AreEqual(100, result.Data.ProgressPercentage);
            Assert.IsNotNull(result.Data.EndDate);
            Assert.AreEqual(ServiceStatus.Completed, result.Data.Services[0].StatusService);
        }

        [TestMethod]
        public async Task ViewRepairProgressAsync_NotAuthenticated_ReturnsError()
        {
            _httpContextAccessor.Setup(x => x.HttpContext)
                .Returns(new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) });

            var result = await _service.ViewRepairProgressAsync(1, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Message));
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public async Task ViewRepairProgressAsync_CustomerHasNoPermission_ReturnsError()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "5"),
                new Claim(ClaimTypes.Role, "Customer")
            }, "Test"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            var jobCard = new JobCardEntity
            {
                JobCardId = 4,
                Customer = new Customer
                {
                    User = new User { Id = 10, PhoneNumber = "012" },
                    FirstName = "Nguyen",
                    LastName = "An",
                    UserId = 10
                },
                CustomerId = 10,
                StartDate = DateTime.UtcNow,
                Status = JobCardStatus.InProgress,
                ProgressPercentage = 40,
                Services = new List<JobCardServiceEntity>()
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(4)).ReturnsAsync(jobCard);

            var result = await _service.ViewRepairProgressAsync(4, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Message));
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public async Task ViewRepairProgressAsync_Customer_ReturnsSuccessWithHiddenNotes()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "10"),
                new Claim(ClaimTypes.Role, "Customer"),
                new Claim("CustomerId", "10")
            }, "Test"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            var serviceTask = new JobCardServiceTaskEntity
            {
                JobCardServiceTaskId = 1,
                ServiceTask = new ServiceTask { TaskName = "Oil change", EstimateMinute = 30 },
                Status = ServiceStatus.InProgress
            };

            var mechanic = new JobCardMechanic
            {
                EmployeeId = 5,
            };

            var jobCardService = new JobCardService
            {
                JobCardServiceId = 100,
                ServiceId = 1,
                Service = new Service { ServiceName = "Service A" },
                Status = ServiceStatus.InProgress,
                ServiceTasks = new List<JobCardServiceTask> { serviceTask }
            };

            var jobCard = new JobCardEntity
            {
                JobCardId = 10,
                Customer = new Customer
                {
                    User = new User { Id = 10, PhoneNumber = "012345" },
                    FirstName = "Nguyen",
                    LastName = "An",
                    UserId = 10
                },
                CustomerId = 10,
                Status = JobCardStatus.InProgress,
                ProgressPercentage = 45,
                ProgressNotes = "Working",
                StartDate = DateTime.UtcNow,
                Services = new List<JobCardServiceEntity> { jobCardService },
                Mechanics = new List<JobCardMechanic> { mechanic }
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(jobCard);

            var result = await _service.ViewRepairProgressAsync(10, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Message));
            Assert.IsNotNull(result.Data);
            Assert.IsNull(result.Data.ProgressNotes);
            Assert.AreEqual(1, result.Data.Services.Count);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Data.AssignedMechanic));
        }
    }
}
