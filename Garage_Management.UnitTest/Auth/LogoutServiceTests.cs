using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Garage_Management.Application.Services.Accounts;
using Garage_Management.Base.Interface;
using Garage_Management.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Garage_Management.Base.Entities.Accounts;
using System;
using Microsoft.Extensions.Options;
using Garage_Management.Application.Interfaces.Services;

namespace Garage_Management.UnitTest.Auth
{
    [TestClass]
    public class LogoutServiceTests
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

        [TestMethod]
        public async Task LogoutAsync_WhenCalled_ReturnsSuccess()
        {
            _mockTokenCookieService.Setup(x => x.DeleteTokenCookie(It.IsAny<HttpContext>()));
            _mockSignInManager.Setup(x => x.SignOutAsync()).Returns(Task.CompletedTask);

            var result = await _authService.LogoutAsync();

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Thành công", result.Message);
            _mockTokenCookieService.Verify(x => x.DeleteTokenCookie(It.IsAny<HttpContext>()), Times.Once);
        }
    }
}