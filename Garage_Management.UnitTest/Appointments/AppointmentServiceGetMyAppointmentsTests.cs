using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Appointments;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
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

namespace Garage_Management.UnitTest.Appointments
{
    [TestClass]
    public class AppointmentServiceGetMyAppointmentsTests
    {
        private Mock<IAppointmentRepository> _appointmentRepo;
        private Mock<IEmployeeRepository> _employeeRepo;
        private Mock<ICustomerRepository> _customerRepo;
        private Mock<IVehicleRepository> _vehicleRepo;
        private Mock<IServiceRepository> _serviceRepo;
        private Mock<IInventoryRepository> _inventoryRepo;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Application.Services.Appointments.AppointmentService _service;
        private Mock<IVehicleModelRepository> _vehicleModelRepo;

        [TestInitialize]
        public void Setup()
        {
            _appointmentRepo = new Mock<IAppointmentRepository>();
            _employeeRepo = new Mock<IEmployeeRepository>();
            _customerRepo = new Mock<ICustomerRepository>();
            _vehicleRepo = new Mock<IVehicleRepository>();
            _serviceRepo = new Mock<IServiceRepository>();
            _inventoryRepo = new Mock<IInventoryRepository>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _vehicleModelRepo = new Mock<IVehicleModelRepository>();

            var mockHttpContext = new Mock<HttpContext>();
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

            _service = new Application.Services.Appointments.AppointmentService(
                _appointmentRepo.Object,
                _employeeRepo.Object,
                _customerRepo.Object,
                _vehicleRepo.Object,
                _vehicleModelRepo.Object,
                _serviceRepo.Object,
                _inventoryRepo.Object,
                _httpContextAccessor.Object);
        }

        [TestMethod]
        public async Task GetMyAppointmentsAsync_NotAuthenticated_ReturnsError()
        {
            var httpContext = new DefaultHttpContext();
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            var result = await _service.GetMyAppointmentsAsync(1, 10, CancellationToken.None);

            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task GetMyAppointmentsAsync_AsCustomer_ReturnsPaged()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "10"),
                new Claim(ClaimTypes.Role, "Customer")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = principal };
            _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

            var customers = new List<Customer>
            {
                new Customer { CustomerId = 1, UserId = 10, FirstName = "Nguyễn", LastName = "Văn An" }
            }.AsQueryable();
            var asyncCustomers = new TestAsyncEnumerable<Customer>(customers);
            _customerRepo.Setup(x => x.GetAll()).Returns(asyncCustomers);

            var paged = new PagedResult<Appointment>
            {
                Page = 1,
                PageSize = 10,
                Total = 1,
                PageData = new List<Appointment>
                {
                    new Appointment
                    {
                        AppointmentId = 1,
                        CustomerId = 1,
                        FirstName = "Nguyễn",
                        LastName = "Văn An",
                        Phone = "+84912345678",
                        CustomVehicleBrand = "Toyota",
                        CustomVehicleModel = "Vios",
                        LicensePlate = "29A-12345",
                        AppointmentDateTime = new DateTime(2026, 4, 20, 9, 0, 0, DateTimeKind.Utc),
                        Status = Base.Common.Enums.AppointmentStatus.Pending,
                        Description = "Bảo dưỡng định kỳ 10.000km",
                        Services = new List<Garage_Management.Base.Entities.Accounts.AppointmentService>(),
                        SpareParts = new List<AppointmentSparePart>()
                    }
                }
            };
            _appointmentRepo.Setup(x => x.GetByCustomerIdAsync(1, 10, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(paged);

            var result = await _service.GetMyAppointmentsAsync(1, 10, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.Data.Total);
        }
    }
}
