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
using System.Threading.Tasks;
using Garage_Management.Application.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace Garage_Management.UnitTest.Auth
{
    [TestClass]
    public class OtpServiceTests
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
            // reusing same setup pattern
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

        #region SendOtpLoginAsync Tests

        [TestMethod]
        public async Task SendOtpLoginAsync_WithValidPhoneNumber_ReturnsSuccess()
        {
            var phone = "0987654321";
            var user = new User { Id = 1, PhoneNumber = phone };

            _mockUserManager.Setup(x => x.FindByEmailAsync(phone)).ReturnsAsync((User)null);
            _mockUserManager.Setup(x => x.Users).Returns(new List<User> { user }.AsQueryable());
            _mockSmsService.Setup(x => x.SendOtpAsync(phone)).ReturnsAsync((true, "OTP sent"));

            var result = await _authService.SendOtpLoginAsync(phone);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Đã gửi mã OTP thành công", result.Message);
        }

        [TestMethod]
        public async Task SendOtpLoginAsync_WithNonExistentUser_ReturnsFailed()
        {
            var phone = "0987654321";

            _mockUserManager.Setup(x => x.FindByEmailAsync(phone)).ReturnsAsync((User)null);
            _mockUserManager.Setup(x => x.Users).Returns(new List<User>().AsQueryable());

            var result = await _authService.SendOtpLoginAsync(phone);

            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task SendOtpLoginAsync_WithNullPhoneOrEmail_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _authService.SendOtpLoginAsync(null));
        }

        #endregion

        #region VerifyOtpAndActivateAsync Tests

        [TestMethod]
        public async Task VerifyOtpAndActivateAsync_WithValidOtp_ReturnsSuccess()
        {
            var userId = 1;
            var phone = "0987654321";
            var otp = "123456";
            var request = new VerifyOtpRequest { UserId = userId, Otp = otp };
            var user = new User { Id = userId, PhoneNumber = phone, IsActive = false, PhoneNumberConfirmed = false };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _mockSmsService.Setup(x => x.VerifyOtpAsync(It.IsAny<string>(), otp))
                .ReturnsAsync((true, "Valid"));
            _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            var result = await _authService.VerifyOtpAndActivateAsync(request);

            Assert.IsTrue(result.Success);
            Assert.IsTrue(user.IsActive);
            Assert.IsTrue(user.PhoneNumberConfirmed);
        }

        [TestMethod]
        public async Task VerifyOtpAndActivateAsync_WithInvalidOtp_ReturnsFailed()
        {
            var userId = 1;
            var phone = "0987654321";
            var otp = "wrong";
            var request = new VerifyOtpRequest { UserId = userId, Otp = otp };
            var user = new User { Id = userId, PhoneNumber = phone, IsActive = false };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _mockSmsService.Setup(x => x.VerifyOtpAsync(It.IsAny<string>(), otp))
                .ReturnsAsync((false, "Invalid OTP"));

            var result = await _authService.VerifyOtpAndActivateAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Invalid OTP", result.Message);
        }

        [TestMethod]
        public async Task VerifyOtpAndActivateAsync_WithNonExistentUser_ReturnsFailed()
        {
            var request = new VerifyOtpRequest { UserId = 999 };
            _mockUserManager.Setup(x => x.FindByIdAsync("999")).ReturnsAsync((User)null);

            var result = await _authService.VerifyOtpAndActivateAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Không tìm thấy tài khoản", result.Message);
        }

        [TestMethod]
        public async Task VerifyOtpAndActivateAsync_WithAlreadyActiveUser_ReturnsFailed()
        {
            var userId = 1;
            var request = new VerifyOtpRequest { UserId = userId };
            var user = new User { Id = userId, IsActive = true };
            _mockUserManager.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);

            var result = await _authService.VerifyOtpAndActivateAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Tài khoản đã được kích hoạt", result.Message);
        }

        #endregion

        #region ResendOtpAsync Tests

        [TestMethod]
        public async Task ResendOtpAsync_WithValidPhone_ReturnsSuccess()
        {
            var phone = "0987654321";
            var request = new ResendOtpRequest { PhoneNumber = phone };

            _mockSmsService.Setup(x => x.SendOtpAsync(phone)).ReturnsAsync((true, "OTP sent"));

            var result = await _authService.ResendOtpAsync(request);

            Assert.IsTrue(result.Success);
            //Assert.AreEqual("Đã gửi lại mã OTP thành công", result.Message);
        }

        [TestMethod]
        public async Task ResendOtpAsync_WithSmsFailure_ReturnsFailed()
        {
            var phone = "0987654321";
            var request = new ResendOtpRequest { PhoneNumber = phone };

            _mockSmsService.Setup(x => x.SendOtpAsync(phone)).ReturnsAsync((false, "SMS error"));

            var result = await _authService.ResendOtpAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("SMS error", result.Message);
        }

        [TestMethod]
        public async Task ResendOtpAsync_WithNullRequest_ThrowsException()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => _authService.ResendOtpAsync(null));
        }

        #endregion


    }
}