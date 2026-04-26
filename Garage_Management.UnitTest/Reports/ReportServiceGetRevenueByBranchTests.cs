using Garage_Management.Application.Services.Reports;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities;
using Garage_Management.Base.Entities.Branches;
using Garage_Management.UnitTest.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Reports
{
    [TestClass]
    public class ReportServiceGetRevenueByBranchTests
    {
        private static AppDbContext NewDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private static void SeedBranch(AppDbContext db, int id, string code, string name)
        {
            db.Branches.Add(new Branch
            {
                BranchId = id,
                BranchCode = code,
                Name = name,
                Address = "123 Test",
                IsActive = true
            });
            db.SaveChanges();
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
        /// UTCID01 - Normal: Admin lấy revenue tất cả branch → mỗi branch có aggregate riêng
        /// </summary>
        [TestMethod]
        public async Task UTCID01_GetRevenueByBranch_Admin_ReturnsAllBranches()
        {
            using var db = NewDb();
            SeedBranch(db, 100, "HN-100", "Hà Nội");
            SeedBranch(db, 200, "SG-200", "Sài Gòn");
            db.Invoices.AddRange(
                MakeInvoice(1, 100, new DateTime(2026, 4, 10, 9, 0, 0, DateTimeKind.Utc), 200000m, 100000m),
                MakeInvoice(2, 100, new DateTime(2026, 4, 11, 9, 0, 0, DateTimeKind.Utc), 100000m, 50000m),
                MakeInvoice(3, 200, new DateTime(2026, 4, 12, 9, 0, 0, DateTimeKind.Utc), 500000m, 200000m)
            );
            await db.SaveChangesAsync();

            var service = new ReportService(db, MockCurrentUser.AsAdmin());

            var result = await service.GetRevenueByBranchAsync(null, null);

            Assert.IsNotNull(result);
            var hn = result.FirstOrDefault(r => r.BranchId == 100);
            var sg = result.FirstOrDefault(r => r.BranchId == 200);
            Assert.IsNotNull(hn);
            Assert.IsNotNull(sg);
            Assert.AreEqual(2, hn!.InvoiceCount);
            Assert.AreEqual(300000m, hn.ServiceTotal);
            Assert.AreEqual(150000m, hn.SparePartTotal);
            Assert.AreEqual(450000m, hn.GrandTotal);
            Assert.AreEqual(1, sg!.InvoiceCount);
            Assert.AreEqual(700000m, sg.GrandTotal);
        }

        /// <summary>
        /// UTCID02 - Abnormal: Non-admin (Staff) → throw UnauthorizedAccessException
        /// </summary>
        [TestMethod]
        public async Task UTCID02_GetRevenueByBranch_NonAdmin_Throws()
        {
            using var db = NewDb();
            var service = new ReportService(db, MockCurrentUser.AsStaff(branchId: 100));

            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                service.GetRevenueByBranchAsync(null, null));
            Assert.AreEqual("Chỉ Admin được xem báo cáo tất cả chi nhánh", ex.Message);
        }

        /// <summary>
        /// UTCID03 - Boundary: Branch không có invoice nào → vẫn xuất hiện trong list với total=0
        /// </summary>
        [TestMethod]
        public async Task UTCID03_GetRevenueByBranch_BranchWithoutInvoices_ReturnsZerosForThatBranch()
        {
            using var db = NewDb();
            SeedBranch(db, 100, "HN-100", "Hà Nội");
            SeedBranch(db, 200, "SG-200", "Sài Gòn"); // không có invoice
            db.Invoices.Add(MakeInvoice(1, 100, new DateTime(2026, 4, 10, 9, 0, 0, DateTimeKind.Utc), 100000m, 50000m));
            await db.SaveChangesAsync();

            var service = new ReportService(db, MockCurrentUser.AsAdmin());

            var result = await service.GetRevenueByBranchAsync(null, null);

            var sg = result.FirstOrDefault(r => r.BranchId == 200);
            Assert.IsNotNull(sg);
            Assert.AreEqual(0, sg!.InvoiceCount);
            Assert.AreEqual(0m, sg.ServiceTotal);
            Assert.AreEqual(0m, sg.SparePartTotal);
            Assert.AreEqual(0m, sg.GrandTotal);
        }

        /// <summary>
        /// UTCID04 - Normal: Date range filter → loại invoice ngoài range trước khi aggregate per-branch
        /// </summary>
        [TestMethod]
        public async Task UTCID04_GetRevenueByBranch_WithDateRange_FiltersInvoices()
        {
            using var db = NewDb();
            SeedBranch(db, 100, "HN-100", "Hà Nội");
            db.Invoices.AddRange(
                MakeInvoice(1, 100, new DateTime(2026, 4, 1, 9, 0, 0, DateTimeKind.Utc), 100000m, 50000m),  // out
                MakeInvoice(2, 100, new DateTime(2026, 4, 15, 9, 0, 0, DateTimeKind.Utc), 200000m, 100000m), // in
                MakeInvoice(3, 100, new DateTime(2026, 5, 5, 9, 0, 0, DateTimeKind.Utc), 999000m, 999000m)   // out
            );
            await db.SaveChangesAsync();

            var service = new ReportService(db, MockCurrentUser.AsAdmin());

            var from = new DateTime(2026, 4, 10, 0, 0, 0, DateTimeKind.Utc);
            var to = new DateTime(2026, 4, 30, 23, 59, 59, DateTimeKind.Utc);
            var result = await service.GetRevenueByBranchAsync(from, to);

            var hn = result.FirstOrDefault(r => r.BranchId == 100);
            Assert.IsNotNull(hn);
            Assert.AreEqual(1, hn!.InvoiceCount);
            Assert.AreEqual(200000m, hn.ServiceTotal);
            Assert.AreEqual(100000m, hn.SparePartTotal);
            Assert.AreEqual(300000m, hn.GrandTotal);
            Assert.AreEqual(from, hn.FromDate);
            Assert.AreEqual(to, hn.ToDate);
        }
    }
}
