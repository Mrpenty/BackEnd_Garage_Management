using Garage_Management.Application.DTOs.User;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Accounts;
using Garage_Management.Base.Entities.Accounts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.UserTests
{
    [TestClass]
    public class ChangeUserStatusTests
    {
        private Mock<UserManager<User>> _mockUserManager;
        private Mock<ICustomerRepository> _mockCustomerRepository;
        private Mock<IEmployeeRepository> _mockEmployeeRepository;
        private Mock<RoleManager<IdentityRole<int>>> _mockRoleManager;
        private Mock<ILogger<UserService>> _mockLogger;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<HttpContext> _mockHttpContext;
        private UserService _userService;

        [TestInitialize]
        public void Setup()
        {
            _mockUserManager = CreateMockUserManager();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockEmployeeRepository = new Mock<IEmployeeRepository>();
            _mockRoleManager = CreateMockRoleManager();
            _mockLogger = new Mock<ILogger<UserService>>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpContext = new Mock<HttpContext>();

            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_mockHttpContext.Object);

            _userService = new UserService(
                _mockUserManager.Object,
                _mockCustomerRepository.Object,
                _mockEmployeeRepository.Object,
                _mockRoleManager.Object,
                _mockLogger.Object,
                _mockUserRepository.Object,
                _mockHttpContextAccessor.Object
            );
        }

        [TestMethod]
        public async Task ChangeUserStatusAsync_NotAuthenticated_ReturnsError()
        {
            _mockHttpContext.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity()));
            var request = new ChangeUserStatusRequest
            {
                UserId = 2,
                IsActive = false
            };

            var result = await _userService.ChangeUserStatusAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Không tìm thấy thông tin người dùng đăng nhập", result.Message);
        }

        [TestMethod]
        public async Task ChangeUserStatusAsync_UserNotAdmin_ReturnsError()
        {
            var principal = BuildClaimsPrincipal(1, new[] { "User" });
            _mockHttpContext.Setup(x => x.User).Returns(principal);

            var request = new ChangeUserStatusRequest
            {
                UserId = 2,
                IsActive = false
            };

            var result = await _userService.ChangeUserStatusAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Chỉ Admin mới có quyền thay đổi trạng thái tài khoản", result.Message);
        }

        [TestMethod]
        public async Task ChangeUserStatusAsync_RequestOnSelf_ReturnsError()
        {
            var principal = BuildClaimsPrincipal(1, new[] { "Admin" });
            _mockHttpContext.Setup(x => x.User).Returns(principal);

            var request = new ChangeUserStatusRequest
            {
                UserId = 1,
                IsActive = false
            };

            var result = await _userService.ChangeUserStatusAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Không thể tự thay đổi trạng thái tài khoản của chính mình", result.Message);
        }

        [TestMethod]
        public async Task ChangeUserStatusAsync_UserNotFound_ReturnsError()
        {
            var principal = BuildClaimsPrincipal(1, new[] { "Admin" });
            _mockHttpContext.Setup(x => x.User).Returns(principal);
            _mockUserManager.SetupGet(x => x.Users).Returns(new List<User>().AsQueryable());

            var request = new ChangeUserStatusRequest
            {
                UserId = 2,
                IsActive = false
            };

            var result = await _userService.ChangeUserStatusAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Không tìm thấy người dùng", result.Message);
        }

        [TestMethod]
        public async Task ChangeUserStatusAsync_UserAlreadyActive_ReturnsMessageWithoutUpdate()
        {
            var existingUser = new User
            {
                Id = 2,
                UserName = "alice",
                Email = "alice@example.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var principal = BuildClaimsPrincipal(1, new[] { "Admin" });
            _mockHttpContext.Setup(x => x.User).Returns(principal);
            _mockUserManager.SetupGet(x => x.Users).Returns(new List<User> { existingUser }.AsQueryable());
            _mockEmployeeRepository.Setup(x => x.GetByUserIdAsync(2)).ReturnsAsync((Employee?)null);
            _mockUserManager.Setup(x => x.GetRolesAsync(existingUser)).ReturnsAsync(new List<string>());

            var request = new ChangeUserStatusRequest
            {
                UserId = 2,
                IsActive = true
            };

            var result = await _userService.ChangeUserStatusAsync(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Tài khoản đã ở trạng thái hoạt động", result.Message);
            Assert.AreEqual(2, result.Data!.UserId);
            Assert.IsTrue(result.Data.IsActive);
        }

        [TestMethod]
        public async Task ChangeUserStatusAsync_DeactivateUser_ReturnsSuccess()
        {
            var existingUser = new User
            {
                Id = 2,
                UserName = "alice",
                Email = "alice@example.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var principal = BuildClaimsPrincipal(1, new[] { "Admin" });
            _mockHttpContext.Setup(x => x.User).Returns(principal);
            _mockUserManager.SetupGet(x => x.Users).Returns(new List<User> { existingUser }.AsQueryable());
            _mockEmployeeRepository.Setup(x => x.GetByUserIdAsync(2)).ReturnsAsync((Employee?)null);
            _mockUserManager.Setup(x => x.UpdateAsync(It.Is<User>(u => u.Id == 2 && u.IsActive == false)))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(existingUser)).ReturnsAsync(new List<string>());

            var request = new ChangeUserStatusRequest
            {
                UserId = 2,
                IsActive = false
            };

            var result = await _userService.ChangeUserStatusAsync(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Khóa tài khoản thành công (người dùng không thể đăng nhập)", result.Message);
            Assert.AreEqual(2, result.Data!.UserId);
            Assert.IsFalse(result.Data.IsActive);
        }

        private static ClaimsPrincipal BuildClaimsPrincipal(int userId, string[] roles = null)
        {
            roles ??= Array.Empty<string>();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        private static Mock<UserManager<User>> CreateMockUserManager()
        {
            var userStore = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(
                userStore.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object
            );
        }

        private static Mock<RoleManager<IdentityRole<int>>> CreateMockRoleManager()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole<int>>>();
            return new Mock<RoleManager<IdentityRole<int>>>(
                roleStore.Object,
                Array.Empty<IRoleValidator<IdentityRole<int>>>(),
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<IdentityRole<int>>>>().Object
            );
        }
    }
}

