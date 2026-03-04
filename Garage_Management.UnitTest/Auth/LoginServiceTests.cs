using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Garage_Management.Application.DTOs.Auth;
using Garage_Management.Application.Services.Accounts;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Interface;
using Garage_Management.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Garage_Management.Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Garage_Management.UnitTest.Auth
{
    [TestClass]
    public class LoginServiceTests
    {
        private Mock<UserManager<User>> _mockUserManager;
        private Mock<SignInManager<User>> _mockSignInManager;
        private Mock<RoleManager<IdentityRole<int>>> _mockRoleManager;
        private Mock<IGenerateToken> _mockTokenGenerator;
        private Mock<ITokenCookieService> _mockTokenCookieService;
        private Mock<ISmsService> _mockSmsService;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<ICustomerRepository> _mockCustomerRepository;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private AuthService _authService;

      
        [TestInitialize]
        public void Setup()
        {
            _mockUserManager = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[0],
                new IPasswordValidator<User>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<Microsoft.Extensions.Logging.ILogger<UserManager<User>>>().Object
            );

            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<Microsoft.Extensions.Logging.ILogger<SignInManager<User>>>().Object,
                new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<User>>().Object
            );

            _mockRoleManager = new Mock<RoleManager<IdentityRole<int>>>(
                new Mock<IRoleStore<IdentityRole<int>>>().Object,
                new IRoleValidator<IdentityRole<int>>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<Microsoft.Extensions.Logging.ILogger<RoleManager<IdentityRole<int>>>>().Object
            );

            _mockTokenGenerator = new Mock<IGenerateToken>();
            _mockTokenCookieService = new Mock<ITokenCookieService>();
            _mockSmsService = new Mock<ISmsService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            var mockHttpContext = new Mock<HttpContext>();
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

            _authService = new AuthService(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockRoleManager.Object,
                _mockTokenGenerator.Object,
                _mockTokenCookieService.Object,
                _mockSmsService.Object,
                _mockConfiguration.Object,
                _mockUserRepository.Object,
                _mockCustomerRepository.Object,
                _mockHttpContextAccessor.Object
            );
        }

        #region LoginCustomerAsync Tests

        [TestMethod]
        public async Task LoginCustomerAsync_WithValidCredentials_ReturnsSuccess()
        {
            var phoneNumber = "0987654312";
            var request = new CustomerLoginRequest { PhoneNumber = phoneNumber, UseOtp = false, Password = "Khach@123!" };
            var user = new User { Id = 1, PhoneNumber = phoneNumber, IsActive = true };

            //_mockUserManager.Setup(x => x.FindByEmailAsync(phoneNumber)).ReturnsAsync((User)null);
            _mockUserManager.Setup(x => x.Users).Returns(ToAsyncQueryable(new List<User> { user }));

            _mockSignInManager.Setup(x => x.PasswordSignInAsync(user, "Khach@123!", false, false))
                .ReturnsAsync(SignInResult.Success);
            _mockTokenGenerator.Setup(x => x.GenerateJwtTokenAsync(user)).ReturnsAsync("test-token");
            _mockTokenGenerator.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");
            _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Customer" });
            _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            var result = await _authService.LoginCustomerAsync(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Đăng nhập thành công", result.Message);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual("test-token", result.Data.AccessToken);
            Assert.AreEqual("refresh-token", result.Data.RefreshToken);
        }

        [TestMethod]
        public async Task LoginCustomerAsync_WithInactiveUser_ReturnsFailed()
        {
            var phoneNumber = "0987654312";
            var request = new CustomerLoginRequest { PhoneNumber = phoneNumber, UseOtp = false, Password = "Khach@123!" };
            var user = new User { Id = 1, PhoneNumber = phoneNumber, IsActive = false };

            _mockUserManager.Setup(x => x.FindByEmailAsync(phoneNumber)).ReturnsAsync((User)null);
            _mockUserManager.Setup(x => x.Users).Returns(ToAsyncQueryable(new List<User> { user }));

            var result = await _authService.LoginCustomerAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Tài khoản của bạn đã bị khóa.", result.Message);
        }

        [TestMethod]
        public async Task LoginCustomerAsync_WithNonExistentUser_ReturnsFailed()
        {
            var phoneNumber = "0987654321";
            var request = new CustomerLoginRequest { PhoneNumber = phoneNumber };

            _mockUserManager.Setup(x => x.FindByEmailAsync(phoneNumber)).ReturnsAsync((User)null);
            _mockUserManager.Setup(x => x.Users).Returns(ToAsyncQueryable(new List<User>()));

            var result = await _authService.LoginCustomerAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Tài khoản không tồn tại", result.Message);
        }

        [TestMethod]
        public async Task LoginCustomerAsync_WithInvalidPassword_ReturnsFailed()
        {
            var phoneNumber = "0987654321";
            var request = new CustomerLoginRequest { PhoneNumber = phoneNumber, UseOtp = false, Password = "WrongPassword" };
            var user = new User { Id = 1, PhoneNumber = phoneNumber, IsActive = true };

            _mockUserManager.Setup(x => x.FindByEmailAsync(phoneNumber)).ReturnsAsync((User)null);
            _mockUserManager.Setup(x => x.Users).Returns(ToAsyncQueryable(new List<User> { user }));
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(user, "WrongPassword", false, false))
                .ReturnsAsync(SignInResult.Failed);

            var result = await _authService.LoginCustomerAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("MSG02", result.Message);
        }

        [TestMethod]
        public async Task LoginCustomerAsync_WithEmptyPassword_ReturnsFailed()
        {
            var phoneNumber = "0987654321";
            var request = new CustomerLoginRequest { PhoneNumber = phoneNumber, UseOtp = false, Password = "" };
            var user = new User { Id = 1, PhoneNumber = phoneNumber, IsActive = true };

            _mockUserManager.Setup(x => x.FindByEmailAsync(phoneNumber)).ReturnsAsync((User)null);
            _mockUserManager.Setup(x => x.Users).Returns(ToAsyncQueryable(new List<User> { user }));

            var result = await _authService.LoginCustomerAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("MSG01", result.Message);
        }

        #endregion

        #region LoginStaffAsync Tests

        [TestMethod]
        public async Task LoginStaffAsync_WithValidCredentials_ReturnsSuccess()
        {
            var email = "staff@test.com";
            var password = "Password123";
            var request = new StaffLoginRequest { Email = email, Password = password };
            var user = new User { Id = 1, Email = email, IsActive = true };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Manager" });
            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, password, true))
                .ReturnsAsync(SignInResult.Success);
            _mockTokenGenerator.Setup(x => x.GenerateJwtTokenAsync(user)).ReturnsAsync("test-token");
            _mockTokenGenerator.Setup(x => x.GenerateRefreshToken()).Returns("refresh-token");
            _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            var result = await _authService.LoginStaffAsync(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Đăng nhập thành công", result.Message);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual("test-token", result.Data.AccessToken);
        }

        [TestMethod]
        public async Task LoginStaffAsync_WithCustomerRole_ReturnsFailed()
        {
            var email = "customer@test.com";
            var password = "Password123";
            var request = new StaffLoginRequest { Email = email, Password = password };
            var user = new User { Id = 1, Email = email, IsActive = true };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Customer" });

            var result = await _authService.LoginStaffAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Tài khoản không có quyền truy cập khu vực nhân viên", result.Message);
        }

        [TestMethod]
        public async Task LoginStaffAsync_WithInactiveUser_ReturnsFailed()
        {
            var email = "staff@test.com";
            var password = "Password123";
            var request = new StaffLoginRequest { Email = email, Password = password };
            var user = new User { Id = 1, Email = email, IsActive = false };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);

            var result = await _authService.LoginStaffAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Tài khoản của bạn đã bị khóa.", result.Message);
        }

        [TestMethod]
        public async Task LoginStaffAsync_WithNullRequest_ThrowsException()
        {
            StaffLoginRequest request = null;
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _authService.LoginStaffAsync(request));
        }

        [TestMethod]
        public async Task LoginStaffAsync_WithMissingEmail_ReturnsFailed()
        {
            var request = new StaffLoginRequest { Email = "", Password = "Password123" };

            var result = await _authService.LoginStaffAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Hãy nhập đầy đủ các trường thiếu", result.Message);
        }

        #endregion
    }
}
