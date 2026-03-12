using Garage_Management.Application.DTOs.JobCard;
using Garage_Management.Application.Repositories.Appointments;
using Garage_Management.Application.Repositories.Inventories;
using Garage_Management.Application.Repositories.JobCards;
using Garage_Management.Application.Repositories.Services;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.JobCard
{
    [TestClass]
    public class CreateJobCardTests
    {
        private Application.Services.JobCards.JobCardService BuildService(AppDbContext ctx)
        {
            return new Application.Services.JobCards.JobCardService(
                new JobCardRepository(ctx),
                new ServiceRepository(ctx),
                new InventoryRepository(ctx),
                new JobCardServiceRepository(ctx),
                new JobCardSparePartRepository(ctx),
                new WorkBayRepository(ctx),
                new AppointmentRepository(ctx)
            );
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenAppointmentAlreadyHasJobCard()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new AppDbContext(options);
            var appointment = new Appointment
            {
                AppointmentId = 1,
                Status = AppointmentStatus.Confirmed,
                AppointmentDateTime = DateTime.UtcNow
            };
            context.Appointments.Add(appointment);
            context.JobCards.Add(new JobCard
            {
                AppointmentId = 1,
                CustomerId = 1,
                VehicleId = 1,
                StartDate = DateTime.UtcNow,
                Status = JobCardStatus.Created
            });
            await context.SaveChangesAsync();

            var service = BuildService(context);
            var dto = new CreateJobCardDto
            {
                AppointmentId = 1,
                CustomerId = 1,
                VehicleId = 1
            };

            await Assert.ThrowsExceptionAsync<Exception>(
                () => service.CreateAsync(dto, 42, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_Throws_WhenVehicleHasActiveJobCard()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new AppDbContext(options);
            // existing active job card for vehicle 5
            context.JobCards.Add(new JobCard
            {
                AppointmentId = 1,
                CustomerId = 1,
                VehicleId = 5,
                StartDate = DateTime.UtcNow,
                Status = JobCardStatus.Created
            });

            var appointment = new Appointment
            {
                AppointmentId = 2,
                Status = AppointmentStatus.Confirmed,
                AppointmentDateTime = DateTime.UtcNow
            };
            context.Appointments.Add(appointment);
            await context.SaveChangesAsync();

            var service = BuildService(context);
            var dto = new CreateJobCardDto
            {
                AppointmentId = 2,
                CustomerId = 2,
                VehicleId = 5
            };

            await Assert.ThrowsExceptionAsync<Exception>(
                () => service.CreateAsync(dto, 1, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_ReturnsDto_WhenValid()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new AppDbContext(options);
            var appointment = new Appointment
            {
                AppointmentId = 10,
                Status = AppointmentStatus.Confirmed,
                AppointmentDateTime = DateTime.UtcNow
            };
            context.Appointments.Add(appointment);
            await context.SaveChangesAsync();

            var service = BuildService(context);
            var dto = new CreateJobCardDto
            {
                AppointmentId = 10,
                CustomerId = 20,
                VehicleId = 30,
                Note = "test",
                SupervisorId = 3
            };

            var result = await service.CreateAsync(dto, currentUserId: 77, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(dto.AppointmentId, result.AppointmentId);
            Assert.AreEqual(dto.CustomerId, result.CustomerId);
            Assert.AreEqual(dto.VehicleId, result.VehicleId);
            Assert.AreEqual(JobCardStatus.Created, result.Status);
        }
    }
}