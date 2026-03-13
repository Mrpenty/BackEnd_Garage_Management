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

        [TestMethod]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            _repo.Setup(x => x.GetByIdAsync(100)).ReturnsAsync((Service?)null);

            var result = await _service.GetByIdAsync(100, CancellationToken.None);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_Found_ReturnsMappedResponse()
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
    }
}

