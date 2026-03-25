using Garage_Management.Application.DTOs.Vehicles.VehicleType;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Vehicles;
using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.VehicleTypes
{
    [TestClass]
    public class VehicleTypeServiceCreateTests
    {
        private Mock<IVehicleTypeRepository> _repo = null!;
        private VehicleTypeService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IVehicleTypeRepository>();
            _service = new VehicleTypeService(_repo.Object);
        }

        [TestMethod]
        public async Task CreateAsync_ValidRequest_ReturnsCreatedResponse()
        {
            var request = new VehicleTypeCreateRequest
            {
                TypeName = "Xe ga",
                Description = "Xe tay ga",
                IsActive = true
            };

            _repo.Setup(x => x.ExistsByTypeNameAsync("Xe ga", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);
            _repo.Setup(x => x.AddAsync(It.IsAny<VehicleType>(), It.IsAny<CancellationToken>()))
                .Callback<VehicleType, CancellationToken>((e, _) => e.VehicleTypeId = 7)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(7, result.VehicleTypeId);
            Assert.AreEqual("Xe ga", result.TypeName);
            Assert.AreEqual("Xe tay ga", result.Description);
            Assert.IsTrue(result.IsActive);
        }

        [TestMethod]
        public async Task CreateAsync_InvalidTypeName_Throws()
        {
            var request = new VehicleTypeCreateRequest
            {
                TypeName = "   ",
                IsActive = true
            };

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_DuplicateTypeName_Throws()
        {
            var request = new VehicleTypeCreateRequest
            {
                TypeName = "Xe may",
                IsActive = true
            };

            _repo.Setup(x => x.ExistsByTypeNameAsync("Xe may", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_TypeNameNull_Throws()
        {
            var request = new VehicleTypeCreateRequest
            {
                TypeName = null!,
                IsActive = true
            };

            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(
                () => _service.CreateAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task CreateAsync_TypeNameHasSpaces_TrimBeforeDuplicateCheck()
        {
            var request = new VehicleTypeCreateRequest
            {
                TypeName = " Xe ga ",
                Description = "Mo ta",
                IsActive = true
            };

            _repo.Setup(x => x.ExistsByTypeNameAsync("Xe ga", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _repo.Setup(x => x.AddAsync(It.IsAny<VehicleType>(), It.IsAny<CancellationToken>()))
                .Callback<VehicleType, CancellationToken>((e, _) => e.VehicleTypeId = 8)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.AreEqual(8, result.VehicleTypeId);
            Assert.AreEqual("Xe ga", result.TypeName);
            _repo.Verify(x => x.ExistsByTypeNameAsync("Xe ga", null, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateAsync_DescriptionNull_MapsNull()
        {
            var request = new VehicleTypeCreateRequest
            {
                TypeName = "Xe con tay ga",
                Description = null,
                IsActive = true
            };

            _repo.Setup(x => x.ExistsByTypeNameAsync("Xe con tay ga", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _repo.Setup(x => x.AddAsync(It.IsAny<VehicleType>(), It.IsAny<CancellationToken>()))
                .Callback<VehicleType, CancellationToken>((e, _) => e.VehicleTypeId = 9)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.AreEqual(9, result.VehicleTypeId);
            Assert.IsNull(result.Description);
        }

        [TestMethod]
        public async Task CreateAsync_ValidRequest_CallsAddAndSaveOnce()
        {
            var request = new VehicleTypeCreateRequest
            {
                TypeName = "Xe tay con",
                Description = "Loai test",
                IsActive = true
            };

            _repo.Setup(x => x.ExistsByTypeNameAsync("Xe tay con", null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _repo.Setup(x => x.AddAsync(It.IsAny<VehicleType>(), It.IsAny<CancellationToken>()))
                .Callback<VehicleType, CancellationToken>((e, _) => e.VehicleTypeId = 10)
                .Returns(Task.CompletedTask);
            _repo.Setup(x => x.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var result = await _service.CreateAsync(request, CancellationToken.None);

            Assert.IsNotNull(result);
            _repo.Verify(x => x.AddAsync(It.IsAny<VehicleType>(), It.IsAny<CancellationToken>()), Times.Once);
            _repo.Verify(x => x.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
