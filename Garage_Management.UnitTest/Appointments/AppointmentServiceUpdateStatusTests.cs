using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Appointments;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Accounts;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Appointments
{
    [TestClass]
    public class AppointmentServiceUpdateStatusTests
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
        public async Task UpdateStatusAsync_NotFound_ReturnsNull()
        {
            _appointmentRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((Appointment?)null);

            var result = await _service.UpdateStatusAsync(1, AppointmentStatus.Confirmed, CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_PendingToConfirmed_UpdatesStatus()
        {
            var entity = CreateAppointment(AppointmentStatus.Pending);
            _appointmentRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _appointmentRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _appointmentRepo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.UpdateStatusAsync(1, AppointmentStatus.Confirmed, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(AppointmentStatus.Confirmed, entity.Status);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_PendingToCancelled_UpdatesStatus()
        {
            var entity = CreateAppointment(AppointmentStatus.Pending);
            _appointmentRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _appointmentRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _appointmentRepo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.UpdateStatusAsync(1, AppointmentStatus.Cancelled, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(AppointmentStatus.Cancelled, entity.Status);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_PendingToInProgress_Throws()
        {
            var entity = CreateAppointment(AppointmentStatus.Pending);
            _appointmentRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateStatusAsync(1, AppointmentStatus.ConvertedToJobCard, CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateStatusAsync_ConfirmedToInProgress_UpdatesStatus()
        {
            var entity = CreateAppointment(AppointmentStatus.Confirmed);
            _appointmentRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _appointmentRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _appointmentRepo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.UpdateStatusAsync(1, AppointmentStatus.ConvertedToJobCard, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(AppointmentStatus.ConvertedToJobCard, entity.Status);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_ConfirmedToNoShow_UpdatesStatus()
        {
            var entity = CreateAppointment(AppointmentStatus.Confirmed);
            _appointmentRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _appointmentRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _appointmentRepo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.UpdateStatusAsync(1, AppointmentStatus.NoShow, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(AppointmentStatus.NoShow, entity.Status);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_ConfirmedToCompleted_Throws()
        {
            var entity = CreateAppointment(AppointmentStatus.Confirmed);
            _appointmentRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateStatusAsync(1, AppointmentStatus.Completed, CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateStatusAsync_InProgressToConvertedToJobCard_UpdatesStatus()
        {
            var entity = CreateAppointment(AppointmentStatus.ConvertedToJobCard);
            _appointmentRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _appointmentRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _appointmentRepo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.UpdateStatusAsync(1, AppointmentStatus.ConvertedToJobCard, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(AppointmentStatus.ConvertedToJobCard, entity.Status);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_InProgressToCompleted_UpdatesStatus()
        {
            var entity = CreateAppointment(AppointmentStatus.ConvertedToJobCard);
            _appointmentRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _appointmentRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _appointmentRepo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.UpdateStatusAsync(1, AppointmentStatus.Completed, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(AppointmentStatus.Completed, entity.Status);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_ConvertedToJobCardToCompleted_UpdatesStatus()
        {
            var entity = CreateAppointment(AppointmentStatus.ConvertedToJobCard);
            _appointmentRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _appointmentRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _appointmentRepo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.UpdateStatusAsync(1, AppointmentStatus.Completed, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(AppointmentStatus.Completed, entity.Status);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_NoShowToPending_UpdatesStatus()
        {
            var entity = CreateAppointment(AppointmentStatus.NoShow);
            _appointmentRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);
            _appointmentRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _appointmentRepo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.UpdateStatusAsync(1, AppointmentStatus.Pending, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(AppointmentStatus.Pending, entity.Status);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_Cancelled_Throws()
        {
            var entity = CreateAppointment(AppointmentStatus.Cancelled);
            _appointmentRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateStatusAsync(1, AppointmentStatus.Pending, CancellationToken.None));
        }

        [TestMethod]
        public async Task UpdateStatusAsync_Completed_Throws()
        {
            var entity = CreateAppointment(AppointmentStatus.Completed);
            _appointmentRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.UpdateStatusAsync(1, AppointmentStatus.Pending, CancellationToken.None));
        }

        private static Appointment CreateAppointment(AppointmentStatus status)
        {
            return new Appointment
            {
                AppointmentId = 1,
                Status = status,
                CreatedAt = DateTime.UtcNow,
                Services = new List<Garage_Management.Base.Entities.Accounts.AppointmentService>(),
                SpareParts = new List<AppointmentSparePart>()
            };
        }
    }
}
