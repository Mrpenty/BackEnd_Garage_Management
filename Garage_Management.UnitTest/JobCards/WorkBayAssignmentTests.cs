using Garage_Management.Application.DTOs.JobCard;
using Garage_Management.Application.Repositories.Appointments;
using Garage_Management.Application.Repositories.Inventories;
using Garage_Management.Application.Repositories.JobCards;
using Garage_Management.Application.Repositories.Services;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Common.Enums;
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
    public class WorkBayAssignmentTests
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
        public async Task AssignWorkBayAsync_ReturnFalse_WhenJobCardNotFound()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            context.Set<WorkBay>().Add(new WorkBay { Id = 1, Name = "Bay1", Status = WorkBayStatus.Available });
            await context.SaveChangesAsync();

            var service = BuildService(context);
            var result = await service.AssignWorkBayAsync(new AssignWorkBayRequestDto { JobCardId = 999, WorkBayId = 1 }, CancellationToken.None);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AssignWorkBayAsync_ReturnFalse_WhenWorkBayNotFound()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            context.JobCards.Add(new JobCard { JobCardId = 2, CustomerId = 1, VehicleId = 1, StartDate = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var service = BuildService(context);
            var result = await service.AssignWorkBayAsync(new AssignWorkBayRequestDto { JobCardId = 2, WorkBayId = 99 }, CancellationToken.None);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AssignWorkBayAsync_ReturnFalse_WhenWorkBayOccupied()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            context.JobCards.Add(new JobCard { JobCardId = 3, CustomerId = 1, VehicleId = 1, StartDate = DateTime.UtcNow });
            context.Set<WorkBay>().Add(new WorkBay { Id = 5, Name = "Bay5", Status = WorkBayStatus.Occupied, JobcardId = 123 });
            await context.SaveChangesAsync();

            var service = BuildService(context);
            var result = await service.AssignWorkBayAsync(new AssignWorkBayRequestDto { JobCardId = 3, WorkBayId = 5 }, CancellationToken.None);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AssignWorkBayAsync_ReturnTrue_WhenValid()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            context.JobCards.Add(new JobCard { JobCardId = 4, CustomerId = 1, VehicleId = 1, StartDate = DateTime.UtcNow });
            context.Set<WorkBay>().Add(new WorkBay { Id = 6, Name = "Bay6", Status = WorkBayStatus.Available });
            await context.SaveChangesAsync();

            var service = BuildService(context);
            var result = await service.AssignWorkBayAsync(new AssignWorkBayRequestDto { JobCardId = 4, WorkBayId = 6 }, CancellationToken.None);
            Assert.IsTrue(result);

            var bay = context.Set<WorkBay>().Find(6);
            Assert.AreEqual(WorkBayStatus.Occupied, bay.Status);
            Assert.AreEqual(4, bay.JobcardId);
        }

        [TestMethod]
        public async Task ReleaseWorkBayAsync_ReturnFalse_WhenWorkBayNotFound()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            var service = BuildService(context);
            var result = await service.ReleaseWorkBayAsync(new ReleaseWorkBayDto { WorkBayId = 999 }, CancellationToken.None);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ReleaseWorkBayAsync_ReturnTrue_WhenValid()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            context.Set<WorkBay>().Add(new WorkBay { Id = 7, Name = "Bay7", Status = WorkBayStatus.Occupied, JobcardId = 55 });
            await context.SaveChangesAsync();

            var service = BuildService(context);
            var result = await service.ReleaseWorkBayAsync(new ReleaseWorkBayDto { WorkBayId = 7 }, CancellationToken.None);
            Assert.IsTrue(result);

            var bay = context.Set<WorkBay>().Find(7);
            Assert.AreEqual(WorkBayStatus.Available, bay.Status);
            Assert.IsNull(bay.JobcardId);
        }
    }
}