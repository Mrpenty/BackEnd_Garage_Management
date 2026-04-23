using Garage_Management.Application.DTOs.JobCardMechanics;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.JobCardMechanics
{
    [TestClass]
    public class StartInspectionAsyncTests
    {
        private Mock<IJobCardMechanicRepository> _repositoryMock = null!;
        private Mock<IJobCardRepository> _jobCardRepositoryMock = null!;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock = null!;
        private JobCardMechanicService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IJobCardMechanicRepository>();
            _jobCardRepositoryMock = new Mock<IJobCardRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _service = new JobCardMechanicService(_repositoryMock.Object, _jobCardRepositoryMock.Object, _httpContextAccessorMock.Object);
        }

        [TestMethod]
        public async Task StartInspectionAsync_WhenMechanicBusy_ReturnsError()
        {
            _repositoryMock.Setup(x => x.GetAll()).Returns(new List<JobCardMechanic>
            {
                new JobCardMechanic
                {
                    JobCardId = 99,
                    EmployeeId = 5,
                    Status = MechanicAssignmentStatus.InProgress
                }
            }.AsQueryable());

            var response = await _service.StartInspectionAsync(new StartInspectionDto
            {
                JobCardId = 1,
                MechanicId = 5
            }, CancellationToken.None);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Thợ đang bận sửa xe khác, không thể yêu cầu sửa jobCard này", response.Message);
        }

        [TestMethod]
        public async Task StartInspectionAsync_WhenAssignmentExists_UpdatesStatuses()
        {
            var jobCard = new JobCard
            {
                JobCardId = 1,
                Status = JobCardStatus.WaitingInspection,
                Note = "Kiểm tra tiếng máy"
            };
            var assignment = new JobCardMechanic
            {
                JobCardId = 1,
                EmployeeId = 5,
                Status = MechanicAssignmentStatus.Assigned,
                JobCard = jobCard
            };

            _repositoryMock.Setup(x => x.GetAll()).Returns(new List<JobCardMechanic>().AsQueryable());
            _jobCardRepositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(jobCard);
            _repositoryMock.Setup(x => x.GetByIdsAsync(1, 5)).ReturnsAsync(assignment);
            _jobCardRepositoryMock.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var response = await _service.StartInspectionAsync(new StartInspectionDto
            {
                JobCardId = 1,
                MechanicId = 5
            }, CancellationToken.None);

            Assert.IsTrue(response.Success);
            Assert.AreEqual(MechanicAssignmentStatus.InProgress, assignment.Status);
            Assert.AreEqual(JobCardStatus.Inspection, jobCard.Status);
            Assert.IsNotNull(assignment.StartedAt);
        }
    }
}
