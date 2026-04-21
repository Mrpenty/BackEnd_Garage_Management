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

        /// <summary>
        /// UTCID01 - Normal: Khach vang lai tao lich hen thanh cong voi dich vu va phu tung
        /// </summary>
        [TestMethod]
        public async Task UTCID01_CreateAsync_WithValidGuest_ReturnsResponse()
        {
            var request = new AppointmentCreateRequest
            {
                BranchId = 1,
                CustomerId = null,
                FirstName = "Khánh",
                LastName = "Đỗ",
                Phone = "0912345678",
                CustomVehicleBrand = "Honda",
                CustomVehicleModel = "Vision",
                LicensePlate = "29A-12345",
                AppointmentDateTime = new DateTime(2026, 4, 13, 8, 0, 0, DateTimeKind.Utc),
                ServiceIds = new List<int> { 2, 4 },
                SparePartsIds = new List<int> { 10 }
            };

            var services = new List<Service>
            {
                new Service { ServiceId = 2 },
                new Service { ServiceId = 4 }
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
                                ServiceId = 2,
                                Service = new Service
                                {
                                    ServiceId = 2,
                                    ServiceName = "Thay nhớt",
                                    ServiceTasks = new List<ServiceTask>()
                                }
                            },
                            new Garage_Management.Base.Entities.Accounts.AppointmentService
                            {
                                ServiceId = 4,
                                Service = new Service
                                {
                                    ServiceId = 4,
                                    ServiceName = "Rửa xe",
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
                                    PartName = "Lọc gió động cơ"
                                }
                            }
                        }
                    };
                });

            var result = await _service.CreateAsync(request);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.AppointmentId);
            Assert.AreEqual("Khánh", result.FirstName);
            Assert.AreEqual("Đỗ", result.LastName);
            Assert.AreEqual("+84912345678", result.Phone);
            Assert.AreEqual("Honda", result.CustomVehicleBrand);
            Assert.AreEqual("Vision", result.CustomVehicleModel);
            Assert.AreEqual("29A-12345", result.LicensePlate);
            Assert.AreEqual(2, result.Services.Count);
            Assert.AreEqual(1, result.SpareParts.Count);
        }

        /// <summary>
        /// UTCID02 - Abnormal: CustomerId khong ton tai trong he thong
        /// </summary>
        [TestMethod]
        public async Task UTCID02_CreateAsync_WithInvalidCustomerId_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                CustomerId = 99,
                FirstName = "Nguyễn",
                LastName = "Văn Minh",
                Phone = "0987654321",
                CustomVehicleBrand = "Honda",
                CustomVehicleModel = "Vision",
                LicensePlate = "29A-12345",
                AppointmentDateTime = new DateTime(2026, 4, 13, 8, 0, 0, DateTimeKind.Utc),
                ServiceIds = new List<int> { 2, 4 },
                SparePartsIds = new List<int> { 10 }
            };

            _customerRepo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Customer?)null);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
            Assert.AreEqual("CustomerId không tồn tại", ex.Message);
        }

        /// <summary>
        /// UTCID03 - Abnormal: SparePartsIds khong hop le (khong ton tai trong kho)
        /// </summary>
        [TestMethod]
        public async Task UTCID03_CreateAsync_WithInvalidSparePartsIds_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                CustomerId = null,
                FirstName = "Trần",
                LastName = "Thị Lan",
                Phone = "0356789012",
                CustomVehicleBrand = "Honda",
                CustomVehicleModel = "Vision",
                LicensePlate = "29A-12345",
                AppointmentDateTime = new DateTime(2026, 4, 13, 8, 0, 0, DateTimeKind.Utc),
                SparePartsIds = new List<int> { 99 }
            };

            var inventories = new List<Inventory>().AsQueryable();
            var asyncInventories = new TestAsyncEnumerable<Inventory>(inventories);
            _inventoryRepo.Setup(x => x.Query()).Returns(asyncInventories);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
            Assert.AreEqual("SparePartsIds không hợp lệ", ex.Message);
        }

        /// <summary>
        /// UTCID04 - Abnormal: ServiceIds khong hop le (khong ton tai trong danh sach dich vu)
        /// </summary>
        [TestMethod]
        public async Task UTCID04_CreateAsync_WithInvalidServiceIds_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                CustomerId = null,
                FirstName = "Lê",
                LastName = "Hoàng Nam",
                Phone = "0912345678",
                CustomVehicleBrand = "Honda",
                CustomVehicleModel = "Vision",
                LicensePlate = "29A-12345",
                AppointmentDateTime = new DateTime(2026, 4, 13, 8, 0, 0, DateTimeKind.Utc),
                ServiceIds = new List<int> { 99 }
            };

            var services = new List<Service>().AsQueryable();
            var asyncServices = new TestAsyncEnumerable<Service>(services);
            _serviceRepo.Setup(x => x.GetAll()).Returns(asyncServices);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
            Assert.AreEqual("ServiceIds không hợp lệ", ex.Message);
        }

        /// <summary>
        /// UTCID05 - Abnormal: Khong co thong tin xe (VehicleId, VehicleModelId, CustomVehicleBrand deu null)
        /// </summary>
        [TestMethod]
        public async Task UTCID05_CreateAsync_WithoutVehicleInfo_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                FirstName = "Phạm",
                LastName = "Văn Đức",
                Phone = "0978123456",
                AppointmentDateTime = new DateTime(2026, 4, 13, 8, 0, 0, DateTimeKind.Utc)
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
            Assert.AreEqual("Bắt buộc phải có VehicleId hoặc (VehicleModelId + LicensePlate) hoặc (CustomVehicleBrand + CustomVehicleModel + LicensePlate)", ex.Message);
        }

        /// <summary>
        /// UTCID06 - Abnormal: Co VehicleId nhung van nhap CustomVehicleBrand/Model/LicensePlate
        /// </summary>
        [TestMethod]
        public async Task UTCID06_CreateAsync_WithVehicleId_AndCustomInfo_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                VehicleId = 1,
                FirstName = "Nguyễn",
                LastName = "Thị Hoa",
                Phone = "0867543210",
                CustomVehicleBrand = "Honda",
                CustomVehicleModel = "Vision",
                LicensePlate = "29A-12345",
                AppointmentDateTime = new DateTime(2026, 4, 13, 8, 0, 0, DateTimeKind.Utc)
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
            Assert.AreEqual("khi có VehicleId, VehicleModelId/CustomVehicleBrand/CustomVehicleModel/LicensePlate phải để trống", ex.Message);
        }

        /// <summary>
        /// UTCID07 - Abnormal: Co VehicleModelId nhung khong co LicensePlate
        /// </summary>
        [TestMethod]
        public async Task UTCID07_CreateAsync_WithVehicleModelId_NoLicensePlate_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                VehicleModelId = 1,
                FirstName = "Võ",
                LastName = "Minh Tuấn",
                Phone = "0934567890",
                AppointmentDateTime = new DateTime(2026, 4, 13, 8, 0, 0, DateTimeKind.Utc)
            };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
        }

        /// <summary>
        /// UTCID08 - Abnormal: Chi nhap CustomVehicleBrand ma khong nhap CustomVehicleModel
        /// Code bat loi o buoc kiem tra thong tin xe truoc (thieu CustomVehicleModel nen bo 3 khong hop le)
        /// </summary>
        [TestMethod]
        public async Task UTCID08_CreateAsync_WithCustomBrandOnly_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                FirstName = "Đặng",
                LastName = "Quốc Bảo",
                Phone = "0945678901",
                CustomVehicleBrand = "Honda",
                LicensePlate = "29A-12345",
                AppointmentDateTime = new DateTime(2026, 4, 13, 8, 0, 0, DateTimeKind.Utc)
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
            Assert.AreEqual("Bắt buộc phải có VehicleId hoặc (VehicleModelId + LicensePlate) hoặc (CustomVehicleBrand + CustomVehicleModel + LicensePlate)", ex.Message);
        }

        /// <summary>
        /// UTCID09 - Abnormal: Co CustomVehicleBrand va CustomVehicleModel nhung khong co LicensePlate
        /// Code bat loi o buoc kiem tra thong tin xe truoc (thieu LicensePlate nen bo 3 khong hop le)
        /// </summary>
        [TestMethod]
        public async Task UTCID09_CreateAsync_WithCustomBrandModel_NoLicensePlate_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                FirstName = "Bùi",
                LastName = "Thành Long",
                Phone = "0823456789",
                CustomVehicleBrand = "Honda",
                CustomVehicleModel = "Vision",
                AppointmentDateTime = new DateTime(2026, 4, 13, 8, 0, 0, DateTimeKind.Utc)
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
            Assert.AreEqual("Bắt buộc phải có VehicleId hoặc (VehicleModelId + LicensePlate) hoặc (CustomVehicleBrand + CustomVehicleModel + LicensePlate)", ex.Message);
        }

        /// <summary>
        /// UTCID10 - Abnormal: VehicleId khong ton tai trong he thong
        /// </summary>
        [TestMethod]
        public async Task UTCID10_CreateAsync_WithInvalidVehicleId_Throws()
        {
            var request = new AppointmentCreateRequest
            {
                VehicleId = 999,
                FirstName = "Hoàng",
                LastName = "Văn Sơn",
                Phone = "0376543210",
                AppointmentDateTime = new DateTime(2026, 4, 13, 8, 0, 0, DateTimeKind.Utc)
            };

            _vehicleRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((Garage_Management.Base.Entities.Vehiclies.Vehicle?)null);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _service.CreateAsync(request));
            Assert.AreEqual("VehicleId không tồn tại", ex.Message);
        }

        /// <summary>
        /// UTCID11 - Boundary: Tao lich hen voi VehicleModelId + LicensePlate (khong co VehicleId)
        /// </summary>
        [TestMethod]
        public async Task UTCID11_CreateAsync_WithVehicleModelIdAndLicense_ReturnsResponse()
        {
            var request = new AppointmentCreateRequest
            {
                BranchId = 1,
                VehicleModelId = 1,
                LicensePlate = "29A-12345",
                FirstName = "Lý",
                LastName = "Quang Huy",
                Phone = "0901234567",
                AppointmentDateTime = new DateTime(2026, 4, 13, 8, 0, 0, DateTimeKind.Utc)
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
            Assert.IsNull(result.CustomerId);
            Assert.AreEqual("Lý", result.FirstName);
            Assert.AreEqual("Quang Huy", result.LastName);
            Assert.AreEqual("+84901234567", result.Phone);
            Assert.IsNull(result.VehicleId);
            Assert.AreEqual(1, result.VehicleModelId);
            Assert.AreEqual("29A-12345", result.LicensePlate);
        }

        /// <summary>
        /// UTCID12 - Normal: Tao lich hen voi VehicleId co san trong he thong
        /// </summary>
        [TestMethod]
        public async Task UTCID12_CreateAsync_WithVehicleId_ReturnsResponse()
        {
            var request = new AppointmentCreateRequest
            {
                BranchId = 1,
                VehicleId = 1,
                FirstName = "Trương",
                LastName = "Minh Khoa",
                Phone = "0765432109",
                AppointmentDateTime = new DateTime(2026, 4, 13, 8, 0, 0, DateTimeKind.Utc)
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
            Assert.IsNull(result.CustomerId);
            Assert.AreEqual("Trương", result.FirstName);
            Assert.AreEqual("Minh Khoa", result.LastName);
            Assert.AreEqual("+84765432109", result.Phone);
            Assert.AreEqual(1, result.VehicleId);
            Assert.IsNull(result.VehicleModelId);
        }
    }
}
