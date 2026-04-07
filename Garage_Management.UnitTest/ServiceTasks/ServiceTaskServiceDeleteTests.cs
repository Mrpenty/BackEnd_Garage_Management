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
    public class ServiceTaskServiceDeleteTests
    {
        [TestMethod]
        public async Task DeleteAsync_NotFound_ReturnsFalse()
        {
            var repo = new Mock<IServiceTaskRepository>();
            var service = new ServiceTaskService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((ServiceTask?)null);

            var ok = await service.DeleteAsync(99);
            Assert.IsFalse(ok);
        }

        [TestMethod]
        public async Task DeleteAsync_Found_ReturnsTrue()
        {
            var repo = new Mock<IServiceTaskRepository>();
            var service = new ServiceTaskService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new ServiceTask { ServiceTaskId = 1, ServiceId = 2, TaskName = "A" });
            repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var ok = await service.DeleteAsync(1);
            Assert.IsTrue(ok);
        }
    }
}
