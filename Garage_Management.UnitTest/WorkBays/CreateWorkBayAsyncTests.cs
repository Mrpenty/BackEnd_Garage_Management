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
    public class CreateWorkBayAsyncTests
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
        public async Task CreateWorkBayAsync_ValidRequest_CreatesWorkBaySuccessfully()
        {
            // Arrange
            var request = new CreateWorkBayRequest
            {
                Name = "New Bay",
                Note = "Test note"
            };
            var createdWorkBay = new WorkBay
            {
                Id = 1,
                Name = request.Name,
                Note = request.Note,
                Status = WorkBayStatus.Available,
                CreateAt = System.DateTime.Now 
            };
            _workBayRepositoryMock.Setup(x => x.AddAsync(It.IsAny<WorkBay>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _workBayRepositoryMock.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            var result = await _workBayService.CreateWorkBayAsync(request, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("Tạo khoang sửa chữa thành công", result.Message);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(request.Name, result.Data.Name);
            Assert.AreEqual(request.Note, result.Data.Note);
            Assert.AreEqual(WorkBayStatus.Available, result.Data.Status);
            _workBayRepositoryMock.Verify(x => x.AddAsync(It.IsAny<WorkBay>(), CancellationToken.None), Times.Once);
            _workBayRepositoryMock.Verify(x => x.SaveAsync(CancellationToken.None), Times.Once);
        }

        [TestMethod]
        public async Task CreateWorkBayAsync_EmptyNameAndNote_CreatesFail()
        {
            // Arrange
            var request = new CreateWorkBayRequest { Name = "", Note = "" };
            _workBayRepositoryMock.Setup(x => x.AddAsync(It.IsAny<WorkBay>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _workBayRepositoryMock.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var result = await _workBayService.CreateWorkBayAsync(request, CancellationToken.None);
            Assert.IsFalse(result.Success);
            Assert.IsNull(result.Data);
        }
    }
}