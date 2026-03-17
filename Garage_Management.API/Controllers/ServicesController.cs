using Garage_Management.Application.DTOs.Services;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _service;

        public ServicesController(IServiceService service)
        {
            _service = service;
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách dịch vụ được phân trang
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ServiceResponse>>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(ApiResponse<PagedResult<ServiceResponse>>.SuccessResponse(data, "OK"));
        }

        ///Author: KhanhDV
        ///Created Date: 6-3-2026
        /// <summary>
        /// Lấy danh sách dịch vụ theo loại xe (VehicleType)
        /// </summary>
        [HttpGet("by-vehicle-type/{vehicleTypeId:int}")]
        public async Task<ActionResult<ApiResponse<List<ServiceResponse>>>> GetByVehicleType(
            int vehicleTypeId,
            CancellationToken ct = default)
        {
            var data = await _service.GetByVehicleTypeAsync(vehicleTypeId, ct);
            return Ok(ApiResponse<List<ServiceResponse>>.SuccessResponse(data, "OK"));
        }

        [HttpGet("service-vehicle-type-pairs")]
        public async Task<ActionResult<ApiResponse<PagedResult<ServiceVehicleTypePairResponse>>>> GetServiceVehicleTypePairs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _service.GetServiceVehicleTypePairsAsync(page, pageSize, ct);
            return Ok(ApiResponse<PagedResult<ServiceVehicleTypePairResponse>>.SuccessResponse(result, "OK"));
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy chi tiết 1 dịch vụ
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<ServiceResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<ServiceResponse>.ErrorResponse("Service not found"));

            return Ok(ApiResponse<ServiceResponse>.SuccessResponse(data, "OK"));
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Tạo 1 dịch vụ mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ServiceResponse>>> Create(
            [FromBody] ServiceCreateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = data.ServiceId }, ApiResponse<ServiceResponse>.SuccessResponse(data, "Created"));
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Xóa 1 dịch vụ
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok)
                return NotFound(ApiResponse<object>.ErrorResponse("Service not found"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deleted"));
        }

        /// <summary>
        /// Deactivate 1 service.
        /// </summary>
        [HttpPatch("{id:int}/deactivate")]
        public async Task<ActionResult<ApiResponse<ServiceResponse>>> Deactivate(int id, CancellationToken ct = default)
        {
            var data = await _service.DeactivateAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<ServiceResponse>.ErrorResponse("Service not found"));

            return Ok(ApiResponse<ServiceResponse>.SuccessResponse(data, "Deactivated"));
        }
    }
}
