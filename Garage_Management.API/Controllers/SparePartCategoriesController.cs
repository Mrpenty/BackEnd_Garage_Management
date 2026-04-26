using Garage_Management.Application.DTOs.Inventories.SparePartCategories;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SparePartCategoriesController : ControllerBase
    {
        private const string ManagerRoles = "Supervisor";

        private readonly ISparePartCategoryService _service;

        public SparePartCategoriesController(ISparePartCategoryService service)
        {
            _service = service;
        }
        /// <summary>
        /// Hiện danh sách nhóm phụ tùng dành cho quản lý (hiện cả isActive = false)
        /// </summary>
        /// <param name="query"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("private")]
        public async Task<ActionResult<ApiResponse<PagedResult<SparePartCategoryResponse>>>> GetPaged([FromQuery] ParamQuery query, CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(query, false, ct);
            return Ok(ApiResponse<PagedResult<SparePartCategoryResponse>>.SuccessResponse(data, "OK"));
        }
        /// <summary>
        /// Hiện danh sách nhóm phụ tùng dành cho khách hàng (chỉ hiện isActive = false)
        /// </summary>
        /// <param name="query"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("public")]
        public async Task<ActionResult<ApiResponse<PagedResult<SparePartCategoryResponse>>>> GetPagedPublic([FromQuery] ParamQuery query, CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(query, true, ct);
            return Ok(ApiResponse<PagedResult<SparePartCategoryResponse>>.SuccessResponse(data, "OK"));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<SparePartCategoryResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<SparePartCategoryResponse>.ErrorResponse("SparePartCategory not found"));

            return Ok(ApiResponse<SparePartCategoryResponse>.SuccessResponse(data, "OK"));
        }

        [HttpPost]
        [Authorize(Roles = ManagerRoles)]
        public async Task<ActionResult<ApiResponse<SparePartCategoryResponse>>> Create([FromBody] SparePartCategoryCreateRequest request, CancellationToken ct = default)
        {
            var data = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = data.CategoryId }, ApiResponse<SparePartCategoryResponse>.SuccessResponse(data, "Created"));
        }
        /// <summary>
        /// Chỉ cập nhật được mô tả và tên
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = ManagerRoles)]
        public async Task<ActionResult<ApiResponse<SparePartCategoryResponse>>> Update(int id, [FromBody] SparePartCategoryUpdateRequest request, CancellationToken ct = default)
        {
            var data = await _service.UpdateAsync(id, request, ct);
            if (data == null)
                return NotFound(ApiResponse<SparePartCategoryResponse>.ErrorResponse("SparePartCategory not found"));

            return Ok(ApiResponse<SparePartCategoryResponse>.SuccessResponse(data, "Updated"));
        }

        [HttpPatch("{id:int}")]
        [Authorize(Roles = ManagerRoles)]
        public async Task<ActionResult<ApiResponse<SparePartCategoryResponse>>> UpdateStatus(int id, [FromBody] SparePartUpdateStatusRequest request, CancellationToken ct = default)
        {
            var ok = await _service.UpdateStatusAsync(id, request.IsActive,ct);
            if (ok==null)
                return NotFound(ApiResponse<SparePartCategoryResponse>.ErrorResponse("SparePartCategory not found"));

            return Ok(ApiResponse<SparePartCategoryResponse>.SuccessResponse(ok, "Updated status"));
        }
    }
}
