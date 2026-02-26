using Garage_Management.Application.DTOs.Inventories.SparePartBrands;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SparePartBrandsController : ControllerBase
    {
        private readonly ISparePartBrandService _service;

        public SparePartBrandsController(ISparePartBrandService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách hãng phụ tùng được phân trang
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<SparePartBrandResponse>>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(ApiResponse<PagedResult<SparePartBrandResponse>>.SuccessResponse(data, "OK"));
        }

        /// <summary>
        /// Lấy chi tiết 1 hãng phụ tùng
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<SparePartBrandResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<SparePartBrandResponse>.ErrorResponse("SparePartBrand not found"));

            return Ok(ApiResponse<SparePartBrandResponse>.SuccessResponse(data, "OK"));
        }

        /// <summary>
        /// Tạo hãng phụ tùng
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<SparePartBrandResponse>>> Create(
            [FromBody] SparePartBrandCreateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = data.SparePartBrandId }, ApiResponse<SparePartBrandResponse>.SuccessResponse(data, "Created"));
        }

        /// <summary>
        /// Cập nhật hãng phụ tùng
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<SparePartBrandResponse>>> Update(
            int id,
            [FromBody] SparePartBrandUpdateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.UpdateAsync(id, request, ct);
            if (data == null)
                return NotFound(ApiResponse<SparePartBrandResponse>.ErrorResponse("SparePartBrand not found"));

            return Ok(ApiResponse<SparePartBrandResponse>.SuccessResponse(data, "Updated"));
        }

        /// <summary>
        /// Xóa hãng phụ tùng
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok)
                return NotFound(ApiResponse<object>.ErrorResponse("SparePartBrand not found"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deleted"));
        }
    }
}
