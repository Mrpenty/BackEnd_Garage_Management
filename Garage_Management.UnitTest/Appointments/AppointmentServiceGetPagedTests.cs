using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Appointments;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Common.Models.Appointments;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Entities.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Appointments
{
    [TestClass]
    public class AppointmentServiceGetPagedTests
    {
        private Mock<IAppointmentRepository> _appointmentRepo;
        private Mock<IEmployeeRepository> _employeeRepo;
        private Mock<ICustomerRepository> _customerRepo;
        private Mock<IVehicleRepository> _vehicleRepo;
        private Mock<IVehicleModelRepository> _vehicleModelRepo;
        private Mock<IServiceRepository> _serviceRepo;
        private Mock<IInventoryRepository> _inventoryRepo;
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Application.Services.Appointments.AppointmentService _service;

        [TestInitialize]
        public void Setup()
        {
            _appointmentRepo = new Mock<IAppointmentRepository>();
            _employeeRepo = new Mock<IEmployeeRepository>();
            _customerRepo = new Mock<ICustomerRepository>();
            _vehicleRepo = new Mock<IVehicleRepository>();
            _vehicleModelRepo = new Mock<IVehicleModelRepository>();
            _serviceRepo = new Mock<IServiceRepository>();
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
        public async Task GetPagedAsync_Page_ReturnsPagedResult()
        {
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
                        FirstName = "Nguyễn",
                        LastName = "Văn Minh",
                        Phone = "+84987654321",
                        CustomVehicleBrand = "Toyota",
                        CustomVehicleModel = "Vios",
                        LicensePlate = "29A-12345",
                        AppointmentDateTime = new DateTime(2026, 4, 20, 9, 0, 0, DateTimeKind.Utc),
                        Status = Base.Common.Enums.AppointmentStatus.Pending,
                        Services = new List<Garage_Management.Base.Entities.Accounts.AppointmentService>(),
                        SpareParts = new List<AppointmentSparePart>()
                    }
                }
            };
            _appointmentRepo.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(paged);

            var result = await _service.GetPagedAsync(1, 10, CancellationToken.None);

            Assert.AreEqual(1, result.Total);
            Assert.AreEqual(1, result.PageData.Count());
            Assert.AreEqual(1, result.PageData.First().AppointmentId);
        }

        [TestMethod]
        public async Task GetPagedAsync_Query_ReturnsPagedResult()
        {
            var paged = new PagedResult<Appointment>
            {
                Page = 1,
                PageSize = 10,
                Total = 0,
                PageData = new List<Appointment>()
            };
            _appointmentRepo.Setup(x => x.GetPagedAsync(It.IsAny<AppointmentQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paged);

            var result = await _service.GetPagedAsync(new AppointmentQuery { Page = 1, PageSize = 10 }, CancellationToken.None);

            Assert.AreEqual(0, result.Total);
            Assert.AreEqual(0, result.PageData.Count());
        }

        [TestMethod]
        public async Task GetPagedAsync_Page_Empty_ReturnsEmptyPagedResult()
        {
            var paged = new PagedResult<Appointment>
            {
                Page = 2,
                PageSize = 5,
                Total = 0,
                PageData = new List<Appointment>()
            };
            _appointmentRepo.Setup(x => x.GetPagedAsync(2, 5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(paged);

            var result = await _service.GetPagedAsync(2, 5, CancellationToken.None);

            Assert.AreEqual(2, result.Page);
            Assert.AreEqual(5, result.PageSize);
            Assert.AreEqual(0, result.Total);
            Assert.AreEqual(0, result.PageData.Count());
        }

        [TestMethod]
        public async Task GetPagedAsync_Page_MapsServicesAndSpareParts()
        {
            var entity = new Appointment
            {
                AppointmentId = 10,
                FirstName = "Trần",
                LastName = "Quốc Bảo",
                Phone = "+84356789012",
                CustomVehicleBrand = "Honda",
                CustomVehicleModel = "City",
                LicensePlate = "51G-67890",
                AppointmentDateTime = new DateTime(2026, 4, 22, 14, 30, 0, DateTimeKind.Utc),
                Status = Base.Common.Enums.AppointmentStatus.Confirmed,
                Description = "Rửa xe và kiểm tra lọc gió",
                Services = new List<Garage_Management.Base.Entities.Accounts.AppointmentService>
                {
                    new Garage_Management.Base.Entities.Accounts.AppointmentService
                    {
                        ServiceId = 2,
                        Service = new Service
                        {
                            ServiceId = 2,
                            ServiceName = "Rửa xe",
                            ServiceTasks = new List<ServiceTask>
                            {
                                new ServiceTask { ServiceTaskId = 1, ServiceId = 2, TaskName = "Xịt nước áp lực", TaskOrder = 1, EstimateMinute = 5 },
                                new ServiceTask { ServiceTaskId = 2, ServiceId = 2, TaskName = "Lau khô và đánh bóng", TaskOrder = 2, EstimateMinute = 10 }
                            }
                        }
                    }
                },
                SpareParts = new List<AppointmentSparePart>
                {
                    new AppointmentSparePart
                    {
                        SparePartId = 7,
                        Inventory = new Inventory
                        {
                            SparePartId = 7,
                            PartName = "Lọc gió động cơ"
                        }
                    }
                }
            };

            var paged = new PagedResult<Appointment>
            {
                Page = 1,
                PageSize = 10,
                Total = 1,
                PageData = new List<Appointment> { entity }
            };

            _appointmentRepo.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(paged);

            var result = await _service.GetPagedAsync(1, 10, CancellationToken.None);

            Assert.AreEqual(1, result.Total);
            Assert.AreEqual(1, result.PageData.Count());
            Assert.AreEqual(1, result.PageData.First().Services.Count);
            Assert.AreEqual(1, result.PageData.First().SpareParts.Count);
            Assert.AreEqual(15L, result.PageData.First().TotalEstimateMinute);
        }

        [TestMethod]
        public async Task GetPagedAsync_Query_CallsRepositoryWithQuery()
        {
            var query = new AppointmentQuery
            {
                Page = 3,
                PageSize = 20,
                Search = "khanh",
                Filter = "pending"
            };

            _appointmentRepo.Setup(x => x.GetPagedAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<Appointment>
                {
                    Page = 3,
                    PageSize = 20,
                    Total = 0,
                    PageData = new List<Appointment>()
                });

            var result = await _service.GetPagedAsync(query, CancellationToken.None);

            Assert.AreEqual(3, result.Page);
            Assert.AreEqual(20, result.PageSize);
            _appointmentRepo.Verify(x => x.GetPagedAsync(query, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
