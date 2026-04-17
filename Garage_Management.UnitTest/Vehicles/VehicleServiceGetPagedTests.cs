using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Services.Vehicles;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Vehiclies;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Garage_Management.UnitTest.Vehicles
{
    [TestClass]
    public class VehicleServiceGetPagedTests
    {
        private Mock<IVehicleRepository> _repo = null!;
        private VehicleService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<IVehicleRepository>();
            _service = new VehicleService(
                _repo.Object,
                new Mock<ICustomerRepository>().Object,
                new Mock<IVehicleModelRepository>().Object,
                new Mock<IHttpContextAccessor>().Object);
        }

        [TestMethod]
        public async Task GetPagedAsync_WithData_ReturnsSuccess()
        {
            var query = new ParamQuery { Page = 1, PageSize = 10 };
            var repoData = new PagedResult<Vehicle>
            {
                Page = 1,
                PageSize = 10,
                Total = 1,
                PageData = new List<Vehicle>
                {
                    new Vehicle
                    {
                        VehicleId = 1,
                        CustomerId = 2,
                        ModelId = 3,
                        Brand = new VehicleBrand { BrandName = "Yamaha" },
                        Model = new VehicleModel { ModelName = "Exciter" }
                    }
                }
            };
            _repo.Setup(x => x.GetPagedAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ApiResponse<PagedResult<Vehicle>>.SuccessResponse(repoData, "OK"));

            var result = await _service.GetPagedAsync(query, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.Data.Total);
            Assert.AreEqual(1, result.Data.PageData.Count());
            Assert.AreEqual("Lấy danh sách phương tiện thành công", result.Message);
            _repo.Verify(x => x.GetPagedAsync(query, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task GetPagedAsync_Empty_ReturnsSuccessWithEmptyMessage()
        {
            var query = new ParamQuery { Page = 1, PageSize = 10 };
            var repoData = new PagedResult<Vehicle>
            {
                Page = 1,
                PageSize = 10,
                Total = 0,
                PageData = new List<Vehicle>()
            };
            _repo.Setup(x => x.GetPagedAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ApiResponse<PagedResult<Vehicle>>.SuccessResponse(repoData, "OK"));

            var result = await _service.GetPagedAsync(query, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.Data.Total);
            Assert.AreEqual("Không có phương tiện nào", result.Message);
            _repo.Verify(x => x.GetPagedAsync(query, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task GetPagedAsync_RepoError_ReturnsErrorResponse()
        {
            var query = new ParamQuery { Page = 1, PageSize = 10 };
            _repo.Setup(x => x.GetPagedAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(ApiResponse<PagedResult<Vehicle>>.ErrorResponse("DB lỗi"));

            var result = await _service.GetPagedAsync(query, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("DB lỗi", result.Message);
            _repo.Verify(x => x.GetPagedAsync(query, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
