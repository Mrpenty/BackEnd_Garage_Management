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

    }
}