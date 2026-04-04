using Garage_Management.Application.DTOs.Vehicles.VehicleBrand;
using Garage_Management.Application.Interfaces.Services.Vehiclies;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleBrandsController : ControllerBase
    {
        private readonly IVehicleBrandService _service;

        public VehicleBrandsController(IVehicleBrandService service)
        {
            _service = service;
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh brand xe máy được phân trang
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<VehicleBrandResponse>>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetPagedAsync(page, pageSize, ct);
                return Ok(ApiResponse<PagedResult<VehicleBrandResponse>>.SuccessResponse(data, "Lấy danh sách hãng xe máy thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<PagedResult<VehicleBrandResponse>>.ErrorResponse(ex.Message));

            }
        }
        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy chi tiết 1 brand xe máy
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<VehicleBrandResponse>>> GetById(int id, CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetByIdAsync(id, ct);
                if (data == null)
                    return NotFound(ApiResponse<VehicleBrandResponse>.ErrorResponse("Không tìm thấy hãng xe máy"));
                return Ok(ApiResponse<VehicleBrandResponse>.SuccessResponse(data, "OK"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<VehicleBrandResponse>.ErrorResponse(ex.Message));
            }
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Tạo 1 brand xe máy mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<VehicleBrandResponse>>> Create(
            [FromBody] VehicleBrandCreateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.CreateAsync(request, ct);
                return CreatedAtAction(nameof(GetById), new { id = data.BrandId }, ApiResponse<VehicleBrandResponse>.SuccessResponse(data, "Hãng xe máy đã được tạo thành công "));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<VehicleBrandResponse>.ErrorResponse(ex.Message));
            }
          
        }

        /// <summary>
        /// Cập nhật trạng thái isActive của brand xe máy.
        /// </summary>
        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(
            int id,
            [FromBody] VehicleBrandStatusUpdateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.UpdateStatusAsync(id, request.IsActive, ct);
                if (!data)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Không tìm thấy hãng xe máy"));

                return Ok(ApiResponse<bool>.SuccessResponse(true, "Cập nhật trạng thái thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<bool>.ErrorResponse(ex.Message));
            }
        }
        /// <summary>
        /// Xóa cứng brand xe máy.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            var deleted = await _service.DeleteAsync(id, ct);
            if (!deleted)
                return NotFound(ApiResponse<object>.ErrorResponse("VehicleBrand not found"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deleted"));
        }

    }
}
