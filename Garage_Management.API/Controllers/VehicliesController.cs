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
        public async Task<ActionResult<ApiResponse<PagedResult<VehicleResponse>>>> GetPaged([FromQuery] ParamQuery query,CancellationToken ct = default)
        {
            var result = await _service.GetPagedAsync(query, ct);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy chi tiết 1 xe máy
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<VehicleResponse>>> GetById(int id, CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetByIdAsync(id, ct);
                if (data == null)
                    return NotFound(ApiResponse<VehicleResponse>.ErrorResponse("Không tìm thấy phương tiện"));
                return Ok(ApiResponse<VehicleResponse>.SuccessResponse(data, "OK"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<VehicleResponse>.ErrorResponse(ex.Message));
            }
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách xe máy theo khách hàng được phân trang
        /// </summary>
        [HttpGet("Customer/MyVehicle")]
        public async Task<ActionResult<ApiResponse<PagedResult<VehicleResponse>>>> GetByCustomer(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _service.GetMyVehicle(page, pageSize, ct);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
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
            try
            {
                var data = await _service.CreateAsync(request, ct);

                return CreatedAtAction(nameof(GetById),
                    new { id = data.VehicleId },
                    ApiResponse<VehicleResponse>.SuccessResponse(data, "Tạo xe thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<VehicleResponse>.ErrorResponse(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<VehicleResponse>.ErrorResponse("Lỗi hệ thống"));
            }
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
            try
            {
                var data = await _service.UpdateAsync(id, request, ct);

                if (data == null)
                    return NotFound(ApiResponse<VehicleResponse>.ErrorResponse("Không tìm thấy phương tiện"));

                return Ok(ApiResponse<VehicleResponse>.SuccessResponse(data, "Cập nhật thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<VehicleResponse>.ErrorResponse(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<VehicleResponse>.ErrorResponse("Lỗi hệ thống"));
            }
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
