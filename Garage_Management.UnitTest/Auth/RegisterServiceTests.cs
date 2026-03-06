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
using System.Threading.Tasks;
using Garage_Management.Application.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace Garage_Management.UnitTest.Auth
{
    [TestClass]
    public class RegisterServiceTests
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
            // duplicate setup from LoginServiceTests
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

        #region RegisterCustomerAsync Tests

        [TestMethod]
        public async Task RegisterCustomerAsync_WithValidData_ReturnsSuccess()
        {
            var phone = "0987654321";
            var request = new CustomerRegisterRequest
            {
                PhoneNumber = phone,
                Password = "Password@123",
                FirstName = "John",
                LastName = "Doe"
            };

            _mockUserRepository.Setup(x => x.ExistsByPhoneNumberAsync(phone, It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(false);
            _mockSmsService.Setup(x => x.SendOtpAsync(phone)).ReturnsAsync((true, "OTP sent"));
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), "Password@123"))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "Customer"))
                .ReturnsAsync(IdentityResult.Success);
            _mockCustomerRepository.Setup(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<System.Threading.CancellationToken>())).Returns(Task.CompletedTask);
            _mockCustomerRepository.Setup(x => x.SaveAsync(It.IsAny<System.Threading.CancellationToken>())).Returns((Task<int>)Task.CompletedTask);

            var result = await _authService.RegisterCustomerAsync(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Tạo tài khoản thành công", result.Message);
            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<User>(), "Password@123"), Times.Once);
        }

        [TestMethod]
        public async Task RegisterCustomerAsync_WithExistingPhone_ReturnsFailed()
        {
            var phone = "0987654321";
            var request = new CustomerRegisterRequest
            {
                PhoneNumber = phone,
                Password = "Password123"
            };

            _mockUserRepository.Setup(x => x.ExistsByPhoneNumberAsync(phone, It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(true);

            var result = await _authService.RegisterCustomerAsync(request);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Message.Contains("đã được đăng ký"));
        }

        [TestMethod]
        public async Task RegisterCustomerAsync_WithWeakPassword_ReturnsFailed()
        {
            var phone = "0987654321";
            var request = new CustomerRegisterRequest
            {
                PhoneNumber = phone,
                Password = "Pass1"
            };

            _mockUserRepository.Setup(x => x.ExistsByPhoneNumberAsync(phone, It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(false);

            var result = await _authService.RegisterCustomerAsync(request);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Message.Contains("ít nhất 9 ký tự"));
        }

        [TestMethod]
        public async Task RegisterCustomerAsync_WithoutUppercaseInPassword_ReturnsFailed()
        {
            var phone = "0987654321";
            var request = new CustomerRegisterRequest
            {
                PhoneNumber = phone,
                Password = "password123"
            };

            _mockUserRepository.Setup(x => x.ExistsByPhoneNumberAsync(phone, It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(false);

            var result = await _authService.RegisterCustomerAsync(request);

            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task RegisterCustomerAsync_WithSmsFailure_ReturnsFailed()
        {
            var phone = "0987654321";
            var request = new CustomerRegisterRequest
            {
                PhoneNumber = phone,
                Password = "Password123"
            };

            _mockUserRepository.Setup(x => x.ExistsByPhoneNumberAsync(phone, It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(false);
            _mockSmsService.Setup(x => x.SendOtpAsync(phone)).ReturnsAsync((false, "SMS service failed"));

            var result = await _authService.RegisterCustomerAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("SMS service failed", result.Message);
        }

        #endregion
    }

}
