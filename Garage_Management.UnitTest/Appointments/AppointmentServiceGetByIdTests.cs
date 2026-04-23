using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Appointments;
using Garage_Management.Base.Entities.Accounts;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Appointments
{
    [TestClass]
    public class AppointmentServiceGetByIdTests
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
            _vehicleModelRepo = new Mock<IVehicleModelRepository>();
            _inventoryRepo = new Mock<IInventoryRepository>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

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
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            _appointmentRepo.Setup(x => x.GetByIdWithDetailsAsync(100, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Appointment?)null);

            var result = await _service.GetByIdAsync(100, CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_InvalidId_ReturnsNull()
        {
            _appointmentRepo.Setup(x => x.GetByIdWithDetailsAsync(-1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Appointment?)null);

            var result = await _service.GetByIdAsync(-1, CancellationToken.None);

            Assert.IsNull(result);
            _appointmentRepo.Verify(x => x.GetByIdWithDetailsAsync(-1, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task GetByIdAsync_Found_ReturnsResponse()
        {
            var entity = new Appointment
            {
                AppointmentId = 1,
                FirstName = "Nguyễn",
                LastName = "Văn Minh",
                Phone = "+84912345678",
                CustomVehicleBrand = "Toyota",
                CustomVehicleModel = "Vios",
                LicensePlate = "29A-12345",
                AppointmentDateTime = new DateTime(2026, 4, 20, 9, 0, 0, DateTimeKind.Utc),
                Status = Base.Common.Enums.AppointmentStatus.Pending,
                Description = "Xe có tiếng kêu lạ khi vào số",
                Services = new List<Garage_Management.Base.Entities.Accounts.AppointmentService>(),
                SpareParts = new List<AppointmentSparePart>()
            };
            _appointmentRepo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(1, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.AppointmentId);
        }
    }
}
