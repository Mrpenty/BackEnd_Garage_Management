using Garage_Management.Application.Services.Reports;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Branches;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.UnitTest.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Reports
{
    [TestClass]
    public class ReportServiceGetBranchJobCardSummaryTests
    {
        private static AppDbContext NewDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private static Branch SeedBranch(AppDbContext db, int id = 100)
        {
            var branch = new Branch
            {
                BranchId = id,
                BranchCode = $"HN-{id}",
                Name = $"Hà Nội {id}",
                Address = "123 Test",
                IsActive = true
            };
            db.Branches.Add(branch);
            db.SaveChanges();
            return branch;
        }

        private static JobCard MakeJobCard(int id, int branchId, JobCardStatus status, DateTime startDate)
            => new JobCard
            {
                JobCardId = id,
                BranchId = branchId,
                CustomerId = 1,
                VehicleId = 1,
                StartDate = startDate,
                Status = status
            };

        /// <summary>
        /// UTCID01 - Normal: Admin lấy summary với 5 jobcard ở 3 status khác nhau → breakdown đúng
        /// </summary>
        [TestMethod]
        public async Task UTCID01_GetSummary_Admin_ReturnsBreakdown()
        {
            using var db = NewDb();
            SeedBranch(db, 100);
            db.JobCards.AddRange(
                MakeJobCard(1, 100, JobCardStatus.Created, new DateTime(2026, 4, 10, 9, 0, 0, DateTimeKind.Utc)),
                MakeJobCard(2, 100, JobCardStatus.InProgress, new DateTime(2026, 4, 11, 9, 0, 0, DateTimeKind.Utc)),
                MakeJobCard(3, 100, JobCardStatus.InProgress, new DateTime(2026, 4, 12, 9, 0, 0, DateTimeKind.Utc)),
                MakeJobCard(4, 100, JobCardStatus.Completed, new DateTime(2026, 4, 13, 9, 0, 0, DateTimeKind.Utc)),
                MakeJobCard(5, 100, JobCardStatus.Completed, new DateTime(2026, 4, 14, 9, 0, 0, DateTimeKind.Utc))
            );
            await db.SaveChangesAsync();

            var service = new ReportService(db, MockCurrentUser.AsAdmin());

            var result = await service.GetBranchJobCardSummaryAsync(100, null, null);

            Assert.IsNotNull(result);
            Assert.AreEqual(100, result.BranchId);
            Assert.AreEqual(5, result.TotalCount);
            Assert.AreEqual(3, result.StatusBreakdown.Count);
            Assert.AreEqual(1, result.StatusBreakdown.First(x => x.Status == JobCardStatus.Created).Count);
            Assert.AreEqual(2, result.StatusBreakdown.First(x => x.Status == JobCardStatus.InProgress).Count);
            Assert.AreEqual(2, result.StatusBreakdown.First(x => x.Status == JobCardStatus.Completed).Count);
            Assert.AreEqual("Completed", result.StatusBreakdown.First(x => x.Status == JobCardStatus.Completed).StatusName);
        }

        /// <summary>
        /// UTCID02 - Normal: Staff branch=100 + date range → chỉ tính jobcard StartDate trong khoảng
        /// </summary>
        [TestMethod]
        public async Task UTCID02_GetSummary_StaffOwnBranch_WithDateRange_ReturnsFiltered()
        {
            using var db = NewDb();
            SeedBranch(db, 100);
            db.JobCards.AddRange(
                MakeJobCard(1, 100, JobCardStatus.InProgress, new DateTime(2026, 4, 1, 9, 0, 0, DateTimeKind.Utc)),  // out
                MakeJobCard(2, 100, JobCardStatus.InProgress, new DateTime(2026, 4, 15, 9, 0, 0, DateTimeKind.Utc)), // in
                MakeJobCard(3, 100, JobCardStatus.Completed, new DateTime(2026, 4, 20, 9, 0, 0, DateTimeKind.Utc)),  // in
                MakeJobCard(4, 100, JobCardStatus.Created, new DateTime(2026, 5, 5, 9, 0, 0, DateTimeKind.Utc))      // out
            );
            await db.SaveChangesAsync();

            var service = new ReportService(db, MockCurrentUser.AsStaff(branchId: 100));

            var from = new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2026, 4, 30, 23, 59, 59, DateTimeKind.Utc);
            var result = await service.GetBranchJobCardSummaryAsync(100, from, to);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.TotalCount);
            Assert.AreEqual(2, result.StatusBreakdown.Count);
            Assert.AreEqual(1, result.StatusBreakdown.First(x => x.Status == JobCardStatus.InProgress).Count);
            Assert.AreEqual(1, result.StatusBreakdown.First(x => x.Status == JobCardStatus.Completed).Count);
            Assert.AreEqual(from, result.FromDate);
            Assert.AreEqual(to, result.ToDate);
        }

        /// <summary>
        /// UTCID03 - Abnormal: Staff branch=100 cố xem branch=200 → throw
        /// </summary>
        [TestMethod]
        public async Task UTCID03_GetSummary_StaffCrossBranch_Throws()
        {
            using var db = NewDb();
            SeedBranch(db, 200);

            var service = new ReportService(db, MockCurrentUser.AsStaff(branchId: 100));

            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                service.GetBranchJobCardSummaryAsync(200, null, null));
            Assert.AreEqual("Không có quyền xem báo cáo của chi nhánh khác", ex.Message);
        }

        /// <summary>
        /// UTCID04 - Abnormal: Branch không tồn tại → null
        /// </summary>
        [TestMethod]
        public async Task UTCID04_GetSummary_BranchNotFound_ReturnsNull()
        {
            using var db = NewDb();
            var service = new ReportService(db, MockCurrentUser.AsAdmin());

            var result = await service.GetBranchJobCardSummaryAsync(99999, null, null);

            Assert.IsNull(result);
        }

        /// <summary>
        /// UTCID05 - Boundary: Branch không có jobcard → TotalCount=0, StatusBreakdown rỗng
        /// </summary>
        [TestMethod]
        public async Task UTCID05_GetSummary_NoJobCards_ReturnsEmptyBreakdown()
        {
            using var db = NewDb();
            SeedBranch(db, 100);

            var service = new ReportService(db, MockCurrentUser.AsAdmin());

            var result = await service.GetBranchJobCardSummaryAsync(100, null, null);

            Assert.IsNotNull(result);
            Assert.AreEqual(100, result.BranchId);
            Assert.AreEqual(0, result.TotalCount);
            Assert.AreEqual(0, result.StatusBreakdown.Count);
        }
    }
}
