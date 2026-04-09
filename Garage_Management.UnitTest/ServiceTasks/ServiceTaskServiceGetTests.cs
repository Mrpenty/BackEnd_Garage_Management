using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Services.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.ServiceTasks
{
    [TestClass]
    public class ServiceTaskServiceGetTests
    {
        [TestMethod]
        public async Task GetByServiceIdAsync_ReturnsMappedList()
        {
            var repo = new Mock<IServiceTaskRepository>();
            var service = new ServiceTaskService(repo.Object);
            repo.Setup(x => x.GetByServiceIdAsync(2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ServiceTask>
                {
                    new ServiceTask{ ServiceTaskId = 1, ServiceId = 2, TaskName = "A", TaskOrder = 1, EstimateMinute = 5 },
                    new ServiceTask{ ServiceTaskId = 2, ServiceId = 2, TaskName = "B", TaskOrder = 2, EstimateMinute = 7 }
                });

            var result = await service.GetByServiceIdAsync(2);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("A", result.First().TaskName);
        }

        [TestMethod]
        public async Task GetPagedAsync_ReturnsMapped()
        {
            var repo = new Mock<IServiceTaskRepository>();
            var service = new ServiceTaskService(repo.Object);
            repo.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedResult<ServiceTask>
                {
                    Page = 1,
                    PageSize = 10,
                    Total = 1,
                    PageData = new List<ServiceTask> { new ServiceTask { ServiceTaskId = 1, ServiceId = 2, TaskName = "A", TaskOrder = 1, EstimateMinute = 5 } }
                });

            var result = await service.GetPagedAsync(1, 10);

            Assert.AreEqual(1, result.Total);
            Assert.AreEqual(1, result.PageData.Count());
        }
    }
}
