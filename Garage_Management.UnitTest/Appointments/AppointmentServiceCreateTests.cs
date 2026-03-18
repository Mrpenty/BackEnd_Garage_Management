using Garage_Management.Application.DTOs.Appointments;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Appointments;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Entities.Services;
using Garage_Management.UnitTest.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Appointments
{
    [TestClass]
    public class AppointmentServiceCreateTests
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
        public async Task CreateAsync_WithValidGuest_ReturnsResponse()
        {
            var request = new AppointmentCreateRequest
            {
                CustomerId = null,
                FirstName = "Khanh",
                LastName = "Do",
                Phone = "0912345678",
                CustomVehicleBrand = "Honda",
                CustomVehicleModel = "Vision",
                LicensePlate = "29A-12345",
                AppointmentDateTime = DateTime.UtcNow,
                ServiceIds = new List<int> { 1, 2 },
                SparePartsIds = new List<int> { 10 }
            };

            var services = new List<Service>
            {
                new Service { ServiceId = 1 },
                new Service { ServiceId = 2 }
            }.AsQueryable();
            var asyncServices = new TestAsyncEnumerable<Service>(services);
            _serviceRepo.Setup(x => x.GetAll()).Returns(asyncServices);

            var inventories = new List<Inventory>
            {
                new Inventory { SparePartId = 10 }
            }.AsQueryable();
            var asyncInventories = new TestAsyncEnumerable<Inventory>(inventories);
            _inventoryRepo.Setup(x => x.Query()).Returns(asyncInventories);

            Appointment? captured = null;
            _appointmentRepo.Setup(x => x.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .Callback<Appointment, CancellationToken>((a, _) =>
                {
                    a.AppointmentId = 1;
                    captured = a;
                })
                .Returns(Task.CompletedTask);
            _appointmentRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _appointmentRepo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    if (captured == null) return null;
                    return new Appointment
                    {
                        AppointmentId = captured.AppointmentId,
                        CustomerId = captured.CustomerId,
                        FirstName = captured.FirstName,
                        LastName = captured.LastName,
                        Phone = captured.Phone,
                        CustomVehicleBrand = captured.CustomVehicleBrand,
                        CustomVehicleModel = captured.CustomVehicleModel,
                        LicensePlate = captured.LicensePlate,
                        AppointmentDateTime = captured.AppointmentDateTime,
                        Status = captured.Status,
                        Description = captured.Description,
                        Services = new List<Garage_Management.Base.Entities.Accounts.AppointmentService>
                        {
                            new Garage_Management.Base.Entities.Accounts.AppointmentService
                            {
                                ServiceId = 1,
                                Service = new Service
                                {
                                    ServiceId = 1,
                                    ServiceName = "S1",
                                    ServiceTasks = new List<ServiceTask>()
                                }
                            },
                            new Garage_Management.Base.Entities.Accounts.AppointmentService
                            {
                                ServiceId = 2,
                                Service = new Service
                                {
                                    ServiceId = 2,
                                    ServiceName = "S2",
                                    ServiceTasks = new List<ServiceTask>()
                                }
                            }
                        },
                        SpareParts = new List<AppointmentSparePart>
                        {
                            new AppointmentSparePart
                            {
                                SparePartId = 10,
                                Inventory = new Inventory
                                {
                                    SparePartId = 10,
                                    PartName = "P1"
                                }
                            }
                        }
                    };
                });

            var result = await _service.CreateAsync(request);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.AppointmentId);
            Assert.AreEqual("Khanh", result.FirstName);
            Assert.AreEqual("Do", result.LastName);
            Assert.AreEqual("+84912345678", result.Phone);
            Assert.AreEqual("Honda", result.CustomVehicleBrand);
            Assert.AreEqual("Vision", result.CustomVehicleModel);
            Assert.AreEqual("29A-12345", result.LicensePlate);
            Assert.AreEqual(2, result.Services.Count);
            Assert.AreEqual(1, result.SpareParts.Count);
        }

        [TestMethod]
        public async Task CreateAsync_WithInvalidCustomerId_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                CustomerId = 99,
                FirstName = "A",
                LastName = "B",
                Phone = "0900000000",
                CustomVehicleBrand = "Honda",
                CustomVehicleModel = "Vision",
                LicensePlate = "29A-12345",
                AppointmentDateTime = DateTime.UtcNow
            };

            _customerRepo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Customer?)null);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        }

        [TestMethod]
        public async Task CreateAsync_WithInvalidServiceIds_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                CustomerId = null,
                FirstName = "A",
                LastName = "B",
                Phone = "0900000000",
                CustomVehicleBrand = "Honda",
                CustomVehicleModel = "Vision",
                LicensePlate = "29A-12345",
                AppointmentDateTime = DateTime.UtcNow,
                ServiceIds = new List<int> { 1, 2 }
            };

            var services = new List<Service> { new Service { ServiceId = 1 } }.AsQueryable();
            var asyncServices = new TestAsyncEnumerable<Service>(services);
            _serviceRepo.Setup(x => x.GetAll()).Returns(asyncServices);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        }

        [TestMethod]
        public async Task CreateAsync_WithInvalidSpareParts_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                CustomerId = null,
                FirstName = "A",
                LastName = "B",
                Phone = "0900000000",
                CustomVehicleBrand = "Honda",
                CustomVehicleModel = "Vision",
                LicensePlate = "29A-12345",
                AppointmentDateTime = DateTime.UtcNow,
                SparePartsIds = new List<int> { 10 }
            };

            var inventories = new List<Inventory>().AsQueryable();
            var asyncInventories = new TestAsyncEnumerable<Inventory>(inventories);
            _inventoryRepo.Setup(x => x.Query()).Returns(asyncInventories);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        }

        [TestMethod]
        public async Task CreateAsync_WithoutVehicleInfo_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                FirstName = "A",
                LastName = "B",
                Phone = "0900000000",
                AppointmentDateTime = DateTime.UtcNow
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        }

        [TestMethod]
        public async Task CreateAsync_WithVehicleId_AndCustomInfo_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                VehicleId = 1,
                FirstName = "A",
                LastName = "B",
                Phone = "0900000000",
                CustomVehicleBrand = "Honda",
                CustomVehicleModel = "Vision",
                LicensePlate = "29A-12345",
                AppointmentDateTime = DateTime.UtcNow
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        }

        [TestMethod]
        public async Task CreateAsync_WithVehicleModelId_NoLicensePlate_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                VehicleModelId = 1,
                FirstName = "A",
                LastName = "B",
                Phone = "0900000000",
                AppointmentDateTime = DateTime.UtcNow
            };

            _vehicleModelRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Garage_Management.Base.Entities.Vehiclies.VehicleModel());

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        }

        [TestMethod]
        public async Task CreateAsync_WithCustomBrandOnly_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                FirstName = "A",
                LastName = "B",
                Phone = "0900000000",
                CustomVehicleBrand = "Honda",
                AppointmentDateTime = DateTime.UtcNow
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        }

        [TestMethod]
        public async Task CreateAsync_WithCustomBrandModel_NoLicensePlate_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                FirstName = "A",
                LastName = "B",
                Phone = "0900000000",
                CustomVehicleBrand = "Honda",
                CustomVehicleModel = "Vision",
                AppointmentDateTime = DateTime.UtcNow
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        }

        [TestMethod]
        public async Task CreateAsync_WithInvalidVehicleId_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                VehicleId = 99,
                FirstName = "A",
                LastName = "B",
                Phone = "0900000000",
                AppointmentDateTime = DateTime.UtcNow
            };

            _vehicleRepo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Garage_Management.Base.Entities.Vehiclies.Vehicle?)null);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        }

        [TestMethod]
        public async Task CreateAsync_WithInvalidVehicleModelId_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                VehicleModelId = 99,
                LicensePlate = "29A-12345",
                FirstName = "A",
                LastName = "B",
                Phone = "0900000000",
                AppointmentDateTime = DateTime.UtcNow
            };

            _vehicleModelRepo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Garage_Management.Base.Entities.Vehiclies.VehicleModel?)null);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        }

        [TestMethod]
        public async Task CreateAsync_WithVehicleModelIdAndLicense_ReturnsResponse()
        {
            var request = new AppointmentCreateRequest
            {
                VehicleModelId = 1,
                LicensePlate = "29A-12345",
                FirstName = "A",
                LastName = "B",
                Phone = "0900000000",
                AppointmentDateTime = DateTime.UtcNow
            };

            _vehicleModelRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Garage_Management.Base.Entities.Vehiclies.VehicleModel());

            Appointment? captured = null;
            _appointmentRepo.Setup(x => x.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .Callback<Appointment, CancellationToken>((a, _) =>
                {
                    a.AppointmentId = 2;
                    captured = a;
                })
                .Returns(Task.CompletedTask);
            _appointmentRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _appointmentRepo.Setup(x => x.GetByIdWithDetailsAsync(2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => captured);

            var result = await _service.CreateAsync(request);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.AppointmentId);
            Assert.AreEqual(1, result.VehicleModelId);
            Assert.AreEqual("29A-12345", result.LicensePlate);
        }

        [TestMethod]
        public async Task CreateAsync_WithVehicleId_ReturnsResponse()
        {
            var request = new AppointmentCreateRequest
            {
                VehicleId = 1,
                FirstName = "A",
                LastName = "B",
                Phone = "0900000000",
                AppointmentDateTime = DateTime.UtcNow
            };

            _vehicleRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Garage_Management.Base.Entities.Vehiclies.Vehicle());

            Appointment? captured = null;
            _appointmentRepo.Setup(x => x.AddAsync(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
                .Callback<Appointment, CancellationToken>((a, _) =>
                {
                    a.AppointmentId = 3;
                    captured = a;
                })
                .Returns(Task.CompletedTask);
            _appointmentRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _appointmentRepo.Setup(x => x.GetByIdWithDetailsAsync(3, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => captured);

            var result = await _service.CreateAsync(request);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.AppointmentId);
            Assert.AreEqual(1, result.VehicleId);
        }
    }
}
