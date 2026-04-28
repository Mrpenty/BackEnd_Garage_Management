using Garage_Management.Application.DTOs.Branches;
using Garage_Management.Application.Interfaces.Services.Branches;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BranchesController : ControllerBase
    {
        private const string AdminOnly = "Admin";
        private const string ReadRoles = "Admin,Supervisor,Receptionist,Stocker,Mechanic";

        private readonly IBranchService _service;

        public BranchesController(IBranchService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = ReadRoles)]
        public async Task<ActionResult<ApiResponse<PagedResult<BranchResponse>>>> GetPaged(
            [FromQuery] ParamQuery query,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(query, ct);
            return Ok(ApiResponse<PagedResult<BranchResponse>>.SuccessResponse(data, "Lấy danh sách chi nhánh thành công"));
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = ReadRoles)]
        public async Task<ActionResult<ApiResponse<BranchResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<BranchResponse>.ErrorResponse("Không tìm thấy chi nhánh"));

            return Ok(ApiResponse<BranchResponse>.SuccessResponse(data, "OK"));
        }

        [HttpPost]
        [Authorize(Roles = AdminOnly)]
        public async Task<ActionResult<ApiResponse<BranchResponse>>> Create(
            [FromBody] BranchCreateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = data.BranchId },
                ApiResponse<BranchResponse>.SuccessResponse(data, "Tạo chi nhánh thành công"));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = AdminOnly)]
        public async Task<ActionResult<ApiResponse<BranchResponse>>> Update(
            int id,
            [FromBody] BranchUpdateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.UpdateAsync(id, request, ct);
            if (data == null)
                return NotFound(ApiResponse<BranchResponse>.ErrorResponse("Không tìm thấy chi nhánh"));

            return Ok(ApiResponse<BranchResponse>.SuccessResponse(data, "Cập nhật chi nhánh thành công"));
        }

        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = AdminOnly)]
        public async Task<ActionResult<ApiResponse<BranchResponse>>> UpdateStatus(
            int id,
            [FromBody] BranchStatusUpdateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.UpdateStatusAsync(id, request.IsActive, ct);
            if (data == null)
                return NotFound(ApiResponse<BranchResponse>.ErrorResponse("Không tìm thấy chi nhánh"));

            return Ok(ApiResponse<BranchResponse>.SuccessResponse(data, "Cập nhật trạng thái thành công"));
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = AdminOnly)]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok)
                return NotFound(ApiResponse<object>.ErrorResponse("Không tìm thấy chi nhánh"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Xóa chi nhánh thành công"));
        }
    }
}
