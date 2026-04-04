using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Services.Workbays;
using Garage_Management.Application.DTOs.Workbays;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Common.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.WorkBays
{
    [TestClass]
    public class UpdateWorkBayAsyncTests
    {
        private Mock<IWorkBayRepository> _workBayRepositoryMock;
        private WorkBayService _workBayService;

        [TestInitialize]
        public void Setup()
        {
            _workBayRepositoryMock = new Mock<IWorkBayRepository>();
            _workBayService = new WorkBayService(_workBayRepositoryMock.Object);
        }

        [TestMethod]
        public async Task UpdateWorkBayAsync_WorkBayNotFound_ReturnsError()
        {
            // Arrange
            int id = 1;
            var request = new UpdateWorkBayRequest { Name = "Updated Bay", Note = "Updated note", Status = WorkBayStatus.Available };
            _workBayRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync((WorkBay)null);

            // Act
            var result = await _workBayService.UpdateWorkBayAsync(id, request, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Khoang sửa chữa không tồn tại", result.Message);
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public async Task UpdateWorkBayAsync_ValidUpdateToAvailable_UpdatesSuccessfully()
        {
            // Arrange
            int id = 1;
            var existingWorkBay = new WorkBay
            {
                Id = id,
                Name = "Old Bay",
                Note = "Old note",
                Status = WorkBayStatus.Occupied,
                StartAt = System.DateTime.Now.AddHours(-1),
                EndAt = null
            };
            var request = new UpdateWorkBayRequest { Name = "Updated Bay", Note = "Updated note", Status = WorkBayStatus.Available };
            _workBayRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(existingWorkBay);
            _workBayRepositoryMock.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _workBayService.UpdateWorkBayAsync(id, request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Cập nhật khoang sửa chữa thành công", result.Message);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(request.Name, result.Data.Name);
            Assert.AreEqual(request.Note, result.Data.Note);
            Assert.AreEqual(WorkBayStatus.Available, result.Data.Status);
            Assert.IsNull(result.Data.StartAt);
            Assert.IsNotNull(result.Data.EndAt);
            _workBayRepositoryMock.Verify(x => x.Update(existingWorkBay), Times.Once);
            _workBayRepositoryMock.Verify(x => x.SaveAsync(CancellationToken.None), Times.Once);
        }

        [TestMethod]
        public async Task UpdateWorkBayAsync_ValidUpdateToOccupied_UpdatesSuccessfully()
        {
            // Arrange
            int id = 1;
            var existingWorkBay = new WorkBay
            {
                Id = id,
                Name = "Old Bay",
                Note = "Old note",
                Status = WorkBayStatus.Available,
                StartAt = null,
                EndAt = System.DateTime.Now.AddHours(-1)
            };
            var request = new UpdateWorkBayRequest { Name = "Updated Bay", Note = "Updated note", Status = WorkBayStatus.Occupied };
            _workBayRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(existingWorkBay);
            _workBayRepositoryMock.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _workBayService.UpdateWorkBayAsync(id, request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Cập nhật khoang sửa chữa thành công", result.Message);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(request.Name, result.Data.Name);
            Assert.AreEqual(request.Note, result.Data.Note);
            Assert.AreEqual(WorkBayStatus.Occupied, result.Data.Status);
            Assert.IsNotNull(result.Data.StartAt);
            Assert.IsNull(result.Data.EndAt);
            _workBayRepositoryMock.Verify(x => x.Update(existingWorkBay), Times.Once);
            _workBayRepositoryMock.Verify(x => x.SaveAsync(CancellationToken.None), Times.Once);
        }

        [TestMethod]
        public async Task UpdateWorkBayAsync_ValidUpdateToUnderMaintenance_UpdatesSuccessfully()
        {
            // Arrange
            int id = 1;
            var existingWorkBay = new WorkBay
            {
                Id = id,
                Name = "Old Bay",
                Note = "Old note",
                Status = WorkBayStatus.Available
            };
            var request = new UpdateWorkBayRequest { Name = "Updated Bay", Note = "Updated note", Status = WorkBayStatus.Maintenance };
            _workBayRepositoryMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(existingWorkBay);
            _workBayRepositoryMock.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _workBayService.UpdateWorkBayAsync(id, request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Cập nhật khoang sửa chữa thành công", result.Message);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(request.Name, result.Data.Name);
            Assert.AreEqual(request.Note, result.Data.Note);
            Assert.AreEqual(WorkBayStatus.Maintenance, result.Data.Status);
            _workBayRepositoryMock.Verify(x => x.Update(existingWorkBay), Times.Once);
            _workBayRepositoryMock.Verify(x => x.SaveAsync(CancellationToken.None), Times.Once);
        }
    }
}