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
            var data = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(ApiResponse<PagedResult<VehicleBrandResponse>>.SuccessResponse(data, "OK"));
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy chi tiết 1 brand xe máy
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<VehicleBrandResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<VehicleBrandResponse>.ErrorResponse("VehicleBrand not found"));

            return Ok(ApiResponse<VehicleBrandResponse>.SuccessResponse(data, "OK"));
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
            var data = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = data.BrandId }, ApiResponse<VehicleBrandResponse>.SuccessResponse(data, "Created"));
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Cập nhật brand xe máy
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<VehicleBrandResponse>>> Update(
            int id,
            [FromBody] VehicleBrandUpdate request,
            CancellationToken ct = default)
        {
            var data = await _service.UpdateAsync(id, request, ct);
            if (data == null)
                return NotFound(ApiResponse<VehicleBrandResponse>.ErrorResponse("VehicleBrand not found"));

            return Ok(ApiResponse<VehicleBrandResponse>.SuccessResponse(data, "Updated"));
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Deactive 1 brand xe máy
        /// </summary>
        [HttpPatch("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _service.DeActiveAsync(id, ct);
            if (!ok)
                return NotFound(ApiResponse<object>.ErrorResponse("VehicleBrand not found"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deactivated"));
        }
    }
}
