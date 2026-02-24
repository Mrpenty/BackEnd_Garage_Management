using Garage_Management.Application.DTOs.Vehicles;
using Garage_Management.Application.Interfaces.Services.Vehiclies;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicliesController : ControllerBase
    {
        private readonly IVehicleService _service;

        public VehicliesController(IVehicleService service)
        {
            _service = service;
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách xe máy được phân trang
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<VehicleResponse>>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(ApiResponse<PagedResult<VehicleResponse>>.SuccessResponse(data, "OK"));
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy chi tiết 1 xe máy
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<VehicleResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<VehicleResponse>.ErrorResponse("Vehicle not found"));

            return Ok(ApiResponse<VehicleResponse>.SuccessResponse(data, "OK"));
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách xe máy theo khách hàng được phân trang
        /// </summary>
        [HttpGet("by-customer/{customerId:int}")]
        public async Task<ActionResult<ApiResponse<PagedResult<VehicleResponse>>>> GetByCustomer(
            int customerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var data = await _service.GetByCustomerIdAsync(page, pageSize, customerId, ct);
            return Ok(ApiResponse<PagedResult<VehicleResponse>>.SuccessResponse(data, "OK"));
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Tạo 1 thông tin xe máy mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<VehicleResponse>>> Create(
            [FromBody] VehicleCreateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = data.VehicleId }, ApiResponse<VehicleResponse>.SuccessResponse(data, "Created"));
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Cập nhật thông tin xe máy
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<VehicleResponse>>> Update(
            int id,
            [FromBody] VehicleUpdateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.UpdateAsync(id, request, ct);
            if (data == null)
                return NotFound(ApiResponse<VehicleResponse>.ErrorResponse("Vehicle not found"));

            return Ok(ApiResponse<VehicleResponse>.SuccessResponse(data, "Updated"));
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Xóa thông tin xe máy
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok)
                return NotFound(ApiResponse<object>.ErrorResponse("Vehicle not found"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deleted"));
        }
    }
}
