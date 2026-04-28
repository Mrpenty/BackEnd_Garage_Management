using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Vehicles;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.Vehiclies;
using Garage_Management.UnitTest.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Vehicles
{
    [TestClass]
    public class VehicleServiceGetMyVehicleTests
    {
        private Mock<IVehicleRepository> _repo = null!;
        private Mock<ICustomerRepository> _customerRepo = null!;
        private Mock<IVehicleModelRepository> _modelRepo = null!;
        private Mock<IHttpContextAccessor> _httpContextAccessor = null!;
        private VehicleService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IVehicleRepository>();
            _customerRepo = new Mock<ICustomerRepository>();
            _modelRepo = new Mock<IVehicleModelRepository>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _service = new VehicleService(
                _repo.Object,
                _customerRepo.Object,
                _modelRepo.Object,
                _httpContextAccessor.Object);
        }

        [TestMethod]
        public async Task GetMyVehicle_NotAuthenticated_ReturnsError()
        {
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());

            var result = await _service.GetMyVehicle(1, 10, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Vui lòng đăng nhập để xem danh sách xe máy", result.Message);
        }

        [TestMethod]
        public async Task GetMyVehicle_NameIdentifierNotNumber_ReturnsError()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "abc"),
                new Claim(ClaimTypes.Role, "Customer")
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = await _service.GetMyVehicle(1, 10, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Không thể xác định thông tin người dùng", result.Message);
        }

        [TestMethod]
        public async Task GetMyVehicle_NotCustomerRole_ReturnsError()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "10"),
                new Claim(ClaimTypes.Role, "Receptionist")
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var result = await _service.GetMyVehicle(1, 10, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("khách hàng chỉ có thể xem danh sách xe máy cá nhân", result.Message);
        }

        [TestMethod]
        public async Task GetMyVehicle_CustomerNotFound_ReturnsEmptyWithFriendlyMessage()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "10"),
                new Claim(ClaimTypes.Role, "Customer")
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = principal });

            _customerRepo.Setup(x => x.GetAll()).Returns(new TestAsyncEnumerable<Customer>(new List<Customer>().AsQueryable()));

            var result = await _service.GetMyVehicle(1, 10, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Bạn chưa có thông tin khách hàng nào trong hệ thống", result.Message);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(0, result.Data.Total);
            Assert.AreEqual(0, result.Data.PageData.Count());
        }

        [TestMethod]
        public async Task GetMyVehicle_RepoReturnsEmpty_ReturnsEmptyMessage()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "10"),
                new Claim(ClaimTypes.Role, "Customer")
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var customers = new List<Customer> { new Customer { CustomerId = 1, UserId = 10 } }.AsQueryable();
            _customerRepo.Setup(x => x.GetAll()).Returns(new TestAsyncEnumerable<Customer>(customers));

            _repo.Setup(x => x.GetByCustomerIdAsync(1, 10, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<Vehicle>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 0,
                    PageData = new List<Vehicle>()
                });

            var result = await _service.GetMyVehicle(1, 10, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Bạn chưa có phương tiện nào", result.Message);
            Assert.AreEqual(0, result.Data.Total);
            _repo.Verify(x => x.GetByCustomerIdAsync(1, 10, 1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task GetMyVehicle_AsCustomer_ReturnsPagedData()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "10"),
                new Claim(ClaimTypes.Role, "Customer")
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var customers = new List<Customer>
            {
                new Customer { CustomerId = 1, UserId = 10 }
            }.AsQueryable();
            _customerRepo.Setup(x => x.GetAll()).Returns(new TestAsyncEnumerable<Customer>(customers));

            var paged = new PagedResult<Vehicle>
            {
                Page = 1,
                PageSize = 10,
                Total = 1,
                PageData = new List<Vehicle>
                {
                    new Vehicle
                    {
                        VehicleId = 1,
                        CustomerId = 1,
                        ModelId = 2,
                        Brand = new VehicleBrand { BrandName = "Honda" },
                        Model = new VehicleModel { ModelName = "Wave Alpha" }
                    }
                }
            };
            _repo.Setup(x => x.GetByCustomerIdAsync(1, 10, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(paged);

            var result = await _service.GetMyVehicle(1, 10, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.Data.Total);
            Assert.AreEqual(1, result.Data.PageData.Count());
            Assert.AreEqual("Lấy danh sách phương tiện thành công", result.Message);
            _repo.Verify(x => x.GetByCustomerIdAsync(1, 10, 1, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
