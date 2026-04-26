using Garage_Management.Application.DTOs.Inventories.SparePartBrands;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SparePartBrandsController : ControllerBase
    {
        private const string ManagerRoles = "Supervisor";
        private const string AdminOnly = "Admin";

        private readonly ISparePartBrandService _service;

        public SparePartBrandsController(ISparePartBrandService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách hãng phụ tùng được phân trang (dành riêng cho quản lý, có thể filter isActive = true và false)
        /// </summary>
        [HttpGet("private")]
        public async Task<ActionResult<ApiResponse<PagedResult<SparePartBrandResponse>>>> GetPaged(
            [FromQuery] ParamQuery query,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(query, false, ct);
            return Ok(ApiResponse<PagedResult<SparePartBrandResponse>>.SuccessResponse(data, "OK"));
        }

        /// <summary>
        /// Lấy danh sách hàng phụ tùng dành riêng cho khách hàng (isActive = true)   
        /// </summary>
        [HttpGet("public")]
        public async Task<ActionResult<ApiResponse<PagedResult<SparePartBrandResponse>>>> GetPagedPublic(
            [FromQuery] ParamQuery query,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(query, true, ct);
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
        [Authorize(Roles = ManagerRoles)]
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
        [Authorize(Roles = ManagerRoles)]
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
        [Authorize(Roles = AdminOnly)]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok)
                return NotFound(ApiResponse<object>.ErrorResponse("SparePartBrand not found"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deleted"));
        }
    }
}
