using Garage_Management.Application.DTOs.JobCard;
using Garage_Management.Application.Repositories.Appointments;
using Garage_Management.Application.Repositories.Inventories;
using Garage_Management.Application.Repositories.JobCards;
using Garage_Management.Application.Repositories.Services;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.JobCard
{
    [TestClass]
    public class AssignMechanicTests
    {
        private JobCardService BuildService(AppDbContext ctx)
        {
            return new JobCardService(
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
        public async Task AssignMechanic_ReturnFalse_WhenJobCardNotFound()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            var service = BuildService(context);
            var dto = new AssignMechanicDto { MechanicId = 1 };

            var result = await service.AssignMechanicAsync(999, dto, CancellationToken.None);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AssignMechanic_ReturnTrue_WhenAlreadyAssigned()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            var card = new JobCard { JobCardId = 10, CustomerId = 1, VehicleId = 1, StartDate = DateTime.UtcNow };
            card.Mechanics.Add(new JobCardMechanic { EmployeeId = 2, AssignedAt = DateTime.UtcNow, Status = (Garage_Management.Base.Common.Enums.MechanicAssignmentStatus)1 });
            context.JobCards.Add(card);
            await context.SaveChangesAsync();

            var service = BuildService(context);
            var dto = new AssignMechanicDto { MechanicId = 2 };
            var result = await service.AssignMechanicAsync(10, dto, CancellationToken.None);
            Assert.IsTrue(result);
        }
    }
}