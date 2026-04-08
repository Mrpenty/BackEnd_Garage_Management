using Garage_Management.Application.Services.Accounts;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.UserTests
{
    [TestClass]
    public class GetCurrentUserProfileTests
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
        public async Task GetCurrentUserProfileAsync_InvalidClaim_ReturnsError()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity());

            var result = await _userService.GetCurrentUserProfileAsync(principal);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Không tìm thấy thông tin người dùng", result.Message);
        }

        [TestMethod]
        public async Task GetCurrentUserProfileAsync_UserNotFound_ReturnsError()
        {
            var principal = BuildClaimsPrincipal(1);
            _mockUserManager.Setup(x => x.FindByIdAsync("1")).ReturnsAsync((User?)null);

            var result = await _userService.GetCurrentUserProfileAsync(principal);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Không tìm thấy tài khoản", result.Message);
        }

        [TestMethod]
        public async Task GetCurrentUserProfileAsync_WithCustomerRole_ReturnsCustomerProfile()
        {
            var user = new User
            {
                Id = 1,
                UserName = "john.doe",
                Email = "john.doe@example.com",
                PhoneNumber = "0123456789",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };

            var principal = BuildClaimsPrincipal(1, new[] { "Customer" });

            _mockUserManager.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Customer" });
            _mockCustomerRepository.Setup(x => x.GetByUserIdAsync(1)).ReturnsAsync(new Customer
            {
                CustomerId = 10,
                FirstName = "John",
                LastName = "Doe",
                CreatedAt = DateTime.UtcNow,
                UserId = 1
            });

            var result = await _userService.GetCurrentUserProfileAsync(principal);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Lấy thông tin profile thành công", result.Message);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data!.UserId);
            Assert.AreEqual("Doe John", result.Data.FullName);
            Assert.IsNotNull(result.Data.Customer);
            Assert.AreEqual(10, result.Data.Customer!.CustomerId);
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
