using Garage_Management.Application.DTOs.JobCardMechanics;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.JobCardMechanics
{
    [TestClass]
    public class GetJobCardsByEmployeeAsyncTests
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
        public async Task GetJobCardsByEmployeeAsync_NotAuthenticated_ReturnsLoginError()
        {
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            var response = await _service.GetJobCardsByEmployeeAsync(new ParamQuery(), CancellationToken.None);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Vui lòng đăng nhập", response.Message);
            Assert.IsNull(response.Data);
        }

        [TestMethod]
        public async Task GetJobCardsByEmployeeAsync_InvalidClaims_ReturnsInvalidUserInfo()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
                // Role claim missing intentionally
            }, "Test"));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var response = await _service.GetJobCardsByEmployeeAsync(new ParamQuery(), CancellationToken.None);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Thông tin người dùng không hợp lệ", response.Message);
            Assert.IsNull(response.Data);
        }

        [TestMethod]
        public async Task GetJobCardsByEmployeeAsync_InvalidUserId_ReturnsEmployeeIdInvalid()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "0"),
                new Claim("EmployeeId", "5"),
                new Claim(ClaimTypes.Role, "Mechanic")
            }, "Test"));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var response = await _service.GetJobCardsByEmployeeAsync(new ParamQuery(), CancellationToken.None);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("EmployeeId không hợp lệ", response.Message);
            Assert.IsNull(response.Data);
        }

        [TestMethod]
        public async Task GetJobCardsByEmployeeAsync_NoJobCards_ReturnsNotFound()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("EmployeeId", "10"),
                new Claim(ClaimTypes.Role, "Mechanic")
            }, "Test"));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            _repositoryMock.Setup(x => x.GetJobCardsByEmployeeIdAsync(10, It.IsAny<ParamQuery>())).ReturnsAsync((PagedResult<JobCardMechanicDto>)null);

            var response = await _service.GetJobCardsByEmployeeAsync(new ParamQuery(), CancellationToken.None);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Không tìm thấy phiếu sửa chữa nào cho thợ máy này", response.Message);
            Assert.IsNull(response.Data);
        }

        [TestMethod]
        public async Task GetJobCardsByEmployeeAsync_EmptyJobCards_ReturnsNotFound()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("EmployeeId", "10"),
                new Claim(ClaimTypes.Role, "Mechanic")
            }, "Test"));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
            _repositoryMock.Setup(x => x.GetJobCardsByEmployeeIdAsync(10, It.IsAny<ParamQuery>())).ReturnsAsync(new PagedResult<JobCardMechanicDto>
            {
                Total = 0,
                Page = 1,
                PageSize = 10,
                PageData = new List<JobCardMechanicDto>()
            });

            var response = await _service.GetJobCardsByEmployeeAsync(new ParamQuery(), CancellationToken.None);

            Assert.IsFalse(response.Success);
            Assert.AreEqual("Không tìm thấy phiếu sửa chữa nào cho thợ máy này", response.Message);
            Assert.IsNull(response.Data);
        }

        [TestMethod]
        public async Task GetJobCardsByEmployeeAsync_ValidRequest_ReturnsPagedResult()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("EmployeeId", "10"),
                new Claim(ClaimTypes.Role, "Mechanic")
            }, "Test"));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            var expectedPagedResult = new PagedResult<JobCardMechanicDto>
            {
                Total = 1,
                Page = 1,
                PageSize = 10,
                PageData = new List<JobCardMechanicDto>
                {
                    new JobCardMechanicDto
                    {
                        JobCardId = 100,
                        EmployeeId = 10,
                        MechanicAssignmenStatus = MechanicAssignmentStatus.InProgress,
                        JobCardDescription = "Test job card"
                    }
                }
            };
            _repositoryMock.Setup(x => x.GetJobCardsByEmployeeIdAsync(10, It.IsAny<ParamQuery>())).ReturnsAsync(expectedPagedResult);

            var response = await _service.GetJobCardsByEmployeeAsync(new ParamQuery { Page = 1, PageSize = 10 }, CancellationToken.None);

            Assert.IsTrue(response.Success);
            Assert.AreEqual("Lấy danh sách phiếu sửa chữa thành công", response.Message);
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(1, response.Data.Total);
            Assert.AreEqual(100, response.Data.PageData.First().JobCardId);
        }
    }
}