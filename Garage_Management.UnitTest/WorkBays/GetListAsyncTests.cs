using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.JobCards;
using Garage_Management.Application.Services.Workbays;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.UnitTest.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.WorkBays
{
    [TestClass]
    public class GetListAsyncTests
    {
        private Mock<IWorkBayRepository> _workBayRepositoryMock;
        private Mock<IJobCardRepository> _jobCardRepositoryMock;
        private WorkBayService _workBayService;

        [TestInitialize]
        public void Setup()
        {
            _workBayRepositoryMock = new Mock<IWorkBayRepository>();
            _jobCardRepositoryMock = new Mock<IJobCardRepository>();
            _workBayService = new WorkBayService(_workBayRepositoryMock.Object, _jobCardRepositoryMock.Object);
        }

        [TestMethod]
        public async Task GetListAsync_NoStatusFilter_ReturnsAllWorkBays()
        {
            // Arrange
            var workBays = new List<WorkBay>
            {
                new WorkBay { Id = 1, Name = "Bay1", Status = WorkBayStatus.Available },
                new WorkBay { Id = 2, Name = "Bay2", Status = WorkBayStatus.Occupied },
                new WorkBay { Id = 3, Name = "Bay3", Status = WorkBayStatus.Maintenance}
            };
           
            _workBayRepositoryMock.Setup(x => x.GetByStatusAsync(null, It.IsAny<CancellationToken>())).ReturnsAsync(workBays);
            _jobCardRepositoryMock.Setup(x => x.GetByWorkBayIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<JobCard>());

            var result = await _workBayService.GetListAsync(null, CancellationToken.None);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Bay1", result[0].Name);
            Assert.AreEqual("Bay2", result[1].Name);
            Assert.AreEqual("Bay3", result[2].Name);
        }

        [TestMethod]
        public async Task GetListAsync_FilterByAvailableStatus_ReturnsAvailableWorkBays()
        {
            // Arrange
            var workBays = new List<WorkBay>
            {
                new WorkBay { Id = 1, Name = "Bay1", Status = WorkBayStatus.Available },
                new WorkBay { Id = 2, Name = "Bay2", Status = WorkBayStatus.Occupied },
                new WorkBay { Id = 3, Name = "Bay3", Status = WorkBayStatus.Available }
            };
            _workBayRepositoryMock
                .Setup(x => x.GetByStatusAsync(It.IsAny<WorkBayStatus?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((WorkBayStatus? status, CancellationToken _) =>
                {
                    return status == null
                        ? workBays
                        : workBays.Where(x => x.Status == status).ToList();
                }); 
            _jobCardRepositoryMock.Setup(x => x.GetByWorkBayIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<JobCard>());

            var result = await _workBayService.GetListAsync(WorkBayStatus.Available, CancellationToken.None);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Bay1", result[0].Name);
            Assert.AreEqual("Bay3", result[1].Name);
        }

        [TestMethod]
        public async Task GetListAsync_FilterByOccupiedStatus_ReturnsOccupiedWorkBays()
        {
            // Arrange
            var workBays = new List<WorkBay>
            {
                new WorkBay { Id = 1, Name = "Bay1", Status = WorkBayStatus.Available },
                new WorkBay { Id = 2, Name = "Bay2", Status = WorkBayStatus.Occupied },
                new WorkBay { Id = 3, Name = "Bay3", Status = WorkBayStatus.Maintenance }
            };
            _workBayRepositoryMock
                           .Setup(x => x.GetByStatusAsync(It.IsAny<WorkBayStatus?>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((WorkBayStatus? status, CancellationToken _) =>
                           {
                               return status == null
                                   ? workBays
                                   : workBays.Where(x => x.Status == status).ToList();
                           });
            _jobCardRepositoryMock.Setup(x => x.GetByWorkBayIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<JobCard>());
            var result = await _workBayService.GetListAsync(WorkBayStatus.Occupied, CancellationToken.None);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Bay2", result[0].Name);
        }

        [TestMethod]
        public async Task GetListAsync_FilterByUnderMaintenanceStatus_ReturnsUnderMaintenanceWorkBays()
        {
            // Arrange
            var workBays = new List<WorkBay>
            {
                new WorkBay { Id = 1, Name = "Bay1", Status = WorkBayStatus.Available },
                new WorkBay { Id = 2, Name = "Bay2", Status = WorkBayStatus.Maintenance },
                new WorkBay { Id = 3, Name = "Bay3", Status = WorkBayStatus.Maintenance }
            };
            _workBayRepositoryMock.Setup(x => x.Query()).Returns(workBays.AsQueryable());

            _workBayRepositoryMock
                .Setup(x => x.GetByStatusAsync(It.IsAny<WorkBayStatus?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((WorkBayStatus? status, CancellationToken _) =>
                {
                    return status == null
                        ? workBays
                        : workBays.Where(x => x.Status == status).ToList();
                });
            _jobCardRepositoryMock.Setup(x => x.GetByWorkBayIdsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<JobCard>());
            var result = await _workBayService.GetListAsync(WorkBayStatus.Maintenance, CancellationToken.None);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Bay2", result[0].Name);
            Assert.AreEqual("Bay3", result[1].Name);
        }

    }
}