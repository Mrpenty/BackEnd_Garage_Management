using Garage_Management.Application.DTOs.Vehicles;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Vehicles;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Vehicles
{
    [TestClass]
    public class VehicleServiceCreateTests
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

            _service = new VehicleService(_repo.Object, _customerRepo.Object, _modelRepo.Object, _httpContextAccessor.Object);
        }

        [TestMethod]
        public async Task CreateAsync_InvalidCustomerId_Throws()
        {
            var request = new VehicleCreateRequest { CustomerId = 0, ModelId = 1 };

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_CustomerNotFound_Throws()
        {
            var request = new VehicleCreateRequest { CustomerId = 99, ModelId = 1 };
            _customerRepo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Customer?)null);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_InvalidModelId_Throws()
        {
            var request = new VehicleCreateRequest { CustomerId = 1, ModelId = 0 };
            _customerRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Customer { CustomerId = 1 });

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_ModelNotFound_Throws()
        {
            var request = new VehicleCreateRequest { CustomerId = 1, ModelId = 999 };
            _customerRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Customer { CustomerId = 1 });
            _modelRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((VehicleModel?)null);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_ValidRequest_WithAuthUser_SetsCreatedBy()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "10") };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var request = new VehicleCreateRequest
            {
                CustomerId = 1,
                ModelId = 2,
                LicensePlate = "29A12345",
                Year = 2022,
                Vin = "VIN001"
            };

            _customerRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Customer { CustomerId = 1 });
            _modelRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new VehicleModel { ModelId = 2, BrandId = 5 });
            _repo.Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
                .Callback<Vehicle, CancellationToken>((v, _) => v.VehicleId = 99)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(99, result.VehicleId);
            Assert.AreEqual(1, result.CustomerId);
            Assert.AreEqual(2, result.ModelId);
            Assert.AreEqual(10, result.CreatedBy);
            _repo.Verify(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateAsync_NoHttpContext_CreatedByIsNull()
        {
            _httpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

            var request = new VehicleCreateRequest
            {
                CustomerId = 1,
                ModelId = 2,
                LicensePlate = "29A12345"
            };

            _customerRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Customer { CustomerId = 1 });
            _modelRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new VehicleModel { ModelId = 2, BrandId = 5 });

            Vehicle? captured = null;
            _repo.Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
                .Callback<Vehicle, CancellationToken>((v, _) =>
                {
                    captured = v;
                    v.VehicleId = 101;
                })
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsNotNull(captured);
            Assert.IsNull(captured!.CreatedBy);
        }

        [TestMethod]
        public async Task CreateAsync_NameIdentifierNotNumber_CreatedByIsNull()
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "abc") };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = principal });

            var request = new VehicleCreateRequest
            {
                CustomerId = 1,
                ModelId = 2,
                LicensePlate = "29A12345"
            };

            _customerRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Customer { CustomerId = 1 });
            _modelRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new VehicleModel { ModelId = 2, BrandId = 5 });

            Vehicle? captured = null;
            _repo.Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
                .Callback<Vehicle, CancellationToken>((v, _) =>
                {
                    captured = v;
                    v.VehicleId = 102;
                })
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsNotNull(captured);
            Assert.IsNull(captured!.CreatedBy);
        }

        [TestMethod]
        public async Task CreateAsync_OptionalFieldsNull_StillCreates()
        {
            var request = new VehicleCreateRequest
            {
                CustomerId = 1,
                ModelId = 2,
                LicensePlate = null,
                Year = null,
                Vin = null
            };

            _customerRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Customer { CustomerId = 1 });
            _modelRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new VehicleModel { ModelId = 2, BrandId = 5 });

            Vehicle? captured = null;
            _repo.Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
                .Callback<Vehicle, CancellationToken>((v, _) =>
                {
                    captured = v;
                    v.VehicleId = 103;
                })
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsNotNull(captured);
            Assert.IsNull(captured!.LicensePlate);
            Assert.IsNull(captured.Year);
            Assert.IsNull(captured.Vin);
        }

        [TestMethod]
        public async Task CreateAsync_BrandIdComesFromModel()
        {
            var request = new VehicleCreateRequest
            {
                CustomerId = 1,
                ModelId = 2,
                LicensePlate = "29A12345"
            };

            _customerRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Customer { CustomerId = 1 });
            _modelRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new VehicleModel { ModelId = 2, BrandId = 77 });

            Vehicle? captured = null;
            _repo.Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
                .Callback<Vehicle, CancellationToken>((v, _) =>
                {
                    captured = v;
                    v.VehicleId = 104;
                })
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsNotNull(captured);
            Assert.AreEqual(77, captured!.BrandId);
        }

        [TestMethod]
        public async Task CreateAsync_ValidRequest_SetsCreatedAt()
        {
            var request = new VehicleCreateRequest
            {
                CustomerId = 1,
                ModelId = 2,
                LicensePlate = "29A12345"
            };

            _customerRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Customer { CustomerId = 1 });
            _modelRepo.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new VehicleModel { ModelId = 2, BrandId = 5 });

            Vehicle? captured = null;
            _repo.Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
                .Callback<Vehicle, CancellationToken>((v, _) =>
                {
                    captured = v;
                    v.VehicleId = 105;
                })
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.IsNotNull(captured);
            Assert.AreNotEqual(default, captured!.CreatedAt);
        }
    }
}
