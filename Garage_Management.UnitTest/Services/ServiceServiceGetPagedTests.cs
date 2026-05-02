using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Services.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Services
{
    [TestClass]
    public class ServiceServiceGetPagedTests
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
        public async Task GetPagedAsync_WithData_ReturnsMappedPagedResult()
        {
            var paged = new PagedResult<Service>
            {
                Page = 1,
                PageSize = 10,
                Total = 1,
                PageData = new List<Service>
                {
                    new Service
                    {
                        ServiceId = 2,
                        ServiceName = "Thay nhot",
                        BasePrice = 120000m,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        ServiceTasks = new List<ServiceTask>
                        {
                            new ServiceTask { ServiceTaskId = 1, ServiceId = 2, TaskName = "A", TaskOrder = 1, EstimateMinute = 15, CreatedAt = DateTime.UtcNow }
                        }
                    }
                }
            };
            _repo.Setup(x => x.GetPagedAsync(
                1, 10,
                It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<int?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paged);

            var result = await _service.GetPagedAsync(1, 10, ct: CancellationToken.None);

            Assert.AreEqual(1, result.Total);
            Assert.AreEqual(1, result.PageData.Count());
            Assert.AreEqual(15L, result.PageData.First().TotalEstimateMinute);
        }

        [TestMethod]
        public async Task GetPagedAsync_Empty_ReturnsEmptyPagedResult()
        {
            _repo.Setup(x => x.GetPagedAsync(
                2, 5,
                It.IsAny<string?>(), It.IsAny<bool?>(), It.IsAny<bool?>(),
                It.IsAny<int?>(), It.IsAny<decimal?>(), It.IsAny<decimal?>(),
                It.IsAny<string?>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<Service>
                {
                    Page = 2,
                    PageSize = 5,
                    Total = 0,
                    PageData = new List<Service>()
                });

            var result = await _service.GetPagedAsync(2, 5, ct: CancellationToken.None);

            Assert.AreEqual(2, result.Page);
            Assert.AreEqual(5, result.PageSize);
            Assert.AreEqual(0, result.Total);
            Assert.AreEqual(0, result.PageData.Count());
        }
    }
}
