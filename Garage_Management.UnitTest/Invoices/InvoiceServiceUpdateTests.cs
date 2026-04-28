using Garage_Management.Application.DTOs.Invoices;
using Garage_Management.Application.Interfaces.Repositories.Invoices;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Services.Invoices;
using Garage_Management.Base.Entities;
using Garage_Management.UnitTest.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Invoices
{
    [TestClass]
    public class InvoiceServiceUpdateTests
    {
        private Mock<IInvoiceRepository> _invoiceRepo = null!;
        private Mock<IJobCardRepository> _jobCardRepo = null!;
        private InvoiceService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _invoiceRepo = new Mock<IInvoiceRepository>();
            _jobCardRepo = new Mock<IJobCardRepository>();
            _service = new InvoiceService(_invoiceRepo.Object, _jobCardRepo.Object, MockCurrentUser.AsStaff(branchId: 1, userId: 7));
        }

        private Invoice MakeEntity() => new Invoice
        {
            InvoiceId = 1,
            JobCardId = 10,
            BranchId = 1,
            InvoiceDate = new DateTime(2026, 4, 20, 9, 0, 0, DateTimeKind.Utc),
            ServiceTotal = 100000m,
            SparePartTotal = 50000m,
            GrandTotal = 150000m,
            PaymentStatus = "Unpaid",
            PaymentMethod = "Cash"
        };

        /// <summary>
        /// UTCID01 - Abnormal: Id không tồn tại → return null
        /// </summary>
        [TestMethod]
        public async Task UTCID01_UpdateAsync_NotFound_ReturnsNull()
        {
            _invoiceRepo.Setup(x => x.GetByIdWithDetailsAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Invoice?)null);

            var result = await _service.UpdateAsync(99, new InvoiceUpdateRequest { PaymentStatus = "Paid" });

            Assert.IsNull(result);
            _invoiceRepo.Verify(x => x.Update(It.IsAny<Invoice>()), Times.Never);
            _invoiceRepo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// UTCID02 - Normal: Update full fields → recalc GrandTotal + set UpdatedAt/UpdatedBy
        /// </summary>
        [TestMethod]
        public async Task UTCID02_UpdateAsync_FullUpdate_ReturnsResponseWithRecalculatedTotal()
        {
            var entity = MakeEntity();
            _invoiceRepo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
            _invoiceRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var beforeUpdate = DateTime.UtcNow;
            var newDate = new DateTime(2026, 4, 26, 14, 0, 0, DateTimeKind.Utc);

            var result = await _service.UpdateAsync(1, new InvoiceUpdateRequest
            {
                InvoiceDate = newDate,
                ServiceTotal = 200000m,
                SparePartTotal = 100000m,
                PaymentStatus = "Paid",
                PaymentMethod = "Bank"
            });

            Assert.IsNotNull(result);
            Assert.AreEqual(newDate, entity.InvoiceDate);
            Assert.AreEqual(200000m, entity.ServiceTotal);
            Assert.AreEqual(100000m, entity.SparePartTotal);
            Assert.AreEqual(300000m, entity.GrandTotal);
            Assert.AreEqual("Paid", entity.PaymentStatus);
            Assert.AreEqual("Bank", entity.PaymentMethod);
            Assert.IsNotNull(entity.UpdatedAt);
            Assert.IsTrue(entity.UpdatedAt >= beforeUpdate);
            Assert.AreEqual(7, entity.UpdatedBy);
        }

        /// <summary>
        /// UTCID03 - Normal: Partial update chỉ PaymentStatus → KHÔNG recalc GrandTotal
        /// </summary>
        [TestMethod]
        public async Task UTCID03_UpdateAsync_PartialPaymentStatusOnly_DoesNotRecalcGrandTotal()
        {
            var entity = MakeEntity();
            var originalGrandTotal = entity.GrandTotal;
            _invoiceRepo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
            _invoiceRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(1, new InvoiceUpdateRequest { PaymentStatus = "Paid" });

            Assert.IsNotNull(result);
            Assert.AreEqual("Paid", entity.PaymentStatus);
            Assert.AreEqual(originalGrandTotal, entity.GrandTotal);
            // Service/SparePart Total không đổi
            Assert.AreEqual(100000m, entity.ServiceTotal);
            Assert.AreEqual(50000m, entity.SparePartTotal);
        }

        /// <summary>
        /// UTCID04 - Abnormal: Staff Branch=1 update invoice ở Branch=2 → throw UnauthorizedAccessException
        /// </summary>
        [TestMethod]
        public async Task UTCID04_UpdateAsync_CrossBranchAccess_Throws()
        {
            var entity = MakeEntity();
            entity.BranchId = 2;
            _invoiceRepo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.UpdateAsync(1, new InvoiceUpdateRequest { PaymentStatus = "Paid" }));
            Assert.AreEqual("Không có quyền truy cập hóa đơn của chi nhánh khác", ex.Message);
        }

        /// <summary>
        /// UTCID05 - Boundary: Chỉ update SparePartTotal → recalc GrandTotal đúng (ServiceTotal giữ nguyên + new SparePartTotal)
        /// </summary>
        [TestMethod]
        public async Task UTCID05_UpdateAsync_OnlySparePartTotal_RecalcsGrandTotal()
        {
            var entity = MakeEntity();
            _invoiceRepo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
            _invoiceRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(1, new InvoiceUpdateRequest { SparePartTotal = 80000m });

            Assert.IsNotNull(result);
            Assert.AreEqual(100000m, entity.ServiceTotal);  // không đổi
            Assert.AreEqual(80000m, entity.SparePartTotal);  // đã đổi
            Assert.AreEqual(180000m, entity.GrandTotal);     // 100k + 80k
        }

        /// <summary>
        /// UTCID06 - Normal: Admin có thể update invoice của bất kỳ branch (cross-branch OK)
        /// </summary>
        [TestMethod]
        public async Task UTCID06_UpdateAsync_AdminCrossBranch_Succeeds()
        {
            var adminService = new InvoiceService(_invoiceRepo.Object, _jobCardRepo.Object, MockCurrentUser.AsAdmin(userId: 1));
            var entity = MakeEntity();
            entity.BranchId = 2; // branch khác
            _invoiceRepo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
            _invoiceRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await adminService.UpdateAsync(1, new InvoiceUpdateRequest { PaymentStatus = "Paid" });

            Assert.IsNotNull(result);
            Assert.AreEqual("Paid", entity.PaymentStatus);
            Assert.AreEqual(1, entity.UpdatedBy);
        }
    }
}
