using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using JobCardEntity = Garage_Management.Base.Entities.JobCards.JobCard;
using JobCardServiceEntity = Garage_Management.Base.Entities.JobCards.JobCardService;
using JobCardServiceTaskEntity = Garage_Management.Base.Entities.JobCards.JobCardServiceTask;
using System.Security.Claims;

using Garage_Management.Base.Entities.JobCards;

namespace Garage_Management.UnitTest.JobCards
{
    [TestClass]
    public class RepairProgressTests
    {
        private Mock<IJobCardRepository> _jobCardRepo;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Garage_Management.Application.Services.JobCards.JobCardService _service;

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
                _httpContextAccessor.Object
            );
        }

        [TestMethod]
        public async Task UpdateRepairProgressAsync_NotAuthenticated_ReturnsError()
        {
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) });

            var result = await _service.UpdateRepairProgressAsync(1, new UpdateJobCardProgressDto(), CancellationToken.None);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Vui lòng đăng nhập để cập nhật tiến độ", result.Message);
        }

        [TestMethod]
        public async Task UpdateRepairProgressAsync_MechanicNotAssigned_ThrowsUnauthorizedAccessException()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "3") }, "Test"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            var jobCard = new JobCardEntity { JobCardId = 2, Status = JobCardStatus.InProgress, ProgressPercentage = 50, Services = new List<JobCardServiceEntity>() };
            _jobCardRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(jobCard);
            _jobCardRepo.Setup(x => x.IsMechanicAssignedAsync(2, 3)).ReturnsAsync(false);

            await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () =>
            {
                await _service.UpdateRepairProgressAsync(2, new UpdateJobCardProgressDto(), CancellationToken.None);
            });
        }

        [TestMethod]
        public async Task UpdateRepairProgressAsync_UpdatesAndCompletesJobCard_WhenAllServicesCompleted()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "3") }, "Test"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            var jobCardService = new JobCardServiceEntity
            {
                JobCardServiceId = 100,
                ServiceId = 1,
                Service = new Service { ServiceName = "Thay dầu" },
                Status = ServiceStatus.InProgress,
                CreatedAt = DateTime.UtcNow
            };

            var jobCard = new JobCardEntity
            {
                JobCardId = 2,
                Status = JobCardStatus.InProgress,
                ProgressPercentage = 30,
                StartDate = DateTime.UtcNow.AddHours(-1),
                ProgressNotes = "Bắt đầu",
                Mechanics = new List<JobCardMechanic> { new JobCardMechanic { EmployeeId = 3 } },
                Services = new List<JobCardServiceEntity> { jobCardService }
            };

            _jobCardRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(jobCard);
            _jobCardRepo.Setup(x => x.IsMechanicAssignedAsync(2, 3)).ReturnsAsync(true);
            _jobCardRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var dto = new UpdateJobCardProgressDto
            {
                Status = JobCardStatus.InProgress,
                ProgressPercentage = 80,
                ProgressNotes = "Hoàn thành bước 1",
                ServiceUpdates = new List<UpdateServiceStatusDto>
                {
                    new UpdateServiceStatusDto { JobCardServiceId = 100, Status = ServiceStatus.Completed }
                }
            };

            var result = await _service.UpdateRepairProgressAsync(2, dto, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Cập nhật tiến độ thành công.", result.Message);
            Assert.AreEqual(JobCardStatus.Completed, result.Data!.Status); // tất cả service đã complete => JobCardCompleted
            Assert.AreEqual(100, result.Data.ProgressPercentage);
            Assert.IsNotNull(result.Data.EndDate);
            Assert.AreEqual(ServiceStatus.Completed, result.Data.Services[0].Status);
        }

        [TestMethod]
        public async Task ViewRepairProgressAsync_NotAuthenticated_ReturnsError()
        {
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) });

            var result = await _service.ViewRepairProgressAsync(1, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Vui lòng đăng nhập để cập nhật tiến độ", result.Message);
        }

        [TestMethod]
        public async Task ViewRepairProgressAsync_CustomerHasNoPermission_ReturnsError()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "5"),
                new Claim(ClaimTypes.Role, "Customer")
            }, "Test"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            var jobCard = new JobCardEntity
            {
                JobCardId = 4,
                Customer = new Customer { User = new User { Id = 10, PhoneNumber = "012" }, FirstName = "Nguyen", LastName = "An", UserId = 10 },
                CustomerId = 10,
                StartDate = DateTime.UtcNow,
                Status = JobCardStatus.InProgress,
                ProgressPercentage = 40,
                Services = new List<JobCardServiceEntity>()
            };
            _jobCardRepo.Setup(x => x.GetByIdAsync(4)).ReturnsAsync(jobCard);

            var result = await _service.ViewRepairProgressAsync(4, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Bạn không có quyền xem phiếu sửa chữa này.", result.Message);
        }

        [TestMethod]
        public async Task ViewRepairProgressAsync_Customer_ReturnsSuccessWithHiddenNotes()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "10"),
                new Claim(ClaimTypes.Role, "Customer"),
                new Claim("CustomerId", "10")
            }, "Test"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            var serviceTask = new JobCardServiceTaskEntity
            {
                JobCardServiceTaskId = 1,
                ServiceTask = new ServiceTask { TaskName = "Thay dầu", EstimateMinute = 30 },
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
                Service = new Service { ServiceName = "Dịch vụ A" },
                Status = ServiceStatus.InProgress,
                ServiceTasks = new List<JobCardServiceTask> { serviceTask }
            };

            var jobCard = new JobCardEntity
            {
                JobCardId = 10,
                Customer = new Customer { User = new User { Id = 10, PhoneNumber = "012345" }, FirstName = "Nguyen", LastName = "An", UserId = 10 },
                CustomerId = 10,
                Status = JobCardStatus.InProgress,
                ProgressPercentage = 45,
                ProgressNotes = "Đang làm",
                StartDate = DateTime.UtcNow,
                Services = new List<JobCardServiceEntity> { jobCardService },
                Mechanics = new List<JobCardMechanic>() { mechanic }
            };
            _jobCardRepo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(jobCard);

            var result = await _service.ViewRepairProgressAsync(10, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Lấy thông tin tiến độ thành công.", result.Message);
            Assert.IsNull(result.Data!.ProgressNotes);
            Assert.AreEqual(1, result.Data.Services.Count);
            Assert.AreEqual(1, result.Data.AssignedMechanic == null ? 0 : 1); 
        }
    }
}
