using Garage_Management.Application.Interfaces.Repositories.Invoices;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Services.Invoices;
using Garage_Management.Base.Entities;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.Vehiclies;
using Garage_Management.UnitTest.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Invoices
{
    [TestClass]
    public class InvoiceServiceGetByIdTests
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
        /// UTCID01 - Abnormal: Id không tồn tại → return null
        /// </summary>
        [TestMethod]
        public async Task UTCID01_GetByIdAsync_NotFound_ReturnsNull()
        {
            _invoiceRepo.Setup(x => x.GetByIdWithDetailsAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Invoice?)null);

            var result = await _service.GetByIdAsync(99);

            Assert.IsNull(result);
        }

        /// <summary>
        /// UTCID02 - Normal: Found với JobCard.Customer + Vehicle navigation đầy đủ → trả CustomerName + LicensePlate
        /// </summary>
        [TestMethod]
        public async Task UTCID02_GetByIdAsync_FoundWithFullDetails_ReturnsMappedResponse()
        {
            var entity = new Invoice
            {
                InvoiceId = 1,
                JobCardId = 10,
                BranchId = 1,
                InvoiceDate = new DateTime(2026, 4, 20, 9, 0, 0, DateTimeKind.Utc),
                ServiceTotal = 200000m,
                SparePartTotal = 150000m,
                GrandTotal = 350000m,
                PaymentStatus = "Paid",
                PaymentMethod = "Bank",
                CreatedAt = new DateTime(2026, 4, 20, 9, 0, 0, DateTimeKind.Utc),
                JobCard = new JobCard
                {
                    JobCardId = 10,
                    BranchId = 1,
                    Customer = new Customer { CustomerId = 5, FirstName = "Khánh", LastName = "Đỗ" },
                    Vehicle = new Vehicle { VehicleId = 3, LicensePlate = "29A-12345" }
                }
            };
            _invoiceRepo.Setup(x => x.GetByIdWithDetailsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.InvoiceId);
            Assert.AreEqual(10, result.JobCardId);
            Assert.AreEqual(200000m, result.ServiceTotal);
            Assert.AreEqual(150000m, result.SparePartTotal);
            Assert.AreEqual(350000m, result.GrandTotal);
            Assert.AreEqual("Paid", result.PaymentStatus);
            Assert.AreEqual("Bank", result.PaymentMethod);
            Assert.AreEqual("Khánh Đỗ", result.CustomerName);
            Assert.AreEqual("29A-12345", result.VehicleLicensePlate);
        }

        /// <summary>
        /// UTCID03 - Boundary: Found nhưng JobCard navigation null → CustomerName + LicensePlate đều null, không NRE
        /// </summary>
        [TestMethod]
        public async Task UTCID03_GetByIdAsync_FoundWithoutJobCardNav_ReturnsResponseWithNullCustomerInfo()
        {
            var entity = new Invoice
            {
                InvoiceId = 2,
                JobCardId = 11,
                BranchId = 1,
                ServiceTotal = 100000m,
                SparePartTotal = 50000m,
                GrandTotal = 150000m,
                PaymentStatus = "Unpaid",
                JobCard = null!
            };
            _invoiceRepo.Setup(x => x.GetByIdWithDetailsAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(2);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.InvoiceId);
            Assert.IsNull(result.CustomerName);
            Assert.IsNull(result.VehicleLicensePlate);
        }

        /// <summary>
        /// UTCID04 - Boundary: JobCard tồn tại nhưng Customer null → CustomerName null, LicensePlate vẫn lấy được
        /// </summary>
        [TestMethod]
        public async Task UTCID04_GetByIdAsync_FoundWithoutCustomerNav_ReturnsResponseWithNullCustomerName()
        {
            var entity = new Invoice
            {
                InvoiceId = 3,
                JobCardId = 12,
                BranchId = 1,
                ServiceTotal = 80000m,
                SparePartTotal = 20000m,
                GrandTotal = 100000m,
                PaymentStatus = "Unpaid",
                JobCard = new JobCard
                {
                    JobCardId = 12,
                    BranchId = 1,
                    Customer = null!,
                    Vehicle = new Vehicle { VehicleId = 4, LicensePlate = "30B-99999" }
                }
            };
            _invoiceRepo.Setup(x => x.GetByIdWithDetailsAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(3);

            Assert.IsNotNull(result);
            Assert.IsNull(result.CustomerName);
            Assert.AreEqual("30B-99999", result.VehicleLicensePlate);
        }
    }
}
