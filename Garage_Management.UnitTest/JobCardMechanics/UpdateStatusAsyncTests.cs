using Garage_Management.Application.DTOs.JobCardMechanics;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.JobCardMechanics
{
    [TestClass]
    public class UpdateStatusAsyncTests
    {
        private Mock<IJobCardMechanicRepository> _repositoryMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private JobCardMechanicService _service;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IJobCardMechanicRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _service = new JobCardMechanicService(_repositoryMock.Object, _httpContextAccessorMock.Object);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_NotAuthenticated_ReturnsLoginError()
        {
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            var response = await _service.UpdateStatusAsync(1, new UpdateJobCardMechanicStatusDto { NewStatus = MechanicAssignmentStatus.InProgress });

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Vui lòng đăng nhập", response.Message);
            Assert.IsNull(response.Data);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_InvalidClaims_ReturnsInvalidUserInfo()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "Test"));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var response = await _service.UpdateStatusAsync(1, new UpdateJobCardMechanicStatusDto { NewStatus = MechanicAssignmentStatus.InProgress });

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Thông tin người dùng không hợp lệ", response.Message);
            Assert.IsNull(response.Data);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_JobCardMechanicNotFound_ReturnsNotFoundError()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("EmployeeId", "20"),
                new Claim(ClaimTypes.Role, "Mechanic")
            }, "Test"));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            _repositoryMock.Setup(x => x.GetByIdsAsync(5, 20)).ReturnsAsync((JobCardMechanic)null);

            var response = await _service.UpdateStatusAsync(5, new UpdateJobCardMechanicStatusDto { NewStatus = MechanicAssignmentStatus.InProgress });

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Không tìm thấy phân công thợ này", response.Message);
            Assert.IsNull(response.Data);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_UpdateStatusFails_ReturnsFailureMessage()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("EmployeeId", "20"),
                new Claim(ClaimTypes.Role, "Mechanic")
            }, "Test"));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var existingAssignment = new JobCardMechanic
            {
                JobCardId = 5,
                EmployeeId = 20,
                Status = MechanicAssignmentStatus.OnHold,
                JobCard = new JobCard { Note = "Original task" }
            };
            _repositoryMock.Setup(x => x.GetByIdsAsync(5, 20)).ReturnsAsync(existingAssignment);
            _repositoryMock.Setup(x => x.UpdateStatusAsync(5, 20, MechanicAssignmentStatus.InProgress)).ReturnsAsync(false);

            var response = await _service.UpdateStatusAsync(5, new UpdateJobCardMechanicStatusDto { NewStatus = MechanicAssignmentStatus.InProgress });

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Cập nhật trạng thái thất bại", response.Message);
            Assert.IsNull(response.Data);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_ValidUpdate_ReturnsUpdatedDto()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("EmployeeId", "20"),
                new Claim(ClaimTypes.Role, "Mechanic")
            }, "Test"));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var existingAssignment = new JobCardMechanic
            {
                JobCardId = 5,
                EmployeeId = 20,
                Status = MechanicAssignmentStatus.OnHold,
                JobCard = new JobCard { Note = "Original task" }
            };
            var updatedAssignment = new JobCardMechanic
            {
                JobCardId = 5,
                EmployeeId = 20,
                Status = MechanicAssignmentStatus.InProgress,
                JobCard = new JobCard { Note = "Updated task" }
            };

            _repositoryMock.Setup(x => x.GetByIdsAsync(5, 20)).ReturnsAsync(existingAssignment);
            _repositoryMock.Setup(x => x.UpdateStatusAsync(5, 20, MechanicAssignmentStatus.InProgress)).ReturnsAsync(true);
            _repositoryMock.SetupSequence(x => x.GetByIdsAsync(5, 20))
                .ReturnsAsync(existingAssignment)
                .ReturnsAsync(updatedAssignment);

            var response = await _service.UpdateStatusAsync(5, new UpdateJobCardMechanicStatusDto { NewStatus = MechanicAssignmentStatus.InProgress });

            Assert.IsTrue(response.Success);
            Assert.AreEqual("Cập nhật trạng thái thành công", response.Message);
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(5, response.Data.JobCardId);
            Assert.AreEqual(20, response.Data.EmployeeId);
            Assert.AreEqual(MechanicAssignmentStatus.InProgress, response.Data.MechanicAssignmenStatus);
            Assert.AreEqual("Updated task", response.Data.JobCardDescription);
        }
    }
}