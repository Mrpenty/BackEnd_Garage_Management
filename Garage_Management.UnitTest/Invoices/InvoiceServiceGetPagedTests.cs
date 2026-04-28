using Garage_Management.Application.Interfaces.Repositories.Invoices;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Services.Invoices;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Common.Models.Invoices;
using Garage_Management.Base.Entities;
using Garage_Management.UnitTest.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Invoices
{
    [TestClass]
    public class InvoiceServiceGetPagedTests
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
        /// UTCID01 - Normal: GetPagedAsync trả paged data với 2 invoices được map đúng
        /// </summary>
        [TestMethod]
        public async Task UTCID01_GetPagedAsync_ReturnsMappedPagedResult()
        {
            var query = new InvoiceQuery { Page = 1, PageSize = 10 };
            var paged = new PagedResult<Invoice>
            {
                Page = 1,
                PageSize = 10,
                Total = 2,
                PageData = new List<Invoice>
                {
                    new Invoice
                    {
                        InvoiceId = 1, JobCardId = 10, BranchId = 1,
                        ServiceTotal = 100000m, SparePartTotal = 50000m, GrandTotal = 150000m,
                        PaymentStatus = "Unpaid", PaymentMethod = "Cash"
                    },
                    new Invoice
                    {
                        InvoiceId = 2, JobCardId = 11, BranchId = 1,
                        ServiceTotal = 200000m, SparePartTotal = 100000m, GrandTotal = 300000m,
                        PaymentStatus = "Paid", PaymentMethod = "Bank"
                    }
                }
            };
            _invoiceRepo.Setup(x => x.GetPagedAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(paged);

            var result = await _service.GetPagedAsync(query);

            Assert.AreEqual(2, result.Total);
            Assert.AreEqual(1, result.Page);
            Assert.AreEqual(10, result.PageSize);
            Assert.AreEqual(2, result.PageData.Count());
            var first = result.PageData.First();
            Assert.AreEqual(1, first.InvoiceId);
            Assert.AreEqual(150000m, first.GrandTotal);
            Assert.AreEqual("Unpaid", first.PaymentStatus);
        }

        /// <summary>
        /// UTCID02 - Boundary: Empty result → Total=0, PageData rỗng
        /// </summary>
        [TestMethod]
        public async Task UTCID02_GetPagedAsync_Empty_ReturnsEmptyPagedResult()
        {
            var query = new InvoiceQuery { Page = 1, PageSize = 10 };
            _invoiceRepo.Setup(x => x.GetPagedAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<Invoice>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 0,
                    PageData = new List<Invoice>()
                });

            var result = await _service.GetPagedAsync(query);

            Assert.AreEqual(0, result.Total);
            Assert.AreEqual(0, result.PageData.Count());
        }

        /// <summary>
        /// UTCID03 - Normal: Query với PaymentStatus filter → repo nhận đúng query
        /// </summary>
        [TestMethod]
        public async Task UTCID03_GetPagedAsync_WithPaymentStatusFilter_PassesQueryToRepo()
        {
            var query = new InvoiceQuery { Page = 1, PageSize = 10, PaymentStatus = "Paid" };
            _invoiceRepo.Setup(x => x.GetPagedAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<Invoice>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 1,
                    PageData = new List<Invoice>
                    {
                        new Invoice { InvoiceId = 5, BranchId = 1, PaymentStatus = "Paid", GrandTotal = 500000m }
                    }
                });

            var result = await _service.GetPagedAsync(query);

            Assert.AreEqual(1, result.Total);
            Assert.AreEqual("Paid", result.PageData.First().PaymentStatus);
            _invoiceRepo.Verify(x => x.GetPagedAsync(
                It.Is<InvoiceQuery>(q => q.PaymentStatus == "Paid"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// UTCID04 - Normal: Query với CustomerId filter → repo nhận đúng query
        /// </summary>
        [TestMethod]
        public async Task UTCID04_GetPagedAsync_WithCustomerIdFilter_PassesQueryToRepo()
        {
            var query = new InvoiceQuery { Page = 1, PageSize = 10, CustomerId = 5 };
            _invoiceRepo.Setup(x => x.GetPagedAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<Invoice>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 0,
                    PageData = new List<Invoice>()
                });

            var result = await _service.GetPagedAsync(query);

            Assert.AreEqual(0, result.Total);
            _invoiceRepo.Verify(x => x.GetPagedAsync(
                It.Is<InvoiceQuery>(q => q.CustomerId == 5),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// UTCID05 - Boundary: Page=2 với 15 records, PageSize=10 → trả 5 items còn lại
        /// </summary>
        [TestMethod]
        public async Task UTCID05_GetPagedAsync_LastPageWithPartialData_ReturnsRemainingItems()
        {
            var query = new InvoiceQuery { Page = 2, PageSize = 10 };
            var last5 = new List<Invoice>();
            for (int i = 11; i <= 15; i++)
            {
                last5.Add(new Invoice { InvoiceId = i, BranchId = 1, GrandTotal = 100000m * i });
            }

            _invoiceRepo.Setup(x => x.GetPagedAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<Invoice>
                {
                    Page = 2,
                    PageSize = 10,
                    Total = 15,
                    PageData = last5
                });

            var result = await _service.GetPagedAsync(query);

            Assert.AreEqual(15, result.Total);
            Assert.AreEqual(2, result.Page);
            Assert.AreEqual(5, result.PageData.Count());
        }
    }
}
