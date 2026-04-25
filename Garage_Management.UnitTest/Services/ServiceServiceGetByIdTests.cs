using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Services.Services;
using Garage_Management.Base.Entities.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Services
{
    [TestClass]
    public class ServiceServiceGetByIdTests
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
        /// UTCID01 - Abnormal: Id không tồn tại trong DB → trả null
        /// </summary>
        [TestMethod]
        public async Task UTCID01_GetByIdAsync_NotFound_ReturnsNull()
        {
            _repo.Setup(x => x.GetByIdAsync(100)).ReturnsAsync((Service?)null);

            var result = await _service.GetByIdAsync(100, CancellationToken.None);

            Assert.IsNull(result);
        }

        /// <summary>
        /// UTCID02 - Normal: Tìm thấy service với 2 ServiceTasks → map đầy đủ kèm TotalEstimateMinute = sum
        /// </summary>
        [TestMethod]
        public async Task UTCID02_GetByIdAsync_Found_ReturnsMappedResponse()
        {
            var entity = new Service
            {
                ServiceId = 1,
                ServiceName = "Rua xe",
                BasePrice = 100000m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ServiceTasks = new List<ServiceTask>
                {
                    new ServiceTask { ServiceTaskId = 1, ServiceId = 1, TaskName = "A", TaskOrder = 1, EstimateMinute = 5, CreatedAt = DateTime.UtcNow },
                    new ServiceTask { ServiceTaskId = 2, ServiceId = 1, TaskName = "B", TaskOrder = 2, EstimateMinute = 10, CreatedAt = DateTime.UtcNow }
                }
            };
            _repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(1, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.ServiceId);
            Assert.AreEqual(15L, result.TotalEstimateMinute);
            Assert.AreEqual(2, result.ServiceTasks.Count);
        }

        /// <summary>
        /// UTCID03 - Boundary: id = 0 → fail-fast, không gọi repo, trả null
        /// </summary>
        [TestMethod]
        public async Task UTCID03_GetByIdAsync_IdZero_ReturnsNullWithoutCallingRepo()
        {
            var result = await _service.GetByIdAsync(0, CancellationToken.None);

            Assert.IsNull(result);
            _repo.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        /// <summary>
        /// UTCID04 - Abnormal: id âm → fail-fast, không gọi repo, trả null
        /// </summary>
        [TestMethod]
        public async Task UTCID04_GetByIdAsync_NegativeId_ReturnsNullWithoutCallingRepo()
        {
            var result = await _service.GetByIdAsync(-1, CancellationToken.None);

            Assert.IsNull(result);
            _repo.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        /// <summary>
        /// UTCID05 - Boundary: Entity có ServiceTasks = null (edge case) → Map không NRE, trả TotalEstimateMinute=0, ServiceTasks rỗng
        /// </summary>
        [TestMethod]
        public async Task UTCID05_GetByIdAsync_EntityWithNullServiceTasks_ReturnsEmptyTasks()
        {
            var entity = new Service
            {
                ServiceId = 7,
                ServiceName = "Service không có task",
                BasePrice = 50000m,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ServiceTasks = null!
            };
            _repo.Setup(x => x.GetByIdAsync(7)).ReturnsAsync(entity);

            var result = await _service.GetByIdAsync(7, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(7, result.ServiceId);
            Assert.AreEqual(0L, result.TotalEstimateMinute);
            Assert.IsNotNull(result.ServiceTasks);
            Assert.AreEqual(0, result.ServiceTasks.Count);
        }
    }
}

