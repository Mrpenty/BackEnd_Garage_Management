using Garage_Management.Application.DTOs.Vehicles.VehicleModel;
using Garage_Management.Application.Interfaces.Services.Vehiclies;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleModelsController : ControllerBase
    {
        private readonly IVehicleModelService _service;

        public VehicleModelsController(IVehicleModelService service)
        {
            _service = service;
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách model xe máy được phân trang
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<VehicleModelResponse>>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(ApiResponse<PagedResult<VehicleModelResponse>>.SuccessResponse(data, "OK"));
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy chi tiết 1 model xe máy
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<VehicleModelResponse>>> GetById(int id, CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetByIdAsync(id, ct);
                if (data == null)
                    return NotFound(ApiResponse<VehicleModelResponse>.ErrorResponse("Không tìm thấy model xe máy"));
                return Ok(ApiResponse<VehicleModelResponse>.SuccessResponse(data, "OK"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<VehicleModelResponse>.ErrorResponse(ex.Message));
            }
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Tạo 1 model xe máy mới
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<VehicleModelResponse>>> Create(
            [FromBody] VehicleModelCreateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.CreateAsync(request, ct);
                return CreatedAtAction(nameof(GetById), new { id = data.ModelId }, ApiResponse<VehicleModelResponse>.SuccessResponse(data, "Model xe máy đã được tạo thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<VehicleModelResponse>.ErrorResponse(ex.Message));
            }
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Cập nhật 1 model xe máy
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<VehicleModelResponse>>> Update(
            int id,
            [FromBody] VehicleModelUpdate request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.UpdateAsync(id, request, ct);
                if (data == null)
                    return NotFound(ApiResponse<VehicleModelResponse>.ErrorResponse("Model xe máy không tìm thấy"));

                return Ok(ApiResponse<VehicleModelResponse>.SuccessResponse(data, "Cập nhật model xe máy thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<VehicleModelResponse>.ErrorResponse(ex.Message));
            }
           
        }
        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Toggle trạng thái IsActive của 1 model xe (active ↔ deactive)
        /// </summary>
        [HttpPatch("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> ToggleStatus(int id, CancellationToken ct = default)
        {
            try
            {
                var ok = await _service.DeActiveAsync(id, ct);
                if (!ok)
                    return NotFound(ApiResponse<object>.ErrorResponse("VehicleModel not found"));

                return Ok(ApiResponse<object>.SuccessResponse(new { }, "Status updated"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Xóa cứng 1 model xe máy. Chỉ cho phép khi chưa có xe liên kết.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            try
            {
                var ok = await _service.DeleteAsync(id, ct);
                if (!ok)
                    return NotFound(ApiResponse<object>.ErrorResponse("VehicleModel not found"));

                return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deleted"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}
