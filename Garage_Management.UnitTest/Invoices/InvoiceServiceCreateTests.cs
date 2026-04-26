using Garage_Management.Application.DTOs.Invoices;
using Garage_Management.Application.Interfaces.Repositories.Invoices;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Services.Invoices;
using Garage_Management.Base.Entities;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.UnitTest.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Invoices
{
    [TestClass]
    public class InvoiceServiceCreateTests
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

        /// <summary>
        /// UTCID01 - Normal: Tạo hóa đơn hợp lệ từ JobCard chưa có invoice
        /// </summary>
        [TestMethod]
        public async Task UTCID01_CreateAsync_ValidRequest_ReturnsResponse()
        {
            var request = new InvoiceCreateRequest
            {
                JobCardId = 10,
                InvoiceDate = new DateTime(2026, 4, 26, 9, 0, 0, DateTimeKind.Utc),
                ServiceTotal = 200000m,
                SparePartTotal = 150000m,
                PaymentMethod = "Cash"
            };

            _invoiceRepo.Setup(x => x.GetByJobCardIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync((Invoice?)null);
            _jobCardRepo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(new JobCard { JobCardId = 10, BranchId = 1 });

            Invoice? captured = null;
            _invoiceRepo.Setup(x => x.AddAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
                .Callback<Invoice, CancellationToken>((inv, _) => { inv.InvoiceId = 99; captured = inv; })
                .Returns(Task.CompletedTask);
            _invoiceRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _invoiceRepo.Setup(x => x.GetByIdWithDetailsAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => captured);

            var result = await _service.CreateAsync(request);

            Assert.IsNotNull(result);
            Assert.AreEqual(99, result.InvoiceId);
            Assert.AreEqual(10, result.JobCardId);
            Assert.AreEqual(200000m, result.ServiceTotal);
            Assert.AreEqual(150000m, result.SparePartTotal);
            Assert.AreEqual(350000m, result.GrandTotal);
            Assert.AreEqual("Unpaid", result.PaymentStatus);
            Assert.AreEqual("Cash", result.PaymentMethod);
            Assert.IsNotNull(captured);
            Assert.AreEqual(7, captured!.CreatedBy);
            Assert.AreEqual(1, captured.BranchId);
        }

        /// <summary>
        /// UTCID02 - Abnormal: JobCard đã có Invoice (1-1 relation) → throw
        /// </summary>
        [TestMethod]
        public async Task UTCID02_CreateAsync_JobCardAlreadyHasInvoice_Throws()
        {
            var request = new InvoiceCreateRequest
            {
                JobCardId = 10,
                ServiceTotal = 100000m,
                SparePartTotal = 50000m
            };

            _invoiceRepo.Setup(x => x.GetByJobCardIdAsync(10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Invoice { InvoiceId = 1, JobCardId = 10 });

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.CreateAsync(request));
            Assert.AreEqual("JobCard already has an invoice.", ex.Message);
        }

        /// <summary>
        /// UTCID03 - Abnormal: JobCard không tồn tại → throw "Không tìm thấy JobCard"
        /// </summary>
        [TestMethod]
        public async Task UTCID03_CreateAsync_JobCardNotFound_Throws()
        {
            var request = new InvoiceCreateRequest
            {
                JobCardId = 999,
                ServiceTotal = 100000m,
                SparePartTotal = 50000m
            };

            _invoiceRepo.Setup(x => x.GetByJobCardIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Invoice?)null);
            _jobCardRepo.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((JobCard?)null);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                _service.CreateAsync(request));
            Assert.AreEqual("Không tìm thấy JobCard", ex.Message);
        }

        /// <summary>
        /// UTCID04 - Abnormal: Staff Branch=1 cố tạo hóa đơn cho JobCard ở Branch=2 → throw UnauthorizedAccessException
        /// </summary>
        [TestMethod]
        public async Task UTCID04_CreateAsync_CrossBranchAccess_Throws()
        {
            var request = new InvoiceCreateRequest
            {
                JobCardId = 10,
                ServiceTotal = 100000m,
                SparePartTotal = 50000m
            };

            _invoiceRepo.Setup(x => x.GetByJobCardIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync((Invoice?)null);
            _jobCardRepo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(new JobCard { JobCardId = 10, BranchId = 2 });

            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(() =>
                _service.CreateAsync(request));
            Assert.AreEqual("Không có quyền truy cập hóa đơn của chi nhánh khác", ex.Message);
        }

        /// <summary>
        /// UTCID05 - Boundary: ServiceTotal=0 + SparePartTotal=0 → GrandTotal=0, vẫn tạo OK
        /// </summary>
        [TestMethod]
        public async Task UTCID05_CreateAsync_ZeroAmounts_ReturnsResponseWithZeroGrandTotal()
        {
            var request = new InvoiceCreateRequest
            {
                JobCardId = 11,
                ServiceTotal = 0m,
                SparePartTotal = 0m,
                PaymentMethod = "Cash"
            };

            _invoiceRepo.Setup(x => x.GetByJobCardIdAsync(11, It.IsAny<CancellationToken>())).ReturnsAsync((Invoice?)null);
            _jobCardRepo.Setup(x => x.GetByIdAsync(11)).ReturnsAsync(new JobCard { JobCardId = 11, BranchId = 1 });

            Invoice? captured = null;
            _invoiceRepo.Setup(x => x.AddAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
                .Callback<Invoice, CancellationToken>((inv, _) => { inv.InvoiceId = 100; captured = inv; })
                .Returns(Task.CompletedTask);
            _invoiceRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _invoiceRepo.Setup(x => x.GetByIdWithDetailsAsync(100, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => captured);

            var result = await _service.CreateAsync(request);

            Assert.IsNotNull(result);
            Assert.AreEqual(0m, result.GrandTotal);
            Assert.AreEqual("Unpaid", result.PaymentStatus);
        }

        /// <summary>
        /// UTCID06 - Normal: Không truyền InvoiceDate → service set default = DateTime.UtcNow
        /// </summary>
        [TestMethod]
        public async Task UTCID06_CreateAsync_WithoutInvoiceDate_DefaultsToUtcNow()
        {
            var request = new InvoiceCreateRequest
            {
                JobCardId = 12,
                ServiceTotal = 50000m,
                SparePartTotal = 30000m
            };

            _invoiceRepo.Setup(x => x.GetByJobCardIdAsync(12, It.IsAny<CancellationToken>())).ReturnsAsync((Invoice?)null);
            _jobCardRepo.Setup(x => x.GetByIdAsync(12)).ReturnsAsync(new JobCard { JobCardId = 12, BranchId = 1 });

            Invoice? captured = null;
            _invoiceRepo.Setup(x => x.AddAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
                .Callback<Invoice, CancellationToken>((inv, _) => { inv.InvoiceId = 101; captured = inv; })
                .Returns(Task.CompletedTask);
            _invoiceRepo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _invoiceRepo.Setup(x => x.GetByIdWithDetailsAsync(101, It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => captured);
            var beforeCreate = DateTime.UtcNow;

            var result = await _service.CreateAsync(request);

            Assert.IsNotNull(result);
            Assert.IsNotNull(captured);
            Assert.IsTrue(captured!.InvoiceDate >= beforeCreate);
            Assert.IsTrue(captured.InvoiceDate <= DateTime.UtcNow);
        }
    }
}
