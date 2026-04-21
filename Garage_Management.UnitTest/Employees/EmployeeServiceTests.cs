using Garage_Management.Application.DTOs.Employee;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Accounts;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.UnitTest.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;


namespace Garage_Management.UnitTest.Employees
{
    [TestClass]
    public class EmployeeServiceTests
    {
        private Mock<IEmployeeRepository> _employeeRepo;
        private Mock<IUserRepository> _userRepo;
        private Mock<UserManager<User>> _userManager;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Mock<RoleManager<IdentityRole<int>>> _roleManager;
        private EmployeeService _service;

        [TestInitialize]
        public void Setup()
        {
            _employeeRepo = new Mock<IEmployeeRepository>();
            _userRepo = new Mock<IUserRepository>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _roleManager = CreateMockRoleManager();
            _userManager = CreateMockUserManager();

            _service = new EmployeeService(
                _employeeRepo.Object,
                _userRepo.Object,
                _userManager.Object,
                _httpContextAccessor.Object,
                _roleManager.Object
            );
        }

        [TestMethod]
        public async Task CreateEmployeeAsync_NotAdmin_ReturnsError()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "5"),
                new Claim(ClaimTypes.Role, "User")
            }, "Test"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            var request = new CreateEmployeeRequest
            {
                FirstName = "A",
                LastName = "B",
                Role = "Mechanic",
                Email = "a@b.com",
                PhoneNumber = "0987654321",
                UserName = "user1",
                BranchId = 1
            };

            var result = await _service.CreateEmployeeAsync(request, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Chỉ Admin mới được tạo nhân viên", result.Message);
        }

        [TestMethod]
        public async Task CreateEmployeeAsync_MissingRequiredFields_ReturnsError()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "Test"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            var request = new CreateEmployeeRequest
            {
                FirstName = "",
                LastName = "B",
                Role = "",
                Email = "a@b.com",
                PhoneNumber = "0987654321",
                UserName = "user1",
                BranchId = 1
            };

            var result = await _service.CreateEmployeeAsync(request, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Vui lòng nhập đầy đủ thông tin bắt buộc ", result.Message);
        }

        [TestMethod]
        public async Task CreateEmployeeAsync_RoleNotExists_ReturnsError()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "Test"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            _userRepo.Setup(x => x.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _userRepo.Setup(x => x.ExistsByPhoneNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _roleManager.Setup(x => x.RoleExistsAsync("MECHANIC")).ReturnsAsync(false);

            var request = new CreateEmployeeRequest
            {
                FirstName = "A",
                LastName = "B",
                Role = "Mechanic",
                Email = "a@b.com",
                PhoneNumber = "0987654321",
                UserName = "user1",
                BranchId = 1
            };

            var result = await _service.CreateEmployeeAsync(request, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Role 'Mechanic' không tồn tại trong hệ thống", result.Message);
        }

        [TestMethod]
        public async Task CreateEmployeeAsync_Success_ReturnsSuccess()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "Test"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            _userRepo.Setup(x => x.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _userRepo.Setup(x => x.ExistsByPhoneNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _roleManager.Setup(x => x.RoleExistsAsync("MECHANIC")).ReturnsAsync(true);
            _userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success)
                .Callback<User, string>((u, p) => u.Id = 100);
            _userManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "MECHANIC")).ReturnsAsync(IdentityResult.Success);
            var employee = Enumerable.Empty<Employee>();
            var mockQueryable = new TestAsyncEnumerable<Employee>(employee.AsQueryable());
            _employeeRepo.Setup(x => x.GetAll()).Returns(mockQueryable);
            _employeeRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var request = new CreateEmployeeRequest
            {
                FirstName = "A",
                LastName = "B",
                Role = "Mechanic",
                Email = "a@b.com",
                PhoneNumber = "0987654321",
                UserName = "user1",
                BranchId = 1
            };

            var result = await _service.CreateEmployeeAsync(request, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Tạo nhân viên thành công", result.Message);
            Assert.AreEqual("B A", result.Data.FullName);
            Assert.AreEqual(100, result.Data.UserId);
        }

        private Mock<UserManager<User>> CreateMockUserManager()
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

        private Mock<RoleManager<IdentityRole<int>>> CreateMockRoleManager()
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
