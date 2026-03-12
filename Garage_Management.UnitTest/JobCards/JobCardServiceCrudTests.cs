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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.JobCard
{
    [TestClass]
    public class JobCardServiceCrudTests
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
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            var service = BuildService(context);
            var result = await service.GetByIdAsync(123);
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_ReturnsDto_WhenExists()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            context.JobCards.Add(new JobCard { JobCardId = 55, CustomerId = 1, VehicleId = 1, StartDate = DateTime.UtcNow, Status = JobCardStatus.Created });
            await context.SaveChangesAsync();
            var service = BuildService(context);
            var dto = await service.GetByIdAsync(55);
            Assert.IsNotNull(dto);
            Assert.AreEqual(55, dto.JobCardId);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_ReturnsFalse_WhenNotFound()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            var service = BuildService(context);
            var result = await service.UpdateStatusAsync(999, JobCardStatus.Completed, CancellationToken.None);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task UpdateStatusAsync_SetsEndDate_WhenCompleted()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            context.JobCards.Add(new JobCard { JobCardId = 66, CustomerId = 1, VehicleId = 1, StartDate = DateTime.UtcNow, Status = JobCardStatus.InProgress });
            await context.SaveChangesAsync();
            var service = BuildService(context);
            var result = await service.UpdateStatusAsync(66, JobCardStatus.Completed, CancellationToken.None);
            Assert.IsTrue(result);
            var card = context.JobCards.Find(66);
            Assert.AreEqual(JobCardStatus.Completed, card.Status);
            Assert.IsNotNull(card.EndDate);
        }

        [TestMethod]
        public async Task UpdateAsync_ReturnsFalse_WhenNotFoundOrDeleted()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            // entity not added
            var service = BuildService(context);
            var result = await service.UpdateAsync(999, new UpdateJobCardDto { Note = "n" }, CancellationToken.None);
            Assert.IsFalse(result);
            
            // deleted scenario
            var card = new JobCard { JobCardId = 77, CustomerId = 1, VehicleId = 1, StartDate = DateTime.UtcNow, DeletedAt = DateTime.UtcNow };
            context.JobCards.Add(card);
            await context.SaveChangesAsync();
            result = await service.UpdateAsync(77, new UpdateJobCardDto { Note = "n" }, CancellationToken.None);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task UpdateAsync_UpdatesSpecifiedFields()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            var card = new JobCard { JobCardId = 88, CustomerId = 1, VehicleId = 1, StartDate = DateTime.UtcNow, Note = "old", SupervisorId = null };
            context.JobCards.Add(card);
            await context.SaveChangesAsync();
            var service = BuildService(context);
            var dto = new UpdateJobCardDto { Note = "new", SupervisorId = 5, EndDate = DateTime.UtcNow.AddDays(1) };
            var result = await service.UpdateAsync(88, dto, CancellationToken.None);
            Assert.IsTrue(result);
            var updated = context.JobCards.Find(88);
            Assert.AreEqual("new", updated.Note);
            Assert.AreEqual(5, updated.SupervisorId);
            Assert.IsNotNull(updated.EndDate);
        }

        [TestMethod]
        public async Task GetJobCardsBySupervisorIdAsync_ReturnsEmpty_WhenNone()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            var service = BuildService(context);
            var list = await service.GetJobCardsBySupervisorIdAsync(12345);
            Assert.IsNotNull(list);
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public async Task GetJobCardsBySupervisorIdAsync_ReturnsItems_WhenExists()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            var job = new JobCard { JobCardId = 99, CustomerId = 1, VehicleId = 1, StartDate = DateTime.UtcNow, SupervisorId = 7 };
            context.JobCards.Add(job);
            await context.SaveChangesAsync();
            var service = BuildService(context);
            var list = await service.GetJobCardsBySupervisorIdAsync(7);
            Assert.IsNotNull(list);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(99, list[0].JobCardId);
        }
    }
}