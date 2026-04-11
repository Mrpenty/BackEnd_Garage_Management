using Garage_Management.Application.DTOs.ServiceTasks;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Services.Services;
using Garage_Management.Base.Entities.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.ServiceTasks
{
    [TestClass]
    public class ServiceTaskServiceUpdateTests
    {
        [TestMethod]
        public async Task UpdateAsync_NotFound_ReturnsNull()
        {
            var repo = new Mock<IServiceTaskRepository>();
            var service = new ServiceTaskService(repo.Object);
            repo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((ServiceTask?)null);

            var result = await service.UpdateAsync(99, new ServiceTaskUpdateRequest { TaskName = "X" });

            Assert.IsNull(result);
        }
    }
}
