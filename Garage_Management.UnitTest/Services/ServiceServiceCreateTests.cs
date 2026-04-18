using Garage_Management.Application.DTOs.Services;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Services.Services;
using Garage_Management.Base.Entities.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Services
{
    [TestClass]
    public class ServiceServiceCreateTests
    {
        private Mock<IServiceRepository> _repo;
        private ServiceService _service;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IServiceRepository>();
            _service = new ServiceService(_repo.Object);
        }

        /// <summary>
        /// UTCID01 - Normal: Tạo dịch vụ mới thành công với tên và mô tả hợp lệ
        /// Lưu ý: Code luôn set BasePrice=null và IsActive=false khi tạo mới.
        /// Muốn gán giá phải gọi UpdatePriceAsync, kích hoạt phải gọi UpdateStatusAsync.
        /// </summary>
        [TestMethod]
        public async Task UTCID01_CreateAsync_WithValidRequest_ReturnsCreatedResponse()
        {
            var request = new ServiceCreateRequest
            {
                ServiceName = "Bảo dưỡng",
                Description = "Dịch vụ bảo dưỡng định kỳ"
            };

            _repo.Setup(x => x.ExistsByNameAsync("Bảo dưỡng", It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.AddAsync(It.IsAny<Service>(), It.IsAny<CancellationToken>()))
                .Callback<Service, CancellationToken>((e, _) => e.ServiceId = 5)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.ServiceId);
            Assert.AreEqual("Bảo dưỡng", result.ServiceName);
            Assert.AreEqual("Dịch vụ bảo dưỡng định kỳ", result.Description);
            Assert.IsNull(result.BasePrice);
            Assert.IsFalse(result.IsActive);
            Assert.AreEqual(0, result.TotalEstimateMinute);
        }

        /// <summary>
        /// UTCID02 - Abnormal: ServiceName toàn khoảng trắng
        /// </summary>
        [TestMethod]
        public async Task UTCID02_CreateAsync_WithWhitespaceServiceName_Throws()
        {
            var request = new ServiceCreateRequest
            {
                ServiceName = "   ",
                Description = "Dịch vụ test"
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
            Assert.AreEqual("ServiceName không hợp lệ", ex.Message);
        }

        /// <summary>
        /// UTCID03 - Abnormal: ServiceName là chuỗi rỗng
        /// </summary>
        [TestMethod]
        public async Task UTCID03_CreateAsync_WithEmptyServiceName_Throws()
        {
            var request = new ServiceCreateRequest
            {
                ServiceName = "",
                Description = "Dịch vụ test"
            };

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
            Assert.AreEqual("ServiceName không hợp lệ", ex.Message);
        }

        /// <summary>
        /// UTCID04 - Abnormal: ServiceName đã tồn tại trong hệ thống
        /// </summary>
        [TestMethod]
        public async Task UTCID04_CreateAsync_WithDuplicateServiceName_Throws()
        {
            var request = new ServiceCreateRequest
            {
                ServiceName = "Rửa xe",
                Description = "Dịch vụ rửa xe"
            };

            _repo.Setup(x => x.ExistsByNameAsync("Rửa xe", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
            Assert.AreEqual("ServiceName đã tồn tại", ex.Message);
        }
    }
}
