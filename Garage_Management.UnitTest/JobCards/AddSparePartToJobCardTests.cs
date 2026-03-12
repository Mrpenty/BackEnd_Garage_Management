using Garage_Management.Application.DTOs.JobCard;
using Garage_Management.Application.Repositories.Appointments;
using Garage_Management.Application.Repositories.Inventories;
using Garage_Management.Application.Repositories.JobCards;
using Garage_Management.Application.Repositories.Services;
using Garage_Management.Application.Services.JobCards;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.JobCard
{
    [TestClass]
    public class AddSparePartToJobCardTests
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
        public async Task AddSparePartAsync_ReturnFalse_WhenJobCardNotFound()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            var service = BuildService(context);
            var dto = new AddSparePartToJobCardDto { SparePartId = 1, Quantity = 1 };
            var result = await service.AddSparePartAsync(999, dto, CancellationToken.None);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AddSparePartAsync_ReturnFalse_WhenInventoryNotFound()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            context.JobCards.Add(new JobCard { JobCardId = 1, CustomerId = 1, VehicleId = 1, StartDate = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var service = BuildService(context);
            var dto = new AddSparePartToJobCardDto { SparePartId = 999, Quantity = 1 };
            var result = await service.AddSparePartAsync(1, dto, CancellationToken.None);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AddSparePartAsync_ReturnFalse_WhenQuantityNonPositive()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            context.JobCards.Add(new JobCard { JobCardId = 1, CustomerId = 1, VehicleId = 1, StartDate = DateTime.UtcNow });
            context.Set<Inventory>().Add(new Inventory { SparePartId = 5, SellingPrice = 10m });
            await context.SaveChangesAsync();

            var service = BuildService(context);
            var dto = new AddSparePartToJobCardDto { SparePartId = 5, Quantity = 0 };
            var result = await service.AddSparePartAsync(1, dto, CancellationToken.None);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AddSparePartAsync_ReturnTrue_WhenValid()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            using var context = new AppDbContext(options);
            context.JobCards.Add(new JobCard { JobCardId = 7, CustomerId = 1, VehicleId = 1, StartDate = DateTime.UtcNow });
            context.Set<Inventory>().Add(new Inventory { SparePartId = 7, SellingPrice = 20m });
            await context.SaveChangesAsync();

            var service = BuildService(context);
            var dto = new AddSparePartToJobCardDto { SparePartId = 7, Quantity = 2, IsUnderWarranty = true, Note = "ok" };
            var result = await service.AddSparePartAsync(7, dto, CancellationToken.None);
            Assert.IsTrue(result);

            var created = context.Set<JobCardSparePart>().FirstOrDefault(j => j.JobCardId == 7 && j.SparePartId == 7);
            Assert.IsNotNull(created);
            Assert.AreEqual(40m, created.TotalAmount);
        }
    }
}