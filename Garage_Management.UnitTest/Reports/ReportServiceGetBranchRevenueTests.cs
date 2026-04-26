using Garage_Management.Application.Services.Reports;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities;
using Garage_Management.Base.Entities.Branches;
using Garage_Management.UnitTest.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Reports
{
    [TestClass]
    public class ReportServiceGetBranchRevenueTests
    {
        private static AppDbContext NewDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private static Branch SeedBranch(AppDbContext db, int id = 100, string code = "HN-100", string name = "Hà Nội 100")
        {
            var branch = new Branch
            {
                BranchId = id,
                BranchCode = code,
                Name = name,
                Address = "123 Test",
                IsActive = true
            };
            db.Branches.Add(branch);
            db.SaveChanges();
            return branch;
        }

        private static Invoice MakeInvoice(int id, int branchId, DateTime date, decimal service, decimal sparePart)
            => new Invoice
            {
                InvoiceId = id,
                BranchId = branchId,
                JobCardId = id,
                InvoiceDate = date,
                ServiceTotal = service,
                SparePartTotal = sparePart,
                GrandTotal = service + sparePart,
                PaymentStatus = "Paid"
            };

        /// <summary>
        /// UTCID01 - Normal: Admin lấy revenue của branch có 2 invoice → trả tổng đúng
        /// </summary>
        [TestMethod]
        public async Task UTCID01_GetBranchRevenue_Admin_ReturnsAggregated()
        {
            using var db = NewDb();
            SeedBranch(db, 100);
            db.Invoices.AddRange(
                MakeInvoice(1, 100, new DateTime(2026, 4, 10, 9, 0, 0, DateTimeKind.Utc), 200000m, 100000m),
                MakeInvoice(2, 100, new DateTime(2026, 4, 15, 9, 0, 0, DateTimeKind.Utc), 150000m, 50000m)
            );
            await db.SaveChangesAsync();

            var service = new ReportService(db, MockCurrentUser.AsAdmin());

            var result = await service.GetBranchRevenueAsync(100, null, null);

            Assert.IsNotNull(result);
            Assert.AreEqual(100, result.BranchId);
            Assert.AreEqual("HN-100", result.BranchCode);
            Assert.AreEqual(2, result.InvoiceCount);
            Assert.AreEqual(350000m, result.ServiceTotal);
            Assert.AreEqual(150000m, result.SparePartTotal);
            Assert.AreEqual(500000m, result.GrandTotal);
        }

        /// <summary>
        /// UTCID02 - Normal: Staff branch=100 + date range → chỉ tính invoice trong khoảng
        /// </summary>
        [TestMethod]
        public async Task UTCID02_GetBranchRevenue_StaffOwnBranch_WithDateRange_ReturnsFiltered()
        {
            using var db = NewDb();
            SeedBranch(db, 100);
            db.Invoices.AddRange(
                MakeInvoice(1, 100, new DateTime(2026, 4, 1, 9, 0, 0, DateTimeKind.Utc), 100000m, 50000m),  // out
                MakeInvoice(2, 100, new DateTime(2026, 4, 15, 9, 0, 0, DateTimeKind.Utc), 200000m, 100000m), // in
                MakeInvoice(3, 100, new DateTime(2026, 4, 20, 9, 0, 0, DateTimeKind.Utc), 50000m, 30000m),   // in
                MakeInvoice(4, 100, new DateTime(2026, 5, 5, 9, 0, 0, DateTimeKind.Utc), 999000m, 999000m)   // out
            );
            await db.SaveChangesAsync();

            var service = new ReportService(db, MockCurrentUser.AsStaff(branchId: 100));

            var from = new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2026, 4, 30, 23, 59, 59, DateTimeKind.Utc);
            var result = await service.GetBranchRevenueAsync(100, from, to);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.InvoiceCount);
            Assert.AreEqual(250000m, result.ServiceTotal);   // 200k + 50k
            Assert.AreEqual(130000m, result.SparePartTotal); // 100k + 30k
            Assert.AreEqual(380000m, result.GrandTotal);
            Assert.AreEqual(from, result.FromDate);
            Assert.AreEqual(to, result.ToDate);
        }

        /// <summary>
        /// UTCID03 - Abnormal: Staff branch=100 cố xem branch=200 → throw UnauthorizedAccessException
        /// </summary>
        [TestMethod]
        public async Task UTCID03_GetBranchRevenue_StaffCrossBranch_Throws()
        {
            using var db = NewDb();
            SeedBranch(db, 200);

            var service = new ReportService(db, MockCurrentUser.AsStaff(branchId: 100));

            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                service.GetBranchRevenueAsync(200, null, null));
            Assert.AreEqual("Không có quyền xem báo cáo của chi nhánh khác", ex.Message);
        }

        /// <summary>
        /// UTCID04 - Abnormal: Branch không tồn tại → trả null (Admin đã pass access check)
        /// </summary>
        [TestMethod]
        public async Task UTCID04_GetBranchRevenue_BranchNotFound_ReturnsNull()
        {
            using var db = NewDb();
            var service = new ReportService(db, MockCurrentUser.AsAdmin());

            var result = await service.GetBranchRevenueAsync(99999, null, null);

            Assert.IsNull(result);
        }

        /// <summary>
        /// UTCID05 - Boundary: Branch có nhưng không có invoice → tất cả total = 0
        /// </summary>
        [TestMethod]
        public async Task UTCID05_GetBranchRevenue_NoInvoices_ReturnsZeros()
        {
            using var db = NewDb();
            SeedBranch(db, 100);

            var service = new ReportService(db, MockCurrentUser.AsAdmin());

            var result = await service.GetBranchRevenueAsync(100, null, null);

            Assert.IsNotNull(result);
            Assert.AreEqual(100, result.BranchId);
            Assert.AreEqual(0, result.InvoiceCount);
            Assert.AreEqual(0m, result.ServiceTotal);
            Assert.AreEqual(0m, result.SparePartTotal);
            Assert.AreEqual(0m, result.GrandTotal);
        }

        /// <summary>
        /// UTCID06 - Boundary: Date range loại bỏ tất cả invoice → total = 0 nhưng response vẫn có (BranchId, FromDate, ToDate)
        /// </summary>
        [TestMethod]
        public async Task UTCID06_GetBranchRevenue_DateRangeFiltersAll_ReturnsZeros()
        {
            using var db = NewDb();
            SeedBranch(db, 100);
            db.Invoices.Add(MakeInvoice(1, 100, new DateTime(2026, 4, 15, 9, 0, 0, DateTimeKind.Utc), 100000m, 50000m));
            await db.SaveChangesAsync();

            var service = new ReportService(db, MockCurrentUser.AsAdmin());

            var from = new DateTime(2027, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2027, 12, 31, 0, 0, 0, DateTimeKind.Utc);
            var result = await service.GetBranchRevenueAsync(100, from, to);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.InvoiceCount);
            Assert.AreEqual(0m, result.GrandTotal);
            Assert.AreEqual(from, result.FromDate);
            Assert.AreEqual(to, result.ToDate);
        }
    }
}
