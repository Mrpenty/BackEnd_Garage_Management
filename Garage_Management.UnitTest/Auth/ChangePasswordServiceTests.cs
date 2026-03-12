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
using System.Security.Claims;
using System.Threading.Tasks;
using Garage_Management.Application.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace Garage_Management.UnitTest.Auth
{
    [TestClass]
    public class ChangePasswordServiceTests
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

            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpContext = new Mock<HttpContext>();
            _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object,
                _mockHttpContextAccessor.Object,
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

        [TestMethod]
        public async Task ChangePasswordAsync_WithValidData_ReturnsSuccess()
        {
            var userId = "1";
            var oldPassword = "OldPassword123";
            var newPassword = "NewPassword123";
            var request = new ChangePasswordRequest
            {
                OldPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmPassword = newPassword
            };
            var user = new User { Id = 1, IsActive = true };

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));

            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, oldPassword)).ReturnsAsync(true);
            _mockUserManager.Setup(x => x.ChangePasswordAsync(user, oldPassword, newPassword)).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            var result = await _authService.ChangePasswordAsync(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Đổi mật khẩu thành công", result.Message);
        }

        [TestMethod]
        public async Task ChangePasswordAsync_WithIncorrectOldPassword_ReturnsFailed()
        {
            var userId = "1";
            var oldPassword = "WrongPassword";
            var newPassword = "NewPassword123";
            var request = new ChangePasswordRequest
            {
                OldPassword = oldPassword,
                NewPassword = newPassword,
                ConfirmPassword = newPassword
            };
            var user = new User { Id = 1, IsActive = true };

            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            _mockUserManager.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, oldPassword)).ReturnsAsync(false);

            var result = await _authService.ChangePasswordAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Mật khẩu hiện tại không chính xác", result.Message);
        }

        [TestMethod]
        public async Task ChangePasswordAsync_WithMismatchedPasswords_ReturnsFailed()
        {
            var request = new ChangePasswordRequest
            {
                OldPassword = "OldPassword123",
                NewPassword = "NewPassword123",
                ConfirmPassword = "DifferentPassword"
            };

            var result = await _authService.ChangePasswordAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Mật khẩu xác nhận không khớp", result.Message);
        }

        [TestMethod]
        public async Task ChangePasswordAsync_WithUnloggedUser_ReturnsFailed()
        {
            var request = new ChangePasswordRequest
            {
                OldPassword = "OldPassword123",
                NewPassword = "NewPassword123",
                ConfirmPassword = "NewPassword123"
            };

            _mockUserManager.Setup(x => x.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns((string)null);

            var result = await _authService.ChangePasswordAsync(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Người dùng chưa đăng nhập", result.Message);
        }
    }
}