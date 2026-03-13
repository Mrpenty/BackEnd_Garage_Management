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
    public class ServiceServiceGetByVehicleTypeTests
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
        public async Task GetByVehicleTypeAsync_InvalidVehicleTypeId_Throws()
        {
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _service.GetByVehicleTypeAsync(0, CancellationToken.None));
        }

        [TestMethod]
        public async Task GetByVehicleTypeAsync_ValidVehicleTypeId_ReturnsList()
        {
            _repo.Setup(x => x.GetByVehicleTypeAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Service>
                {
                    new Service { ServiceId = 12, ServiceName = "Rua xe may", BasePrice = 50000m, ServiceTasks = new List<ServiceTask>() },
                    new Service { ServiceId = 13, ServiceName = "Thay dau", BasePrice = 120000m, ServiceTasks = new List<ServiceTask>() }
                });

            var result = await _service.GetByVehicleTypeAsync(1, CancellationToken.None);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(12, result[0].ServiceId);
            Assert.AreEqual(13, result[1].ServiceId);
        }
    }
}

