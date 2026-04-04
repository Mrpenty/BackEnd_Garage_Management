using Garage_Management.Application.DTOs.User;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Accounts;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.Vehiclies;
using Garage_Management.UnitTest.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Customers
{
    [TestClass]
    public class CustomerServiceTests
    {
        private Mock<ICustomerRepository> _customerRepository;
        private Mock<IUserRepository> _userRepository;
        private Mock<IVehicleRepository> _vehicleRepository;
        private Mock<UserManager<User>> _userManager;
        private Mock<ILogger<CustomerService>> _logger;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private CustomerService _service;

        [TestInitialize]
        public void Setup()
        {
            _customerRepository = new Mock<ICustomerRepository>();
            _userRepository = new Mock<IUserRepository>();
            _vehicleRepository = new Mock<IVehicleRepository>();
            _logger = new Mock<ILogger<CustomerService>>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _userManager = CreateMockUserManager();

            _service = new CustomerService(
                _customerRepository.Object,
                _userRepository.Object,
                _vehicleRepository.Object,
                _userManager.Object,
                _logger.Object,
                _httpContextAccessor.Object
            );
        }

        [TestMethod]
        public async Task GetPagedAsync_ReturnsCustomersSuccessfully()
        {
            var customers = new List<Customer>
            {
                new Customer
                {
                    CustomerId = 11,
                    FirstName = "A",
                    LastName = "B",
                    Address = "X",
                    CreatedAt = DateTime.UtcNow,
                    User = new User { PhoneNumber = "0123456789", Email = "a@b.com" },
                    Vehicles = new List<Vehicle>
                    {
                        new Vehicle
                        {
                            VehicleId = 100,
                            LicensePlate = "ABC123",
                            Brand = new Base.Entities.Vehiclies.VehicleBrand { BrandName = "Toyota" },
                            Model = new Base.Entities.Vehiclies.VehicleModel { ModelName = "Camry" },
                            Year = 2020
                        }
                    }
                }
            };

            var mockQueryable = customers.AsQueryable();
            _customerRepository.Setup(x => x.GetAll()).Returns(new TestAsyncEnumerable<Customer>(mockQueryable));
            var result = await _service.GetPagedAsync(new ParamQuery { Page = 1, PageSize = 20 }, CancellationToken.None);

            Assert.IsTrue(result.Success);
            var pageData = result.Data.PageData.ToList();

            Assert.AreEqual(1, pageData.Count);
            //Assert.AreEqual("B A", pageData[0].FullName);


        }
        [TestMethod]
        public async Task GetPagedAsync_SearchByName_ReturnsFilteredResult()
        {
            var customers = new List<Customer>
            {
                new Customer { FirstName = "Anh", LastName = "Nguyen", CreatedAt = DateTime.UtcNow },
                new Customer { FirstName = "Binh", LastName = "Tran", CreatedAt = DateTime.UtcNow }
            };

            _customerRepository.Setup(x => x.GetAll()).Returns(new TestAsyncEnumerable<Customer>(customers.AsQueryable()));
            var result = await _service.GetPagedAsync(new ParamQuery { Page = 1, PageSize = 10, Search = "anh" }, CancellationToken.None);
            Assert.IsTrue(result.Success);
            var pageData = result.Data.PageData.ToList();
            Assert.AreEqual(1, pageData.Count);
        }
        [TestMethod]
        public async Task GetPagedAsync_SearchByPhone_ReturnsCorrectCustomer()
        {
            var customers = new List<Customer>
             {
                 new Customer
                 {
                     CreatedAt = DateTime.UtcNow,
                     User = new User { PhoneNumber = "0123" }
                 },
                 new Customer
                 {
                     CreatedAt = DateTime.UtcNow,
                     User = new User { PhoneNumber = "9999" }
                 }
             };
            _customerRepository.Setup(x => x.GetAll()).Returns(new TestAsyncEnumerable<Customer>(customers.AsQueryable()));
            var result = await _service.GetPagedAsync(new ParamQuery { Page = 1, PageSize = 10, Search = "0123" }, CancellationToken.None);
            Assert.IsTrue(result.Success);
            var pageData = result.Data.PageData.ToList();
            Assert.AreEqual(1, pageData.Count);
        }
        [TestMethod]
        public async Task GetPagedAsync_NoData_ReturnsEmpty()
        {
            _customerRepository.Setup(x => x.GetAll()).Returns(new TestAsyncEnumerable<Customer>(Enumerable.Empty<Customer>().AsQueryable()));
            var result = await _service.GetPagedAsync(new ParamQuery { Page = 1, PageSize = 10 }, CancellationToken.None);
            Assert.IsTrue(result.Success);
            var pageData = result.Data.PageData.ToList();
            Assert.AreEqual(0, pageData.Count);
        }
        [TestMethod]
        public async Task GetPagedAsync_ShouldSortByCreatedAtDesc()
        {
            var customers = new List<Customer>
            {
                 new Customer { CustomerId = 1, CreatedAt = DateTime.UtcNow.AddDays(-1) },
                 new Customer { CustomerId = 2, CreatedAt = DateTime.UtcNow }
            };

            _customerRepository.Setup(x => x.GetAll()).Returns(new TestAsyncEnumerable<Customer>(customers.AsQueryable()));
            var result = await _service.GetPagedAsync(new ParamQuery { Page = 1, PageSize = 10 }, CancellationToken.None);
            Assert.AreEqual(2, result.Data.PageData.First().CustomerId);
        }

        [TestMethod]
        public async Task CreateCustomerByReceptionistAsync_NotAuthenticated_ReturnsError()
        {
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) });

            var request = new CreateCustomerRequest
            {
                FirstName = "A",
                LastName = "B",
                PhoneNumber = "0987654321",
                Email = "a@b.com"
            };

            var result = await _service.CreateCustomerByReceptionistAsync(request, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Không tìm thấy thông tin người dùng đăng nhập", result.Message);
        }

        [TestMethod]
        public async Task CreateCustomerByReceptionistAsync_PhoneExists_ReturnsError()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "1") }, "Test"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            var customers = new List<Customer>
            {
                new Customer { CustomerId = 2, User = new User { PhoneNumber = "0987654321" } }
            };
            var mockQueryable = customers.AsQueryable();
            _customerRepository.Setup(x => x.GetAll()).Returns(new TestAsyncEnumerable<Customer>(mockQueryable));
            var request = new CreateCustomerRequest
            {
                FirstName = "A",
                LastName = "B",
                PhoneNumber = "0987654321",
                Email = "a@b.com"
            };

            var result = await _service.CreateCustomerByReceptionistAsync(request, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Số điện thoại này đã được đăng ký.", result.Message);
        }

        [TestMethod]
        public async Task CreateCustomerByReceptionistAsync_Success_ReturnsSuccess()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "1") }, "Test"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            var customers = Enumerable.Empty<Customer>();
            var mockQueryable = new TestAsyncEnumerable<Customer>(customers.AsQueryable());
            _customerRepository.Setup(x => x.GetAll()).Returns(mockQueryable);
            _userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "Customer")).ReturnsAsync(IdentityResult.Success);
            _customerRepository.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var request = new CreateCustomerRequest
            {
                FirstName = "A",
                LastName = "B",
                PhoneNumber = "0987654321",
                Email = "a@b.com",
                Address = "X"
            };

            var result = await _service.CreateCustomerByReceptionistAsync(request, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Tạo khách hàng thành công", result.Message);
            Assert.AreEqual("B A", result.Data.FullName);
        }

        [TestMethod]
        public async Task CreateCustomerByReceptionistAsync_MissingRequiredFields_ReturnsError()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, "1") }, "Test"));

            _httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            var request = new CreateCustomerRequest
            {
                FirstName = "",
                LastName = "B",
                PhoneNumber = ""
            };

            var result = await _service.CreateCustomerByReceptionistAsync(request, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Vui lòng nhập đầy đủ họ, tên và số điện thoại.", result.Message);
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
    }
}
