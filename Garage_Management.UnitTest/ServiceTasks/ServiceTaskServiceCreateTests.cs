using Garage_Management.Application.DTOs.ServiceTasks;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Services.Services;
using Garage_Management.Base.Entities.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.ServiceTasks
{
    [TestClass]
    public class ServiceTaskServiceCreateTests
    {
        [TestMethod]
        public async Task CreateAsync_DuplicateTaskOrder_Throws()
        {
            var repo = new Mock<IServiceTaskRepository>();
            var service = new ServiceTaskService(repo.Object);
            repo.Setup(x => x.HasExistAsync(1, 1, null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                service.CreateAsync(new ServiceTaskCreateRequest { ServiceId = 1, TaskName = "A", TaskOrder = 1, EstimateMinute = 5 }));
        }

        [TestMethod]
        public async Task CreateAsync_Valid_ReturnsResponse()
        {
            var repo = new Mock<IServiceTaskRepository>();
            var service = new ServiceTaskService(repo.Object);

            repo.Setup(x => x.HasExistAsync(2, 2, null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            repo.Setup(x => x.AddAsync(It.IsAny<ServiceTask>(), It.IsAny<CancellationToken>()))
                .Callback<ServiceTask, CancellationToken>((e, _) => e.ServiceTaskId = 20)
                .Returns(Task.CompletedTask);
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await service.CreateAsync(new ServiceTaskCreateRequest { ServiceId = 2, TaskName = "Kiểm tra bugi", TaskOrder = 2, EstimateMinute = 10 });

            Assert.AreEqual(20, result.ServiceTaskId);
        }
    }
}
